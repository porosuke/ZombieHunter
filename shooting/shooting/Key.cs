using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shooting
{
    internal class Key
    {
        public static bool up, down, left, right, debug, click, reload;
        public static String weapon = "knife";
        //所持情報
        public static bool hg, ar, sg, sr;

        public void Push(KeyEventArgs e)
        {
            bool CanChange = (!Player.reload_now) && (!Player.shot_now) && (!Player.change_now);
            switch (e.KeyCode)
            {
                case Keys.A:
                    left = true;
                    break;
                case Keys.D:
                    right = true;
                    break;
                case Keys.W:
                    up = true;
                    break;
                case Keys.S:
                    down = true;
                    break;
                case Keys.Q:
                    if (CanChange) weapon = "knife";
                    break;
                case Keys.F1:
                    debug = !debug;
                    break;
                case Keys.D1:
                    if (hg && CanChange) weapon = "handgun";
                    break;
                case Keys.D2:
                    if (sg && CanChange) weapon = "shotgun";
                    break;
                case Keys.D3:
                    if (ar && CanChange) weapon = "carbine";
                    break;
                case Keys.D4:
                    if (sr && CanChange) weapon = "sniper";
                    break;
                case Keys.R:
                    if (CanChange && (weapon != "knife") && (reload == false)) reload = true;

                    break;
            }
        }
        public void Release(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    left = false;
                    break;
                case Keys.D:
                    right = false;
                    break;
                case Keys.W:
                    up = false;
                    break;
                case Keys.S:
                    down = false;
                    break;
            }
        }
        public void Mouse_down(MouseEventArgs e)
        {
            click = true;
        }
        public void Mouse_up(MouseEventArgs e)
        {
            click = false;
        }
    }
}
