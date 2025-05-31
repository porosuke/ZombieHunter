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
    internal class clsBullet_E
    {
        private const int claw_speed = 0;
        private const int claw_damage = 15;
        private const int claw_distance = 25;
        private const int claw_radius = 25;

        private const int bullet_speed = 300;
        private const int bullet_damage = 10;

        private string type;
        private int damage;
        private float x, y, dir, v, start_x, start_y;
        private Image img;
        private Player player;
        public PointF center;
        public bool dead, first = true;
        public int radius;

        public clsBullet_E(string startType, float startX, float startY, float startDir, Image startImg, Player p)
        {
            type = startType;
            img = startImg;
            x = startX;
            y = startY;
            start_x = startX;
            start_y = startY;
            dir = startDir;
            player = p;
            if (type == "claw")
            {
                v = claw_speed;
                radius = claw_radius;
                damage = claw_damage;
                x += (float)(Math.Cos(Math.PI / 180 * dir)) * claw_distance;
                y += (float)(Math.Sin(Math.PI / 180 * dir)) * claw_distance;
                center = new PointF(x, y);
            }
            else
            {
                v = bullet_speed;
                radius = img.Width / 2;
                damage = bullet_damage;
            }
        }
        public void Tick()
        {
            if (!dead)
            {
                //引っ掻きは動かない
                if (type == "claw")
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
            if ((!dead) && (type != "claw"))
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

            bool claw_out_of_range = true;
            float dx, dy;

            // プレイヤーに当たったら
            dx = player.center.X - c.X;
            dy = player.center.Y - c.Y;
            float radiusSum = r + player.img.Width / 2;
            if (dx * dx + dy * dy <= radiusSum * radiusSum)
            {
                if (first)
                {
                    Main.playSound.Play(SoundType.damage);
                    player.health -= damage;
                    if (player.health < 0) player.health = 0;
                    first = false;
                    player.damage_flag = true;

                    if (type == "claw") claw_out_of_range = false;
                    else dead = true;
                }
            }
            if (type == "claw" && claw_out_of_range) dead = true;
        }
        private void CollisionCheck(PointF c, int r)
        {
            if (dead) return;
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
