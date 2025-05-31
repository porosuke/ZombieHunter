using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using static shooting.Main;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace shooting
{
    internal class Player
    {
        //定数
        private const float knife_cooldown = 1.4f;
        private const float knife_changeAnimFrameTime = 0.25f;
        private const float knife_shotAnimFrameTime = 0.1f;

        private const float hg_cooldown = 0.7f;
        private const int hg_Allbullet = 14;
        private const int hg_default_mag = 6;
        private const float hg_changeAnimFrameTime = 0.3f;
        private const float hg_shotAnimFrameTime = 0.4f;
        private const float hg_reloadTime = 2.0f;
        private const int max_hg_b = 50;
        private const int mag_hg_b = 8;

        private const float sg_cooldown = 1.8f;
        private const int sg_Allbullet = 6;
        private const int sg_default_mag = 3;
        private const float sg_changeAnimFrameTime = 0.3f;
        private const float sg_shotAnimFrameTime = 1f;
        private const float sg_reloadTime = 0.6f;
        private const int max_sg_b = 30;
        private const int mag_sg_b = 4;

        private const float ar_cooldown = 0.5f;
        private const int ar_Allbullet = 20;
        private const int ar_default_mag = 12;
        private const float ar_changeAnimFrameTime = 0.4f;
        private const float ar_shotAnimFrameTime = 0.4f;
        private const float ar_reloadTime = 3.0f;
        private const int max_ar_b = 80;
        private const int mag_ar_b = 20;

        private const float sr_cooldown = 2.0f;
        private const int sr_Allbullet = 10;
        private const int sr_default_mag = 2;
        private const float sr_changeAnimFrameTime = 0.5f;
        private const float sr_shotAnimFrameTime = 2.6f;
        private const float sr_reloadTime = 0.8f;
        private const int max_sr_b = 20;
        private const int mag_sr_b = 5;

        private const float recoveryDelayTime = 10f;
        private const float recoveryMoveTime = 2f;
        private const float recoveryStopTime = 0.5f;

        private const float changeCooldown = 0.2f;

        //リスト
        public List<clsBullet_P> Bullets_P = new List<clsBullet_P>();
        //インスタンス作成
        private Move move = new Move();
        private clsBullet_P shot;
        //座標
        public PointF my, center;
        //速度
        public int speed;
        //回転角度
        public float angle;
        //死んだか
        public bool dead;
        //画像
        public Image img;
        //体力
        public int health, max_health, before_health;
        private float recoveryTimer = 0f;
        private float recoveryDelayTimer = 0f;
        //弾
        public int hg_b, hg_mag, ar_b, ar_mag, sg_b, sg_mag, sr_b, sr_mag;
        //武器の記録
        public String before_weapon = "knife";
        public bool damage_flag = false;
        //アニメーションタイマー
        public float weaponCooldown = 0f;
        private int changeAnimFrameIndex = 0;
        private int reloadAnimFrameIndex = 0;
        private int shotAnimFrameIndex = 0;
        private float changeAnimTimer = 0f;
        private float reloadAnimTimer = 0f;
        private float reloadFullAnimTimer = 0f;
        private float shotAnimTimer = 0f;
        private bool midSoundPlayed = false;
        //段階的なリロード用
        private float sg_full_reloadTime = -1;
        private int sg_need_amo = 0;
        private int sg_current_reloaded = 0;
        private float sr_full_reloadTime = -1;
        private int sr_need_amo = 0;
        private int sr_current_reloaded = 0;

        //bool
        public static bool reload_now,shot_now,change_now;
        private int dummy = 1;
        //取得メッセージ表示用
        public List<gainTextData> gainTextList = new List<gainTextData>();
        public struct gainTextData
        {
            public string text;
            public float x;
            public float y;
            public int alpha;
        }

        public Player(float x, float y, int s, float a, int h)
        {
            my = new PointF(x, y);
            speed = s;
            angle = a;
            health = h;
            max_health = h;
            dead = false;

            hg_b = hg_Allbullet;
            ar_b = ar_Allbullet;
            sg_b = sg_Allbullet;
            sr_b = sr_Allbullet;

            hg_mag = hg_default_mag;
            ar_mag = ar_default_mag;
            sg_mag = sg_default_mag;
            sr_mag = sr_default_mag;
            img = imageManager.playerBaseImg;
        }
        public void Draw(Graphics g)
        {
            center = new PointF(my.X + img.Width / 2.0f, my.Y + img.Height / 2.0f);
            foreach (clsBullet_P bullet_P in Bullets_P)
            {
                bullet_P.draw(g);
            }

            if (!dead)
            {
                g.ResetTransform();
                g.TranslateTransform(-center.X, -center.Y);
                g.RotateTransform(angle, MatrixOrder.Append);
                g.TranslateTransform(center.X, center.Y, MatrixOrder.Append);
                //被ダメージ用の白い丸を下に描画
                float radius = (img.Width / 2.0f) - 1;
                g.FillEllipse(System.Drawing.Brushes.White, center.X - radius, center.Y - radius, radius * 2, radius * 2);

                if (!damage_flag)
                {
                    switch (Key.weapon)
                    {
                        case "knife":
                        //切り替え
                        if (change_now) DrawAnimation(g, imageManager.knifeChangeAnim, changeAnimFrameIndex);
                        else
                        {
                            //振った後の硬直
                            if (weaponCooldown > 0 && !shot_now) DrawAnimation(g, imageManager.knifeSwingAnim, imageManager.knifeSwingAnim.Count - 1);
                            //通常
                            else DrawAnimation(g, imageManager.knifeSwingAnim, shotAnimFrameIndex);
                        }
                        break;
                        case "handgun":
                            ApplyAnimation(g, hg_mag, imageManager.hgChangeAnim, imageManager.hgShotAnim, imageManager.hgReloadAnim);
                        break;
                            case "shotgun":
                            ApplyAnimation(g, sg_mag, imageManager.sgChangeAnim, imageManager.sgShotAnim, imageManager.sgReloadAnim);
                        break;
                            case "carbine":
                            ApplyAnimation(g, ar_mag, imageManager.arChangeAnim, imageManager.arShotAnim, imageManager.arReloadAnim);
                        break;
                            case "sniper":
                            ApplyAnimation(g, sr_mag, imageManager.srChangeAnim, imageManager.srShotAnim, imageManager.srReloadAnim);
                        break;
                        }
                }
            }
            else
            {
                g.DrawImage(imageManager.playerDeadImg, center.X - imageManager.playerDeadImg.Width / 2.0f, center.Y - imageManager.playerDeadImg.Height / 2.0f);
                fadeout = true;
            }

        }
        private void ApplyAnimation(Graphics g, int mag, List<ImageManager.ImagePos> changeAnimList, List<ImageManager.ImagePos> shotAnimList, List<ImageManager.ImagePos> reloadAnimList)
        {
            //切り替え
            if (change_now) DrawAnimation(g, changeAnimList, changeAnimFrameIndex);
            //リロード
            else if (reload_now) DrawAnimation(g, reloadAnimList, reloadAnimFrameIndex);
            else
            {
                if(mag != 0) DrawAnimation(g, shotAnimList, shotAnimFrameIndex);
                //弾切れ
                else
                {
                    if (!shot_now)
                    {
                        if (Key.weapon == "sniper") DrawAnimation(g, shotAnimList, 2);
                        else DrawAnimation(g, shotAnimList, 3);
                    }
                    else DrawAnimation(g, shotAnimList, shotAnimFrameIndex);
                }
            }
        }
        private void DrawAnimation(Graphics g, List<ImageManager.ImagePos> animList, int index)
        {
            g.DrawImage(animList[index].img, center.X - animList[index].position.X, center.Y - animList[index].position.Y);
        }
        public void Tick(Point mouse, List<Enemy> enemies)
        {
            move.Pmove(speed, center, ref my, Main.wayPoint.All_Rect[now_stage], img.Width / 2, Key.up, Key.down, Key.left, Key.right);
            center = new PointF(my.X + img.Width / 2.0f, my.Y + img.Height / 2.0f);
            HealthManager();
            BulletChecker();
            ChangeDetecter();
            if(Key.reload) Reload();
            Item();
            Enter();
            Rotate(mouse);
            ShotAnimater();
            if ((!fadeout) && (!fadein) && (!reload_now) && (!change_now) && (!shot_now)) Shot(enemies);
            if((!fadeout) && (!fadein)) weaponCooldown -= Main.deltaTime;
            if (weaponCooldown < 0) weaponCooldown = 0f;
        }
        private void HealthManager()
        {
            if (health <= 0)
            {
                dead = true;
                health = 0;
                return;
            }

            // 被ダメージ時の処理（回復遅延の適用）
            if (before_health != health)
            {
                before_health = health;
                if (damage_flag)
                {
                    recoveryDelayTimer = recoveryDelayTime;
                    damage_flag = false;
                }
            }

            // 回復猶予時間が残っているなら、回復しない
            if (recoveryDelayTimer > 0f)
            {
                recoveryDelayTimer -= deltaTime;
                return;
            }

            // 移動中かどうかをチェック
            bool isMoving = Key.up || Key.down || Key.left || Key.right;

            float requiredTime = isMoving ? recoveryMoveTime : recoveryStopTime;

            recoveryTimer += deltaTime;

            if (recoveryTimer >= requiredTime)
            {
                recoveryTimer = 0f;
                health++;
                if (health > 100)
                    health = 100;
            }
        }
        private void BulletChecker()
        {
            for (int i = Bullets_P.Count - 1; i >= 0; i--)
            {
                Bullets_P[i].Tick();
                //死んでいたら消す
                if (Bullets_P[i].dead) Bullets_P.RemoveAt(i);
            }
        }
        private void ChangeDetecter()
        {
            if (before_weapon != Key.weapon)
            {
                switch (Key.weapon)
                {
                    case "knife":
                        Change(imageManager.knifeChangeAnim, knife_changeAnimFrameTime);
                        break;
                    case "handgun":
                        Change(imageManager.hgChangeAnim, hg_changeAnimFrameTime);
                        break;
                    case "shotgun":
                        Change(imageManager.sgChangeAnim, sg_changeAnimFrameTime);
                        break;
                    case "carbine":
                        Change(imageManager.arChangeAnim, ar_changeAnimFrameTime);
                        break;
                    case "sniper":
                        Change(imageManager.srChangeAnim, sr_changeAnimFrameTime);
                        break;
                }

            }
        }
        private void Change(List<ImageManager.ImagePos> interval, float changeAnimFrameTime)
        {
            //武器チェンジを始める
            if (!change_now)
            {
                Main.playSound.Play(SoundType.HGset);
                weaponCooldown = changeCooldown;
                change_now = true;
                changeAnimFrameIndex = 0;
                changeAnimTimer = 0f;
            }
            // 切り替えアニメ中
            if (change_now)
            {
                // 1フレームあたりの時間を計算
                float frameDuration = changeAnimFrameTime / interval.Count;
                changeAnimTimer += Main.deltaTime;

                // 時間が経過したら次のフレームへ
                if (changeAnimTimer >= frameDuration)
                {
                    changeAnimFrameIndex++;
                    changeAnimTimer -= frameDuration;
                }

                // アニメーション終了（最後のフレームを超えた）
                if (changeAnimFrameIndex >= interval.Count)
                {
                    changeAnimTimer = 0;
                    changeAnimFrameIndex = 0;
                    change_now = false;
                    before_weapon = Key.weapon;
                }
            }
        }
        private void Reload()
        {
            // マガジンいっぱい/弾がないときはリロード出来ない
            switch (Key.weapon)
            {
                case "handgun":
                    if (hg_mag != mag_hg_b && hg_b != 0)
                    {
                        Reloader(SoundType.HGmagrelease, SoundType.HGset, imageManager.hgReloadAnim, ref hg_b, ref hg_mag, mag_hg_b, hg_reloadTime);
                    }
                    else Key.reload = false;
                    break;

                case "shotgun":
                    if (sg_full_reloadTime < 0)
                    {
                        sg_need_amo = mag_sg_b - sg_mag;
                        if (sg_need_amo > sg_b) sg_need_amo = sg_b;
                        if (sg_need_amo != 0) sg_full_reloadTime = sg_reloadTime * (sg_need_amo + 2);
                        else sg_full_reloadTime = 0;
                        sg_current_reloaded = 0;
                        imageManager.ChangeReloadAnimation(Key.weapon, sg_need_amo);
                    }
                    Reloader(SoundType.SGreload, SoundType.SGset, imageManager.sgReloadAnim, ref sg_b, ref sg_mag, mag_sg_b, sg_full_reloadTime);
                    break;

                case "carbine":
                    if (ar_mag != mag_ar_b && ar_b != 0)
                    {
                        Reloader(SoundType.ARmagrelease, SoundType.ARrelease, imageManager.arReloadAnim, ref ar_b, ref ar_mag, mag_ar_b, ar_reloadTime);
                    }
                    else Key.reload = false;
                    break;

                case "sniper":
                    if(sr_full_reloadTime < 0)
                    {
                        sr_need_amo = mag_sr_b - sr_mag;
                        if (sr_need_amo > sr_b) sr_need_amo = sr_b;
                        if(sr_need_amo != 0) sr_full_reloadTime = sr_reloadTime * (sr_need_amo + 2);
                        else sr_full_reloadTime = 0;
                        sr_current_reloaded = 0;
                        imageManager.ChangeReloadAnimation(Key.weapon, sr_need_amo);
                    }
                    Reloader(SoundType.SRreload, SoundType.SRset, imageManager.srReloadAnim, ref sr_b, ref sr_mag, mag_sr_b, sr_full_reloadTime);
                    break;
            }
        }
        private void Reloader(SoundType release,SoundType set, List<ImageManager.ImagePos> interval, ref int All_b,ref int mag_b,int max_b, float reloadAnimFrameTime)
        {
            if (!reload_now)
            {
                if (reloadAnimFrameTime > 0f)
                {
                    if (Key.weapon == "shotgun") sg_current_reloaded = 0;
                    else if (Key.weapon == "sniper") sr_current_reloaded = 0;
                    else Main.playSound.Play(release);

                    reload_now = true;
                    reloadAnimTimer = 0f;
                    reloadFullAnimTimer = 0f;
                    midSoundPlayed = false;
                    reloadAnimFrameIndex = 0;
                }
                else
                {
                    // reloadDuration == 0f なら即完了
                    Key.reload = false;
                    if (Key.weapon == "shotgun") sg_full_reloadTime = -1;
                    if (Key.weapon == "sniper") sr_full_reloadTime = -1;
                }
            }

            if (reload_now)
            {
                reloadAnimTimer += Main.deltaTime;
                reloadFullAnimTimer += Main.deltaTime;

                // Shotgun: 一定時間ごとに音を鳴らして1発ずつリロード
                if (Key.weapon == "shotgun")
                {
                    if (reloadFullAnimTimer >= sg_reloadTime * sg_current_reloaded)
                    {
                        if (All_b > 0 && mag_b < max_b)
                        {
                            //最初と最後は無視
                            if(sg_current_reloaded != 0 && sg_current_reloaded != sg_need_amo + 2)
                            {
                                Main.playSound.Play(release);
                                All_b--;
                                mag_b++;
                            }

                            sg_current_reloaded++;
                        }
                    }
                }
                // Sniperも同様
                else if (Key.weapon == "sniper")
                {
                    if (reloadFullAnimTimer >= sr_reloadTime * sr_current_reloaded)
                    {
                        if (All_b > 0 && mag_b < max_b)
                        {
                            if (sr_current_reloaded != 0 && sr_current_reloaded != sr_need_amo + 2)
                            {
                                Main.playSound.Play(release);
                                All_b--;
                                mag_b++;
                            }
                            sr_current_reloaded++;
                        }
                    }
                }
                else
                {
                    // HG/AR: リロード時間の3/5でマガジンセット音
                    if ((Key.weapon == "handgun" || Key.weapon == "carbine") && !midSoundPlayed && reloadFullAnimTimer >= (reloadAnimFrameTime / 5f) * 3f)
                    {
                        if(Key.weapon == "handgun") Main.playSound.Play(SoundType.HGmagset);
                        if (Key.weapon == "carbine") Main.playSound.Play(SoundType.ARmagset);
                        midSoundPlayed = true;
                    }
                }

                // 1フレームあたりの時間を計算
                float frameDuration = reloadAnimFrameTime / interval.Count;
                if (reloadAnimTimer >= frameDuration)
                {
                    reloadAnimTimer -= frameDuration; // 超過分を持ち越し
                    reloadAnimFrameIndex++;

                    // 最後のフレームを過ぎたら終了
                    if (reloadAnimFrameIndex >= interval.Count)
                    {
                        if (Key.weapon != "shotgun" && Key.weapon != "sniper")
                        {
                            int needed = max_b - mag_b;
                            if (All_b >= needed)
                            {
                                All_b -= needed;
                                mag_b = max_b;
                            }
                            else
                            {
                                mag_b += All_b;
                                All_b = 0;
                            }
                        }

                        Main.playSound.Play(set);
                        reload_now = false;
                        Key.reload = false;
                        reloadAnimTimer = 0f;
                        reloadFullAnimTimer = 0f;
                        if (Key.weapon == "shotgun") sg_full_reloadTime = -1;
                        if (Key.weapon == "sniper") sr_full_reloadTime = -1;
                        midSoundPlayed = false;
                        reloadAnimFrameIndex = 0;
                    }
                }
            }
        }        
        private void Item()
        {
            //All_bを後ろから走査
            for (int i = All_Bullet.Count - 1; i >= 0; i--)
            {
                //今のステージにあるものなら
                if (All_Bullet[i].stage == now_stage)
                {
                    //ぶつかったら
                    if (ItemCollision(All_Bullet[i].x, All_Bullet[i].y, center.X, center.Y, img.Width / 2, All_Bullet[i].img.Width))
                    {
                        int gainBullet = All_Bullet[i].amount;
                        string bulletType = "";
                        switch (All_Bullet[i].type)
                        {
                            case "hg_b":
                                if (hg_b + gainBullet > max_hg_b) gainBullet = max_hg_b - hg_b;
                                hg_b += gainBullet;
                                bulletType = "小口径";
                                break;
                            case "ar_b":
                                if (ar_b + gainBullet > max_ar_b) gainBullet = max_ar_b - ar_b;
                                ar_b += gainBullet;
                                bulletType = "中口径";
                                break;
                            case "sg_b":
                                if (sg_b + gainBullet > max_sg_b) gainBullet = max_sg_b - sg_b;
                                sg_b += gainBullet;
                                bulletType = "ショットシェル";
                                break;
                            case "sr_b":
                                if (sr_b + gainBullet > max_sr_b) gainBullet = max_sr_b - sr_b;
                                sr_b += gainBullet;
                                bulletType = "大口径";
                                break;
                        }
                        //アイテムの中央座標を求める
                        float itemX = All_Bullet[i].x + All_Bullet[i].img.Width / 2.0f;
                        float itemY = All_Bullet[i].y + All_Bullet[i].img.Width / 2.0f;
                        gainTextList.Add(new gainTextData() { text = $"{bulletType}: +{gainBullet}", x = itemX, y = itemY, alpha = 255 });

                        All_Bullet.RemoveAt(i);
                        Main.playSound.Play(SoundType.BulletGet);
                    }
                }
            }
            //All_hを後ろから走査
            for (int i = All_heal.Count - 1; i >= 0; i--)
            {
                //今のステージにあるものなら
                if (All_heal[i].stage == now_stage)
                {
                    //ぶつかったら
                    if (ItemCollision(All_heal[i].x, All_heal[i].y, center.X, center.Y, img.Width / 2, All_heal[i].img.Width))
                    {
                        int gainHealth = All_heal[i].amount;
                        if (gainHealth + health > max_health) gainHealth = max_health - health;
                        health += gainHealth;

                        //アイテムの中央座標を求める
                        float x = All_heal[i].x + All_heal[i].img.Width / 2.0f;
                        float y = All_heal[i].y + All_heal[i].img.Width / 2.0f;
                        gainTextList.Add(new gainTextData() { text = $"体力: +{gainHealth}", x = x, y = y, alpha = 255});

                        All_heal.RemoveAt(i);
                        Main.playSound.Play(SoundType.care);
                    }
                }
            }
            //All_wを後ろから走査
            for (int i = All_weapon.Count - 1; i >= 0; i--)
            {
                //今のステージにあるものなら
                if (All_weapon[i].stage == now_stage)
                {
                    //ぶつかったら
                    if (ItemCollision(All_weapon[i].x, All_weapon[i].y, center.X, center.Y, img.Width / 2, All_weapon[i].img.Width))
                    {
                        switch (All_weapon[i].type)
                        {
                            case "hg":
                                Key.hg = true;
                                break;
                            case "ar":
                                Key.ar = true;
                                break;
                            case "sg":
                                Key.sg = true;
                                break;
                            case "sr":
                                Key.sr = true;
                                break;
                        }
                        All_weapon.RemoveAt(i);
                        Main.playSound.Play(SoundType.WeaponGet);
                    }
                }
            }
        }
        private bool ItemCollision(int obj_x, int obj_y, float x, float y, int radius, int width)
        {
            //大雑把に判定
            if (((obj_x - radius < x) && (obj_x + width + radius > x)) &&
                    ((obj_y - radius < y) && (obj_y + width + radius > y))) return true;
            return false;
        }
        private void Enter()
        {
            if (!fadeout)
            {
                for (int i = 0; i < Door_In.Count; i++)
                {
                    //今のステージにあるものなら
                    if ((Door_In[i].stage == now_stage) && (stage_enemy == 0))
                    {
                        //ぶつかったら
                        if (((Door_In[i].rect.X - img.Width / 2 < center.X) && (Door_In[i].rect.Right + img.Width / 2 > center.X)) &&
                           ((Door_In[i].rect.Y - img.Width / 2 < center.Y) && (Door_In[i].rect.Bottom + img.Width / 2 > center.Y)))
                        {
                            fadeout = true;
                            Bullets_P = new List<clsBullet_P>();
                        }
                    }
                }
            }
        }
        private void Rotate(Point m)
        {
            //旋回角度を計算
            //偏角を計算     Math.Atan2(y, x) * 180 / Math.PI
            angle = -(float)(Math.Atan2(center.X - m.X, center.Y - m.Y) * 180 / Math.PI);
        }
        private void ShotAnimater()
        {
            if (shot_now)
            {
                switch (Key.weapon)
                {
                    case "knife":
                        Shooter(SoundType.cut, imageManager.knifeSwingAnim, dummy, ref dummy, knife_shotAnimFrameTime);
                        break;
                    case "handgun":
                        Shooter(SoundType.HGshot, imageManager.hgShotAnim, hg_b, ref hg_mag, hg_shotAnimFrameTime);
                        break;
                    case "shotgun":
                        Shooter(SoundType.SGshot, imageManager.sgShotAnim, sg_b, ref sg_mag, sg_shotAnimFrameTime);
                        break;
                    case "carbine":
                        Shooter(SoundType.HGshot, imageManager.arShotAnim, ar_b, ref ar_mag, ar_shotAnimFrameTime);
                        break;
                    case "sniper":
                        Shooter(SoundType.SRshot, imageManager.srShotAnim, sr_b, ref sr_mag, sr_shotAnimFrameTime);
                        break;
                }
            }
        }
        private void Shooter(SoundType shot, List<ImageManager.ImagePos> interval, int All_b, ref int mag_b, float shotAnimFrameTime)
        {
            if (!shot_now)
            {
                if (mag_b == 0)
                {
                    Main.playSound.Play(SoundType.outofbullets);
                    weaponCooldown = 0.5f;
                }
                else
                {
                    Main.playSound.Play(shot);
                    if (Key.weapon != "knife") mag_b--;

                    // アニメ初期化
                    shot_now = true;
                    shotAnimTimer = 0f;
                    shotAnimFrameIndex = 1;
                    midSoundPlayed = false;
                }
            }

            if (shot_now)
            {
                shotAnimTimer += deltaTime;

                // 1フレームあたりの時間を計算
                float frameDuration = shotAnimFrameTime / interval.Count;
                // 次のフレームに進むタイミングか？
                if (shotAnimTimer >= frameDuration)
                {
                    shotAnimTimer -= frameDuration; // 超過分を持ち越し（安定性UP）
                    shotAnimFrameIndex++;

                    // 中間音（ポンプアクション）
                    int midIndex = (int)((interval.Count - 1) / 2.0f);
                    if (!midSoundPlayed && shotAnimFrameIndex == midIndex)
                    {
                        if ((Key.weapon == "shotgun") && (mag_b != 0)) Main.playSound.Play(SoundType.SGpomp);
                        if ((Key.weapon == "sniper") && (mag_b != 0)) Main.playSound.Play(SoundType.SRcock);
                        midSoundPlayed = true;
                    }

                    //弾切れのときは、指定フレームまでで終了
                    if(mag_b == 0)
                    {
                        if(Key.weapon == "sniper")
                        {
                            if(shotAnimFrameIndex == 2)
                            {
                                shot_now = false;
                                shotAnimFrameIndex = 0;
                                shotAnimTimer = 0f;
                            }
                        }
                        else
                        {
                            if(shotAnimFrameIndex == 3)
                            {
                                shot_now = false;
                                shotAnimFrameIndex = 0;
                                shotAnimTimer = 0f;
                            }
                        }
                    }

                    // 最後のフレームを過ぎたら終了
                    if (shotAnimFrameIndex >= interval.Count)
                    {
                        shot_now = false;
                        shotAnimFrameIndex = 0;
                        shotAnimTimer = 0f;
                    }
                }
            }
        }
        private void Shot(List<Enemy> Enemies)
        {
            if (Key.click && weaponCooldown <= 0)
            {
                switch (Key.weapon)
                {
                    case "knife":
                        weaponCooldown = knife_cooldown;
                        Bullets_P.Add(shot = new clsBullet_P("knife", center.X, center.Y, angle - 90, imageManager.playerSmallBulletImg, Enemies));
                        Shooter(SoundType.cut, imageManager.knifeSwingAnim, dummy, ref dummy, knife_shotAnimFrameTime);
                        Key.click = false;
                        break;

                    case "handgun":
                        weaponCooldown = hg_cooldown;
                        if (hg_mag != 0)
                            Bullets_P.Add(shot = new clsBullet_P("handgun", center.X - imageManager.playerSmallBulletImg.Width / 2, center.Y - imageManager.playerSmallBulletImg.Width / 2, angle - 90, imageManager.playerSmallBulletImg, Enemies));
                        Shooter(SoundType.HGshot, imageManager.hgShotAnim, hg_b, ref hg_mag, hg_shotAnimFrameTime);
                        Key.click = false;
                        break;

                    case "shotgun":
                        weaponCooldown = sg_cooldown;
                        if (sg_mag != 0)
                        {
                            Bullets_P.Add(shot = new clsBullet_P("shotgun", center.X - imageManager.playerSmallBulletImg.Width / 2, center.Y - imageManager.playerSmallBulletImg.Width / 2, angle + 5 - 90, imageManager.playerSmallBulletImg, Enemies));
                            Bullets_P.Add(shot = new clsBullet_P("shotgun", center.X - imageManager.playerSmallBulletImg.Width / 2, center.Y - imageManager.playerSmallBulletImg.Width / 2, angle - 90, imageManager.playerSmallBulletImg, Enemies));
                            Bullets_P.Add(shot = new clsBullet_P("shotgun", center.X - imageManager.playerSmallBulletImg.Width / 2, center.Y - imageManager.playerSmallBulletImg.Width / 2, angle - 5 - 90, imageManager.playerSmallBulletImg, Enemies));
                        }
                        Shooter(SoundType.SGshot, imageManager.sgShotAnim, sg_b, ref sg_mag, sg_shotAnimFrameTime);
                        Key.click = false;
                        break;

                    case "carbine":
                        weaponCooldown = ar_cooldown;
                        if (ar_mag != 0)
                            Bullets_P.Add(shot = new clsBullet_P("carbine", center.X - imageManager.playerLargeBulletImg.Width / 2, center.Y - imageManager.playerLargeBulletImg.Width / 2, angle - 90, imageManager.playerLargeBulletImg, Enemies));
                        Shooter(SoundType.HGshot, imageManager.arShotAnim, ar_b, ref ar_mag, ar_shotAnimFrameTime);
                        break;

                    case "sniper":
                        weaponCooldown = sr_cooldown;
                        if (sr_mag != 0)
                            Bullets_P.Add(shot = new clsBullet_P("sniper", center.X - imageManager.playerLargeBulletImg.Width / 2, center.Y - imageManager.playerLargeBulletImg.Width / 2, angle - 90, imageManager.playerLargeBulletImg, Enemies));
                        Shooter(SoundType.SRshot, imageManager.srShotAnim, sr_b, ref sr_mag, sr_shotAnimFrameTime);
                        Key.click = false;
                        break;
                }
            }
        }
    }
}
