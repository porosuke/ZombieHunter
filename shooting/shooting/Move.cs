using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shooting
{
    internal class Move
    {
        public void Pmove(int baseSpeed,PointF center,ref PointF my,Rectangle[] Rect,int radius, bool up,bool down,bool left,bool right)
        {
            float speed = baseSpeed * Main.deltaTime;
            //斜め移動用の値（√2で割っている）
            float s = speed / 1.4142f;

            if (up && left)
            {        //左上
                if (!Collision(Rect, center.X - s, center.Y, radius)) my.X -= s;
                if (!Collision(Rect, center.X, center.Y - s, radius)) my.Y -= s;
            }
            else if (up && right)
            {      //右上
                if (!Collision(Rect, center.X + s, center.Y, radius)) my.X += s;
                if (!Collision(Rect, center.X, center.Y - s, radius)) my.Y -= s;
            }
            else if (down && left)
            {     //左下
                if (!Collision(Rect, center.X - s, center.Y, radius)) my.X -= s;
                if (!Collision(Rect, center.X, center.Y + s, radius)) my.Y += s;
            }
            else if (down && right)
            {        //右下
                if (!Collision(Rect, center.X + s, center.Y, radius)) my.X += s;
                if (!Collision(Rect, center.X, center.Y + s, radius)) my.Y += s;
            }
            //通常移動
            else if (up && !down)       //上
            {
                if (!Collision(Rect, center.X, center.Y - speed, radius)) my.Y -= speed;
            }
            else if (down && !up)   //下
            {
                if (!Collision(Rect, center.X, center.Y + speed, radius)) my.Y += speed;
            }
            else if (left && !right)    //左
            {
                if (!Collision(Rect, center.X - speed, center.Y, radius)) my.X -= speed;
            }
            else if (right && !left)    //右
            {
                if (!Collision(Rect, center.X + speed, center.Y, radius)) my.X += speed;
            }
        }
        private bool Collision(Rectangle[] rect,float x,float y,int radius)
        {
            int gap = 1;
            radius -= gap;
            foreach(Rectangle r in rect)
            {
                //大雑把に判定
                if (((r.X - radius < x)&&(r.Right + radius > x))&&
                        ((r.Y - radius < y)&&(r.Bottom + radius > y))) return true;
            }
            return false;
        }
        public void Emove(int baseSpeed,PointF target,PointF center, ref PointF my, PointF player)
        {
            //目標までの距離を求める
            float target_dist = (float)Math.Sqrt(((target.X - center.X) * (target.X - center.X)) + ((target.Y - center.Y) * (target.Y - center.Y)));
            //プレイヤーまでの2乗距離を求める
            float player_dist = ((player.X - center.X) * (player.X - center.X)) + ((player.Y - center.Y) * (player.Y - center.Y));

            //プレイヤーに近づく限界距離
            int limit = 50;
            //プレイヤーに近いか
            bool isTooCloseToPlayer = player_dist < (limit * limit);

            float speed = baseSpeed * Main.deltaTime;

            if (!isTooCloseToPlayer)
            {
                my.X += (target.X - center.X) / target_dist * speed;
                my.Y += (target.Y - center.Y) / target_dist * speed;
            }
        }
    }
}
