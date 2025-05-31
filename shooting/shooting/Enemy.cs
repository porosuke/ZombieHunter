using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static shooting.Main;

namespace shooting
{
    internal class Enemy
    {
        //定数
        private const int clawDistance = 70;
        private float clawAnimFrameTime = 1.0f;
        private const int clawAnimFrame = 7;
        private float shotCoolTime = 5.0f;
        private const float zombieVoiceBaseCooltime = 3.0f;
        private const float RouteUpdateInterval = 0.1f;
        private const int bossBoostDistance = 300;

        //リスト
        public List<clsBullet_E> Bullets_E = new List<clsBullet_E>();
        private clsBullet_E shot;

        //インスタンス作成
        private Move move = new Move();
        //座標
        public PointF my, center;
        //速度
        public int speed;
        //回転角度
        public float angle;
        //死んだか
        public bool dead;
        //画像
        public Image img, bullet;
        //体力
        public int health, before_health;
        //行先のポイント
        public PointF target;
        //タイプ
        private bool shooter = false;
        private bool boss = false;
        //アニメーションタイマー
        private bool isClaw = false;
        private float clawAnimTimer = 0f;
        private float beforeAnimTimer = 0f;
        private int clawAnimFrameIndex = 0;
        private float shotAnimTimer = 0f;
        private bool isScratched = false;
        //乱数生成
        Random r = new Random();
        List<SoundType> zombie_sound = new List<SoundType>();
        private List<ImageManager.ImagePos> animationList;

        //完全ルート計算用
        public List<Point> shortest_route = new List<Point>();
        //射線が通っているか
        public bool isLineOfSightClear;
        //playerが直線上にいるか
        public bool isPlayerAhead;
        //音関係
        private float zombieVoiceTimer = 0;
        private float zombieVoiceTime = 0;
        //追加
        private Point enemyPoint;
        private Point playerPoint;
        private Point targetPoint;
        private float routeUpdateTimer = 0;
        //ボス
        private int phase = 1;
        private int baseSpeed = 0;
        private int fullHealth = 0;

        public Enemy(float x, float y, int s, float a, int h, String E_type)
        {
            my = new PointF(x, y);
            baseSpeed = s;
            speed = s;
            angle = a;
            health = h;
            before_health = h;
            fullHealth = h;
            dead = false;
            zombieVoiceTime = zombieVoiceBaseCooltime;

            if (E_type == "gun") shooter = true;
            if(E_type == "boss")
            {
                shooter = true;
                boss = true;
            }

            img = imageManager.enemyBaseImg;
            if (shooter)
            {
                if(boss)
                {
                    animationList = imageManager.bossEnemyAnimation;
                    bullet = imageManager.bossBulletImg;
                }
                else
                {
                    animationList = imageManager.specialEnemyAnimation;
                    bullet = imageManager.enemyBulletImg;
                }
            }
            else
            {
                animationList = imageManager.normalEnemyAnimation;
                bullet = imageManager.enemyBulletImg;
            }

            zombie_sound.Add(SoundType.zombie1_v);
            zombie_sound.Add(SoundType.zombie2_v);
            zombie_sound.Add(SoundType.zombie3_v);
            zombie_sound.Add(SoundType.zombie4_v);
            zombie_sound.Add(SoundType.zombie5_v);

            center = new PointF(my.X, my.Y);
        }
        public void Draw(ref Graphics g)
        {
            center = new PointF(my.X + img.Width / 2.0f, my.Y + img.Height / 2.0f);
            foreach (clsBullet_E bullet_E in Bullets_E)
            {
                bullet_E.draw(g);
            }
            g.ResetTransform();
            g.TranslateTransform(-center.X, -center.Y);
            g.RotateTransform((float)angle, MatrixOrder.Append);
            g.TranslateTransform(center.X, center.Y, MatrixOrder.Append);

            //被ダメージ用の白い丸を下に描画
            float radius = (img.Width / 2.0f) - 1;
            g.FillEllipse(System.Drawing.Brushes.White, center.X - radius, center.Y - radius, radius * 2, radius * 2);

            if (!dead)
            {
                if (before_health == health) g.DrawImage(animationList[clawAnimFrameIndex].img, center.X - animationList[clawAnimFrameIndex].position.X, center.Y - animationList[clawAnimFrameIndex].position.Y);
                before_health = health;
            }
        }
        public void Tick(PointF Player, Player p)
        {
            if (isClaw) Claw(p);
            else
            {
                Rotate(target);
                move.Emove(speed, target, center, ref my, Player);
            }

            center =  new PointF(my.X + img.Width / 2.0f, my.Y + img.Height / 2.0f);
            enemyPoint = new Point((int)center.X, (int)center.Y);
            playerPoint = new Point((int)Player.X, (int)Player.Y);
            targetPoint = new Point((int)target.X, (int)target.Y);

            isLineOfSightClear = !Main.wayPoint.CheckCollisionRect(Main.wayPoint.All_Rect[now_stage], center, Player);
            isPlayerAhead = CheckAheadPlayer(center, p);

            DistanceCheck(Player, p);
            CheckRoute();

            if (health <= 0)
            {
                Main.playSound.Play(SoundType.zombieDeath);
                dead = true;
            }
            VoiceManager();

            PhaseManager();
        }
        private void CheckRoute()
        {
            routeUpdateTimer += Main.deltaTime;
            if (routeUpdateTimer > RouteUpdateInterval)
            {
                routeUpdateTimer = 0;

                shortest_route = Main.wayPoint.Route(now_stage, enemyPoint, playerPoint);
                target = shortest_route[1];
            }
        }
        private void Claw(Player p)
        {
            if (clawAnimTimer > clawAnimFrameTime)
            {
                if (!isClaw)
                {
                    isClaw = true;
                    clawAnimFrameIndex = 0;
                    beforeAnimTimer = clawAnimTimer;
                    isScratched = false;
                }

                if (isClaw)
                {
                    // 1フレームあたりの時間を計算
                    float frameDuration = clawAnimFrameTime / animationList.Count;

                    // 時間が経過したら次のフレームへ
                    if (clawAnimTimer >= beforeAnimTimer + frameDuration)
                    {
                        clawAnimFrameIndex++;
                        beforeAnimTimer = clawAnimTimer;

                        //指定フレーム目のアニメーションで、判定が出る
                        if(!isScratched && clawAnimFrameIndex == clawAnimFrame)
                        {
                            playSound.Play(SoundType.claw);
                            Bullets_E.Add(shot = new clsBullet_E("claw", center.X, center.Y, angle - 90, bullet, p));
                            isScratched = true;
                        }
                    }
                    //完了したら
                    if (clawAnimFrameIndex >= animationList.Count)
                    {
                        clawAnimFrameIndex = 0;
                        clawAnimTimer = 0;
                        beforeAnimTimer = 0;
                        isClaw = false;
                    }
                }
            }
        }
        private void DistanceCheck(PointF Player, Player p)
        {
            //引っ掻き距離
            int claw_dist = clawDistance * clawDistance;
            if (shooter)
            {
                //近距離なら
                if (Main.wayPoint.DistanceSquared(enemyPoint, playerPoint) < claw_dist && !isClaw) Claw(p);
                //距離が一定以下なら
                else
                {
                    //視線が通っていたら(障害物に当たっていなかったら)
                    if (isLineOfSightClear && isPlayerAhead)
                    {
                        //弾を撃つ
                        if (shotAnimTimer > shotCoolTime)
                        {
                            if (boss)
                            {
                                //同時10方向弾
                                if(phase == 1)
                                {
                                    for (int i = 0; i < 360; i += 36)
                                    {
                                        Bullets_E.Add(shot = new clsBullet_E("shot", center.X - bullet.Width / 2, center.Y - bullet.Height / 2, angle - 90 + i, bullet, p));
                                    }
                                }
                                //前方向ずらし3連弾
                                if(phase == 2)
                                {
                                    _ = ShotBullet(angle - 90, p, 0);
                                    _ = ShotBullet(angle - 90 + 20, p, 250);
                                    _ = ShotBullet(angle - 90 - 20, p, 500);
                                }
                                //12方向10連スパイラル
                                if(phase == 3)
                                {
                                    for(int i = 0; i < 10; i++)
                                    {
                                        for(int j = 0; j < 12; j++)
                                        {
                                            _ = ShotBullet(angle - 90 + (j * 30) + (i * 36), p, (i * 36) * 10);
                                        }
                                    }
                                }
                            }
                            else Bullets_E.Add(shot = new clsBullet_E("shot", center.X - bullet.Width / 2, center.Y - bullet.Height / 2, angle - 90, bullet, p));

                            Main.playSound.Play(SoundType.zombieShot);
                            shotAnimTimer = 0;
                        }
                    }
                }
                shotAnimTimer += Main.deltaTime;
            }
            else
            {
                //近距離なら
                if (Main.wayPoint.DistanceSquared(enemyPoint, playerPoint) < claw_dist && !isClaw) Claw(p);
            }
            clawAnimTimer += Main.deltaTime;

            for (int i = Bullets_E.Count - 1; i >= 0; i--)
            {
                Bullets_E[i].Tick();
                //死んでいたら消す
                if (Bullets_E[i].dead) Bullets_E.RemoveAt(i);
            }
        }
        private async Task ShotBullet(float startDir, Player p, int intervalMs)
        {
            //指定ms待つ
            await Task.Delay(intervalMs);
            Bullets_E.Add(shot = new clsBullet_E("shot", center.X - bullet.Width / 2, center.Y - bullet.Height / 2, startDir, bullet, p));
        }
        private bool CheckAheadPlayer(PointF c, Player player)
        {
            //射線が通っていることが前提
            if (isLineOfSightClear)
            {
                //playerの当たり判定を生成
                RectangleF rect = new RectangleF(player.my.X, player.my.Y, player.img.Width, player.img.Height);

                //今自分が見ている方向2000先の座標
                float rad = (float)(Math.PI / 180 * (angle - 90));
                float x2 = center.X + 2000 * (float)Math.Cos(rad);
                float y2 = center.Y + 2000 * (float)Math.Sin(rad);
                PointF forward = new PointF(x2, y2);

                return Main.wayPoint.CheckCollision(center, forward, rect);
            }
            else return false;
        }
        private void Rotate(PointF p)
        {
            //旋回角度を計算
            //偏角を計算     Math.Atan2(y, x) * 180 / Math.PI
            angle = -(float)(Math.Atan2(center.X - p.X, center.Y - p.Y) * 180 / Math.PI);
        }
        private void VoiceManager()
        {
            if(zombieVoiceTimer > zombieVoiceTime)
            {
                Main.playSound.Play(zombie_sound[r.Next(zombie_sound.Count)]);
                //Baseの1~2倍の時間待つ
                zombieVoiceTime = zombieVoiceBaseCooltime * (1 + (float)r.NextDouble());
                zombieVoiceTimer = 0;
            }
            zombieVoiceTimer += Main.deltaTime;
        }
        private void PhaseManager()
        {
            if (boss)
            {
                //体力100~60%
                if(health >= fullHealth / 100 * 60)
                {
                    phase = 1;
                    speed = (int)(baseSpeed * 1.25f);
                    clawAnimFrameTime = 1.2f;
                    shotCoolTime = 4f;
                }
                //体力60~30%
                else if (health >= fullHealth / 100 * 30)
                {
                    if (phase == 1) playSound.Play(SoundType.phase2);
                    phase = 2;
                    speed = (int)(baseSpeed * 3);
                    clawAnimFrameTime = 0.8f;
                    shotCoolTime = 5f;
                }
                //体力30~0%
                else
                {
                    if (phase == 2) playSound.Play(SoundType.phase3);
                    phase = 3;
                    speed = (int)(baseSpeed * 0.8f);
                    clawAnimFrameTime = 0.6f;
                    shotCoolTime = 8f;
                }
            }
        }
    }
}
