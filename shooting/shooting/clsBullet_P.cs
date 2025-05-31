using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;

namespace shooting
{
    internal class clsBullet_P
    {
        //定数
        private const int knife_speed = 0;
        private const int knife_damage = 30;
        private const int knife_distance = 25;
        private const int knife_radius = 25;

        private const int handgun_speed = 900;
        private const int handgun_damage = 10;

        private const int shotgun_speed = 1200;
        private const int shotgun_damage = 10;

        private const int carbine_speed = 1800;
        private const int carbine_damage = 14;
        private const int carbine_blur = 2;

        private const int sniper_speed = 2100;
        private const int sniper_damage = 20;

        private string type;
        private int damage;
        private float x, y, dir, v,start_x,start_y;
        private Image img;
        private List<Enemy> enemies;
        private List<Enemy> hit_enemies;
        public PointF center;
        public bool dead;
        public int radius;


        public clsBullet_P(string startType, float startX, float startY, float startDir, Image startImg, List<Enemy> Enemies)
        {
            type = startType;
            img = startImg;
            x = startX;
            y = startY;
            start_x = startX;
            start_y = startY;
            dir = startDir;
            enemies = Enemies;
            hit_enemies = new List<Enemy>();
            if (type == "knife") radius = knife_radius;
            else radius = img.Width / 2;

                switch (type)
                {
                    case "knife":
                        v = knife_speed;
                        damage = knife_damage;
                        x += (float)(Math.Cos(Math.PI / 180 * dir)) * knife_distance;
                        y += (float)(Math.Sin(Math.PI / 180 * dir)) * knife_distance;
                        center = new PointF(x, y);
                        break;

                    case "handgun":
                        v = handgun_speed;
                        damage = handgun_damage;
                        break;

                    case "shotgun":
                        v = shotgun_speed;
                        damage = shotgun_damage;
                        break;

                    case "carbine":
                        v = carbine_speed;
                        damage = carbine_damage;
                        var rand = new Random();
                        //集弾率
                        dir += rand.Next(-carbine_blur, carbine_blur);
                        break;
                    case "sniper":
                        v = sniper_speed;
                        damage = sniper_damage;
                        break;
                }
        }
        public void Tick()
        {
            if (!dead)
            {
                //ナイフは動かない
                if (type == "knife")
                {
                    DamageCheck(center, radius);
                }
                else
                {
                    float distance = v * Main.deltaTime;

                    int steps = (int)(distance / 1f);
                    float stepSize = distance / Math.Max(steps, 1);

                    for (int i = 0; i < steps; i++)
                    {
                        center = new PointF(x + radius, y + radius);
                        DamageCheck(center, radius);
                        CollisionCheck(center, radius);

                        x += (float)(Math.Cos(Math.PI / 180 * dir)) * stepSize;
                        y += (float)(Math.Sin(Math.PI / 180 * dir)) * stepSize;
                    }
                }
            }
        }
        public void draw(Graphics gr)
        {
            if ((!dead)&&(type != "knife"))
            {
                gr.ResetTransform();
                gr.TranslateTransform(-x + -radius, -y + -radius);
                gr.RotateTransform((float)dir, MatrixOrder.Append);
                gr.TranslateTransform(x + radius, y + radius, MatrixOrder.Append);
                gr.DrawImage(img, x, y);
            }
        }
        private void DamageCheck(PointF c, int r)
        {
            if (dead) return;

            bool knife_out_of_range = true;
            foreach (Enemy enemy in enemies)
            {
                float dx, dy;

                // 敵に当たったら
                dx = enemy.center.X - c.X;
                dy = enemy.center.Y - c.Y;
                float radiusSum = r + enemy.img.Width / 2;
                if (dx * dx + dy * dy <= radiusSum * radiusSum)
                {
                    // 初めて当たる敵なら処理
                    if (!hit_enemies.Contains(enemy))
                    {
                        Main.playSound.Play(SoundType.damage);
                        hit_enemies.Add(enemy);
                        enemy.health -= damage;

                        // knifeでもsniperでもないなら、当たったら消える
                        if (type != "knife" && type != "sniper")
                        {
                            dead = true;
                        }
                        else if(type == "knife") knife_out_of_range = false;
                    }
                }
            }
            // 空振り（誰にも当たらなかった）
            if (type == "knife" && knife_out_of_range) dead = true;
        }
        private void CollisionCheck(PointF c, int r)
        {
            if(dead) return;
            //ぶつかったら死ぬ
            foreach (Rectangle rect in Main.wayPoint.All_Rect[Main.now_stage])
            {
                if (((rect.X - r < c.X) && (rect.Right + r > c.X)) &&
                        ((rect.Y - r < c.Y) && (rect.Bottom + r > c.Y)))
                    dead = true;
            }
        }
    }
}
