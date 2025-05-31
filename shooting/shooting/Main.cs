using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shooting
{
    public partial class Main : Form
    {
        private const int fade_speed = 240;
        private Rectangle mainInvalidateSize = new Rectangle(0, 0, 1300, 500);
        private Rectangle weaponInvalidateSize;
        private Rectangle iconInvalidateSize;
        private Rectangle HPInvalidateSize;

        //マップのインスタンス
        public static WayPoint wayPoint = new WayPoint();
        //画像管理クラスのインスタンス
        public static ImageManager imageManager = new ImageManager();
        //敵のリスト
        List<Enemy> Enemies = new List<Enemy>();
        //弾薬配置リスト
        public static List<Bullet> All_Bullet = new List<Bullet>();
        //回復配置リスト
        public static List<Heal_item> All_heal = new List<Heal_item>();
        //武器配置リスト
        public static List<Weapon> All_weapon = new List<Weapon>();
        //敵配置リスト
        public static List<Enemy_data> All_Enemy = new List<Enemy_data>();
        //開始ドア位置のリスト
        public static List<Door_data> Door_In = new List<Door_data>();
        //終わりドアのリスト
        public static List<Door_data> Door_Out = new List<Door_data>();
        //インスタンス
        private Enemy enemy;
        private Key key = new Key();
        private Player player;
        //ドアの大きさ
        private const int door_width = 50;
        private const int door_height = 25;
        //ステージ
        public static int now_stage = 0;
        public static int Max_stage = 0;
        public static bool clear;
        private int stage_copy = -1;
        //サイズ調整用
        private const float scale = 1f;
        //ペン
        Pen pen1 = new Pen(Color.Black, 10);
        Pen pen2 = new Pen(Color.White, 10);
        //半透明ブラシ
        Brush player_bullet_brush = new SolidBrush(Color.FromArgb(128, Color.Red));
        Brush enemy_bullet_brush = new SolidBrush(Color.FromArgb(128, Color.Pink));
        //描画用ポイント
        Point[] p1,p2,p3,p4;
        //カーソル
        Cursor BLOCK, HG, AR, SG, SR, KN;
        //カーソル用ポイント
        Point sp, cp;
        //保存用
        private int now_score;
        private int high_score;
        private String high_string;
        //音
        public static PlaySound playSound = new PlaySound();
        public struct Bullet
        {
            public int stage;
            public int x;
            public int y;
            public String type;
            public int amount;
            public Image img;
        }
        public struct Heal_item
        {
            public int stage;
            public int x;
            public int y;
            public int amount;
            public Image img;
        }
        public struct Weapon
        {
            public int stage;
            public int x;
            public int y;
            public String type;
            public Image img;
        }
        public struct Enemy_data
        {
            public int stage;
            public int x;
            public int y;
            public int health;
            public int speed;
            public float angle;
            public String type;
        }
        public struct Door_data
        {
            public int stage;
            public Rectangle rect;
        }
        //これはッペンです！！(ブラシ)
        public SolidBrush[] hokusai = new SolidBrush[256];
        private SolidBrush[] textBrush = new SolidBrush[256];
        //フェード
        public static bool fadeout,fadein;
        private int time = 0;
        private bool dead = false;
        private string beforeTimeText, nowTimeText;

        //ステージにいる敵数
        public static int stage_enemy, stage_All_enemy;
        //Dataパス
        private String filePath;
        //FPS用
        private int frameCount = 0;
        private float elapsed = 0;
        private float fps = 0;
        private CancellationTokenSource gameLoopToken;
        //タイマー用
        private bool isTimerRunning = false;
        private DateTime startDT = DateTime.Now;

        public static float deltaTime = 0;


        //以下Windowシステム-------------------------------------------------------------------------

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool MoveWindow(
        IntPtr hWnd,
        int X,
        int Y,
        int nWidth,
        int nHeight,
        bool bRepaint);
        public void SetWindowBounds(Rectangle rect)
        {
            //MaximumSizeを大きくしておく
            if (this.MaximumSize.Width < rect.Width)
                this.MaximumSize =
                    new Size(rect.Width, this.MaximumSize.Height);
            if (this.MaximumSize.Height < rect.Height)
                this.MaximumSize =
                    new Size(this.MaximumSize.Width, rect.Height);

            MoveWindow(this.Handle, rect.X, rect.Y,
                rect.Width, rect.Height, true);
            this.UpdateBounds();
        }

        //以下初期化--------------------------------------------------------------------------------

        public Main()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //初期化
            Setup();
            CheckFile();
            StageMake();
            UI_Arrangement();
            Max_stage = Main.wayPoint.All_Points.Count - 1;
            //ゲーム開始
            StartGameLoop();
        }
        private void Setup()
        {
            //windowサイズ固定
            Text = "Zombie Hunter";
            Rectangle window_rect = new Rectangle(0, 0, 1310, 800);
            SetWindowBounds(window_rect);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.DoubleBuffered = true;
            BackColor = Color.White;
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            BLOCK = new Cursor(asm.GetManifestResourceStream(asm.GetName().Name + ".Block.cur"));
            HG = new Cursor(asm.GetManifestResourceStream(asm.GetName().Name + ".HG.cur"));
            AR = new Cursor(asm.GetManifestResourceStream(asm.GetName().Name + ".AR.cur"));
            SG = new Cursor(asm.GetManifestResourceStream(asm.GetName().Name + ".SG.cur"));
            SR = new Cursor(asm.GetManifestResourceStream(asm.GetName().Name + ".SR.cur"));
            KN = new Cursor(asm.GetManifestResourceStream(asm.GetName().Name + ".KN.cur"));
            for (int i = 0; i < 256; i++)
            {
                hokusai[i] = new SolidBrush(Color.FromArgb(i, 0, 0, 0));
                textBrush[i] = new SolidBrush(Color.FromArgb(i, 255, 215, 0));
            }
            playSound.Init();

            //描画サイズ

            weaponInvalidateSize = new Rectangle(ClientRectangle.X + 10, ClientRectangle.Bottom - 250,
                ClientRectangle.Width / 5 - ClientRectangle.Width / 10 + 80, 130);
            iconInvalidateSize = new Rectangle(ClientRectangle.X + (int)(5 * scale), ClientRectangle.Bottom - (int)(95 * scale),
                205, 45);
            HPInvalidateSize = new Rectangle((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(35 * scale),
                ClientRectangle.Bottom - 215,
                ((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2) - (int)(30 * scale)) - ((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(40 * scale)),
                60);

            //x/y/speed/angle/health/List
            player = new Player(625, 375, 150, 0, 100);
        }
        private void CheckFile()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string foldername = "Game";
            string fileName = "Highscore.txt";
            string foundPath = FindFileUpwards(exePath, foldername, fileName);

            if (foundPath == null)
            {
                Console.WriteLine($"{foldername}に{fileName}が見つかりませんでした。");
                MessageBox.Show($"The {fileName} file could not be found in {foldername} folder. Check that it has not been renamed or deleted.\n" +
                        $"{foldername}フォルダーに{fileName}ファイルが見つかりませんでした。名前の変更や削除がされていないか確認してください。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            else filePath = foundPath;
        }
        private string FindFileUpwards(string startDirectory, string targetFolderName, string fileName)
        {
            DirectoryInfo current = new DirectoryInfo(startDirectory);

            while (current != null)
            {
                string targetDirPath = Path.Combine(current.FullName, targetFolderName);
                string targetFilePath = Path.Combine(targetDirPath, fileName);

                if (Directory.Exists(targetDirPath) && File.Exists(targetFilePath))
                {
                    return targetFilePath;
                }

                current = current.Parent;
            }

            return null;
        }
        private void StageMake()
        {
            //stage,x,y,amotype,amount,img
            MakeBullet(1, 50, 50, "hg_b", 5, imageManager.hgBulletIcon);
            MakeBullet(1, 1200, 50, "hg_b", 2, imageManager.hgBulletIcon);
            MakeBullet(2, 1000, 200, "sg_b", 3, imageManager.sgBulletIcon);
            MakeBullet(3, 50, 50, "hg_b", 4, imageManager.hgBulletIcon);
            MakeBullet(3, 1200, 50, "sg_b", 6, imageManager.sgBulletIcon);
            MakeBullet(3, 850, 400, "ar_b", 4, imageManager.arBulletIcon);
            MakeBullet(3, 1200, 400, "hg_b", 3, imageManager.hgBulletIcon);
            MakeBullet(4, 450, 400, "sr_b", 3, imageManager.srBulletIcon);
            MakeBullet(4, 1200, 300, "ar_b", 10, imageManager.arBulletIcon);
            MakeBullet(4, 1150, 50, "sg_b", 4, imageManager.sgBulletIcon);
            MakeBullet(5, 150, 200, "ar_b", 10, imageManager.arBulletIcon);
            MakeBullet(5, 550, 300, "hg_b", 6, imageManager.hgBulletIcon);
            MakeBullet(6, 1000, 400, "sg_b", 5, imageManager.sgBulletIcon);
            MakeBullet(6, 200, 50, "sr_b", 4, imageManager.srBulletIcon);
            MakeBullet(6, 1400, 200, "ar_b", 12, imageManager.arBulletIcon);
            MakeBullet(6, 50, 250, "hg_b", 8, imageManager.hgBulletIcon);

            //stage,x,y,amount
            MakeHealth_item(2, 1200, 50, 30);
            MakeHealth_item(4, 700, 250, 20);
            MakeHealth_item(5, 1000, 200, 50);

            //stage,x,y,weapontype,img

            MakeWeapon(1, 250, 300, "hg", imageManager.hgStageIcon);
            MakeWeapon(2, 150, 150, "sg", imageManager.sgStageIcon);
            MakeWeapon(3, 550, 300, "ar", imageManager.arStageIcon);
            MakeWeapon(4, 50, 50, "sr", imageManager.srStageIcon);

            //stage,x,y,health,speed,type
            MakeEnemy(1, 550, 150, 50, 90, "claw");
            MakeEnemy(2, 800, 100, 50, 90, "claw");
            MakeEnemy(2, 1100, 300, 50, 90, "claw");
            MakeEnemy(2, 1200, 400, 50, 120, "claw");
            MakeEnemy(3, 450, 100, 50, 90, "claw");
            MakeEnemy(3, 1200, 300, 80, 120, "gun");
            MakeEnemy(4, 200, 150, 50, 120, "claw");
            MakeEnemy(4, 450, 50, 80, 90, "gun");
            MakeEnemy(4, 1100, 150, 80, 150, "gun");
            MakeEnemy(4, 1100, 300, 50, 120, "claw");
            MakeEnemy(4, 1100, 500, 50, 90, "claw");
            MakeEnemy(5, 100, 100, 80, 120, "gun");
            MakeEnemy(5, 600, 300, 80, 150, "gun");
            MakeEnemy(5, 1200, 400, 80, 90, "gun");

            MakeEnemy(6, 800, 400, 50, 150, "claw");
            MakeEnemy(6, 250, 50, 80, 90, "gun");
            MakeEnemy(6, 600, 350, 500, 60, "boss");

            //stage,x,y,type(in->playerが入っていける)
            MakeDoor(0, 625, 50, "in", door_width, door_height);
            MakeDoor(1, 625, 400, "out", door_width, door_width);
            MakeDoor(1, 625, 50, "in", door_width, door_height);
            MakeDoor(2, 625, 400, "out", door_width, door_width);
            MakeDoor(2, 1225, 400, "in", door_height, door_width);
            MakeDoor(3, 50, 400, "out", door_width, door_width);
            MakeDoor(3, 1225, 250, "in", door_height, door_width);
            MakeDoor(4, 50, 250, "out", door_width, door_width);
            MakeDoor(4, 1200, 50, "in", door_width, door_height);
            MakeDoor(5, 50, 400, "out", door_width, door_width);
            MakeDoor(5, 1200, 50, "in", door_width, door_height);
            MakeDoor(6, 1200, 400, "out", door_width, door_width);
            MakeDoor(6, 625, 50, "in", door_width, door_height);
            MakeDoor(7, 625, 400, "out", door_width, door_width);
        }
        private void MakeBullet(int s, int X, int Y, String t, int a, Image i)
        {
            Bullet b1 = new Bullet()
            {
                stage = s,
                x = X,
                y = Y,
                type = t,
                amount = a,
                img = i,
            };
            All_Bullet.Add(b1);
        }
        private void MakeHealth_item(int s, int X, int Y, int a)
        {
            Heal_item h1 = new Heal_item()
            {
                stage = s,
                x = X,
                y = Y,
                amount = a,
                img = imageManager.healthStageIcon
            };
            All_heal.Add(h1);
        }
        private void MakeWeapon(int s, int X, int Y, String t, Image i)
        {
            Weapon w1 = new Weapon()
            {
                stage = s,
                x = X,
                y = Y,
                type = t,
                img = i,
            };
            All_weapon.Add(w1);
        }
        private void MakeEnemy(int s, int X, int Y, int h, int sp, String t)
        {
            Random rand = new Random();
            Enemy_data e1 = new Enemy_data()
            {
                stage = s,
                x = X,
                y = Y,
                health = h,
                angle = rand.Next(360),
                speed = sp,
                type = t
            };
            All_Enemy.Add(e1);
        }
        private void MakeDoor(int s, int X, int Y, String t, int width, int height)
        {
            Door_data d1 = new Door_data()
            {
                stage = s,
                rect = new Rectangle(X, Y, width, height),
            };
            if (t == "in") Door_In.Add(d1);
            else Door_Out.Add(d1);
        }
        private void UI_Arrangement()
        {
            TimerLabel.Text = "00:00:00.00";
            TimerLabel.Location = new Point((ClientRectangle.Width / 2 - TimerLabel.Size.Width / 2) + 60, ClientRectangle.Bottom - TimerLabel.Size.Height + 10);
            Emark.Location = new Point((ClientRectangle.Width / 5) * 4 + 30, ClientRectangle.Bottom - 255);
            Enum.Location = new Point((ClientRectangle.Width / 5) * 4 + 70, ClientRectangle.Bottom - 200);
            Smark.Location = new Point((ClientRectangle.Width / 5) * 4 + 20, ClientRectangle.Bottom - 135);
            Snum.Location = new Point((ClientRectangle.Width / 5) * 4 + 60, ClientRectangle.Bottom - 70);
            HPmark.Location = new Point((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) - 90, ClientRectangle.Bottom - 220);
            HPnum.Location = new Point((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2) - 10, ClientRectangle.Bottom - 220);
            Highmark.Location = new Point(ClientRectangle.Width / 2 - Highmark.Width, ClientRectangle.Bottom - (TimerLabel.Height + Highscore.Height) + 10);
            Highscore.Location = new Point(ClientRectangle.Width / 2, ClientRectangle.Bottom - (TimerLabel.Height + Highscore.Height) + 10);
            try
            {

                //highscoreを開く
                CheckFile();
                using (StreamReader sr = new StreamReader(filePath))
                {
                    high_score = int.Parse(sr.ReadLine());
                    high_string = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Highscore.Text = high_string;

            Q.Location = new Point(ClientRectangle.X, ClientRectangle.Bottom - 50);
            D1.Location = new Point(ClientRectangle.X + 40, ClientRectangle.Bottom - 50);
            D1.Text = "";
            D2.Location = new Point(ClientRectangle.X + 80, ClientRectangle.Bottom - 50);
            D2.Text = "";
            D3.Location = new Point(ClientRectangle.X + 120, ClientRectangle.Bottom - 50);
            D3.Text = "";
            D4.Location = new Point(ClientRectangle.X + 160, ClientRectangle.Bottom - 50);
            D4.Text = "";
            amonum.Location = new Point(ClientRectangle.X + 200, ClientRectangle.Bottom - 160);
            death_b.Visible = false;


            //武器表示のところ
            p1 = new Point[4];
            p1[0] = new Point(ClientRectangle.X, ClientRectangle.Bottom - 260);
            p1[1] = new Point(ClientRectangle.Width / 5 + 50, ClientRectangle.Bottom - 260);
            p1[2] = new Point(ClientRectangle.Width / 5 - ClientRectangle.Width / 10 + 50, ClientRectangle.Bottom - 110);
            p1[3] = new Point(ClientRectangle.X, ClientRectangle.Bottom - 110);
            //タイマー表示のところ
            p2 = new Point[4];
            p2[0] = new Point((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2), ClientRectangle.Bottom - 80);
            p2[1] = new Point((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2), ClientRectangle.Bottom - 80);
            p2[2] = new Point(((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2)) + (int)(70 * scale), ClientRectangle.Bottom);
            p2[3] = new Point(((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2)) - (int)(70 * scale), ClientRectangle.Bottom);
            //体力左
            p3 = new Point[4];
            p3[0] = new Point((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(5 * scale), ClientRectangle.Bottom - 210);
            p3[1] = new Point((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(35 * scale), ClientRectangle.Bottom - 230);
            p3[2] = new Point((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(35 * scale), ClientRectangle.Bottom - 140);
            p3[3] = new Point((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(5 * scale), ClientRectangle.Bottom - 160);
            //体力右
            p4 = new Point[4];
            p4[0] = new Point((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2) + (int)(-35 * scale), ClientRectangle.Bottom - 230);
            p4[1] = new Point((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2) + (int)(-5 * scale), ClientRectangle.Bottom - 210);
            p4[2] = new Point((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2) + (int)(-5 * scale), ClientRectangle.Bottom - 160);
            p4[3] = new Point((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2) + (int)(-35 * scale), ClientRectangle.Bottom - 140);

        }
        private async void StartGameLoop()
        {
            //GameTimerTickの代わりにこっちを使う
            var sw = new Stopwatch();
            sw.Start();
            long prev = sw.ElapsedMilliseconds;

            gameLoopToken = new CancellationTokenSource();
            var token = gameLoopToken.Token;

            while (!token.IsCancellationRequested)
            {
                long now = sw.ElapsedMilliseconds;
                deltaTime = (now - prev) / 1000f;
                prev = now;

                //ゲームの主ループ
                UpdateGame();
                //FPSの計測
                FPScounter();
                GameTimer();
                //再描画
                InvalidateForm();

                await Task.Delay(1);
            }
        }
        private void FPScounter()
        {
            frameCount++;
            elapsed += deltaTime;
            if (elapsed >= 1f)
            {
                fps = frameCount / elapsed;
                frameCount = 0;
                elapsed = 0;
            }
        }
        private void StartTimer()
        {
            startDT = DateTime.Now;
            isTimerRunning = true;
        }
        private void StopTimer()
        {
            isTimerRunning = false;
        }
        private void GameTimer()
        {
            if (isTimerRunning)
            {
                TimeSpan currentElapsed = (DateTime.Now - startDT);
                TimerLabel.Text = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    currentElapsed.Hours, currentElapsed.Minutes, currentElapsed.Seconds, currentElapsed.Milliseconds / 10);
            }
        }
        private void InvalidateForm()
        {
            //メイン画面
            this.Invalidate(mainInvalidateSize);
            //武器アイコン
            this.Invalidate(iconInvalidateSize);
            //武器アイテム
            this.Invalidate(weaponInvalidateSize);
            //HP
            this.Invalidate(HPInvalidateSize);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            gameLoopToken?.Cancel();
            base.OnFormClosing(e);
        }

        //以下イベントハンドラ------------------------------------------------------------------------

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {//押された
            key.Push(e);
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {//離れた
            key.Release(e);
        }
        private void death_b_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            key.Mouse_up(e);
        }
        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) key.Mouse_down(e);
        }

        //以下描画----------------------------------------------------------------------------------

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //描画(Invalidateで呼ばれる)
            if (Max_stage != 0)
            {
                Graphics g = e.Graphics;

                g.ResetTransform();
                DrawStage(g);
                DrawItem(g);
                player.Draw(g);
                foreach (Enemy enemies in Enemies)
                {
                    enemies.Draw(ref g);
                }

                DrawUI(g);
                //Debug
                DebugView(g);
            }
        }
        private void DrawStage(Graphics g)
        {
            if (Key.debug)
            {
                //障害物を塗りつぶす
                foreach (Rectangle rect in wayPoint.All_Rect[now_stage])
                    g.FillRectangle(Brushes.Black, rect);
                // 点を可視化
                for(int i = 0; i < wayPoint.All_Points[now_stage].Count; i++)
                {
                    FillCircle(g, Brushes.Gray, wayPoint.All_Points[now_stage][i], 2);
                    g.DrawString(i.ToString(), new Font("Arial", 10), Brushes.Red, wayPoint.All_Points[now_stage][i]);
                }
                //範囲線の表示
                foreach (Rectangle rect in Main.wayPoint.All_Rect_ex[now_stage])
                    g.DrawRectangle(Pens.Orange, rect);
                //outドアの表示
                foreach (Door_data door in Door_Out)
                    if (door.stage == now_stage) g.FillRectangle(Brushes.Red, door.rect);
            }
            else
            {
                //通常時
                //床・壁→障害物・装飾の順に描画
                foreach(ImageManager.ImagePos imagePos in imageManager.stageBaseMaps[now_stage])
                {
                    if(imagePos.img != null) g.DrawImage(imagePos.img, imagePos.position);
                }
                foreach (ImageManager.ImagePos imagePos in imageManager.stageItemMaps[now_stage])
                {
                    if (imagePos.img != null) g.DrawImage(imagePos.img, imagePos.position);
                }
            }
        }
        private void DrawItem(Graphics g)
        {
            g.ResetTransform();
            foreach (Bullet bullet_item in All_Bullet) if (bullet_item.stage == now_stage) g.DrawImage(bullet_item.img, bullet_item.x, bullet_item.y);
            foreach (Heal_item health_item in All_heal) if (health_item.stage == now_stage) g.DrawImage(health_item.img, health_item.x, health_item.y);
            foreach (Weapon weapon_item in All_weapon) if (weapon_item.stage == now_stage) g.DrawImage(weapon_item.img, weapon_item.x, weapon_item.y);
            for (int i = 0; i < Door_In.Count; i++)
            {
                if (Door_In[i].stage == now_stage)
                {
                    //全員倒したら
                    if (stage_enemy == 0) g.FillRectangle(Brushes.Green, Door_In[i].rect);
                    else g.FillRectangle(Brushes.Red, Door_In[i].rect);
                }
            }
        }
        private void DrawUI(Graphics g)
        {
            g.ResetTransform();
            //ベース
            g.FillRectangle(Brushes.DarkSlateGray, ClientRectangle.X, ClientRectangle.Bottom - 260, ClientRectangle.Right, ClientRectangle.Bottom);
            //UI枠
            g.DrawRectangle(pen1, ClientRectangle.X, ClientRectangle.Bottom - 260, ClientRectangle.Right, 260);
            //右仕切り線
            g.DrawLine(pen1, (ClientRectangle.Width / 5) * 4, ClientRectangle.Bottom - 260, (ClientRectangle.Width / 5) * 4, ClientRectangle.Bottom);
            //特殊線
            g.DrawPolygon(pen1, p1);
            g.DrawPolygon(pen1, p2);
            g.DrawPolygon(pen1, p3);
            g.DrawPolygon(pen1, p4);
            //HP
            g.FillRectangle(Brushes.DarkRed, (ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(40 * scale), ClientRectangle.Bottom - 210,
            ((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2) - (int)(40 * scale)) - ((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(40 * scale)), 50);
            g.FillRectangle(Brushes.LightGreen, (ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(40 * scale), ClientRectangle.Bottom - 210,
                (int)((((ClientRectangle.Width / 2) + (TimerLabel.Size.Width / 2) - (int)(40 * scale)) - ((ClientRectangle.Width / 2) - (TimerLabel.Size.Width / 2) + (int)(40 * scale))) * ((float)player.health / (float)player.max_health)), 50);

            g.DrawImage(imageManager.knifeIconUI, ClientRectangle.X + (int)(10 * scale), ClientRectangle.Bottom - (int)(90 * scale));
            if (Key.hg)
            {
                g.DrawImage(imageManager.hgIconUI, ClientRectangle.X + (int)(30 * scale) + 20, ClientRectangle.Bottom - (int)(90 * scale));
                D1.Text = "1";
            }
            if (Key.sg)
            {
                g.DrawImage(imageManager.sgIconUI, ClientRectangle.X + (int)(30 * scale) + 60, ClientRectangle.Bottom - (int)(90 * scale));
                D2.Text = "2";
            }
            if (Key.ar)
            {
                g.DrawImage(imageManager.arIconUI, ClientRectangle.X + (int)(30 * scale) + 100, ClientRectangle.Bottom - (int)(90 * scale));
                D3.Text = "3";
            }
            if (Key.sr)
            {
                g.DrawImage(imageManager.srIconUI, ClientRectangle.X + (int)(30 * scale) + 140, ClientRectangle.Bottom - (int)(90 * scale));
                D4.Text = "4";
            }
            if (!player.dead)
            {
                switch (Key.weapon)
                {
                    case "knife":
                        g.DrawImage(imageManager.knifeUI, ClientRectangle.X + 20, ClientRectangle.Bottom - 230);
                        amonum.Text = "- / -";
                        break;
                    case "handgun":
                        g.DrawImage(imageManager.hgUI, ClientRectangle.X + 20, ClientRectangle.Bottom - 230);
                        amonum.Text = $"{player.hg_mag}/{player.hg_b}";
                        break;
                    case "shotgun":
                        g.DrawImage(imageManager.sgUI, ClientRectangle.X + 20, ClientRectangle.Bottom - 230);
                        amonum.Text = $"{player.sg_mag}/{player.sg_b}";
                        break;
                    case "carbine":
                        g.DrawImage(imageManager.arUI, ClientRectangle.X + 20, ClientRectangle.Bottom - 230);
                        amonum.Text = $"{player.ar_mag}/{player.ar_b}";
                        break;
                    case "sniper":
                        g.DrawImage(imageManager.srUI, ClientRectangle.X + 20, ClientRectangle.Bottom - 230);
                        amonum.Text = $"{player.sr_mag}/{player.sr_b}";
                        break;
                }
            }

            HPnum.Text = player.health.ToString();
            Enum.Text = $"{stage_enemy} / {stage_All_enemy}";
            if ((fadeout) || (fadein)) g.FillRectangle(hokusai[time], 0, 0, ClientRectangle.Right, ClientRectangle.Bottom - 260);

            if (dead)
            {
                DrawText(g, new PointF(this.ClientSize.Width / 2f, this.ClientSize.Height / 3f), "死んでしまった！", Brushes.White, 50);
                death_b.Visible = true;
            }
            if (clear)
            {
                float x = this.ClientSize.Width / 2f;
                float y = this.ClientSize.Height / 10f;
                DrawText(g, new PointF(x, y * 1.2f), "ゲームクリア！", Brushes.Black, 50);
                DrawText(g, new PointF(x, y * 2.5f), "おめでとう！", Brushes.Black, 50);
                DrawText(g, new PointF(x, y * 4), beforeTimeText, Brushes.Black, 50);
                DrawText(g, new PointF(x, y * 5), nowTimeText, Brushes.Black, 50);

                death_b.Visible = true;
            }

            //アイテム取得時のText表示(後ろから)
            for(int i = player.gainTextList.Count - 1; i >= 0; i--)
            {
                Player.gainTextData data = player.gainTextList[i];
                DrawOutlineText(g, new PointF(data.x, data.y), data.text, 15, data.alpha);
                //上に動かす(1sで5)
                data.y -= 5 * deltaTime;
                //透明にしていく(1sで-64、4sで消える)
                data.alpha -= (int)(64 * deltaTime);

                //alphaが0未満になったら削除する
                if(data.alpha < 0)
                {
                    player.gainTextList.RemoveAt(i);
                }
                else
                {
                    //更新
                    player.gainTextList[i] = data;
                }
            }
        }
        private void DebugView(Graphics g)
        {
            if (Key.debug)
            {
                //FPS表示
                Font font = new Font("Consolas", 14);
                Brush brush = new SolidBrush(Color.LimeGreen);
                bool isCoolDown = player.weaponCooldown > 0;
                g.DrawString($"FPS : {fps:F1} / Change : {Player.change_now}, Shot : {Player.shot_now}, Reload : {Player.reload_now}, CoolDown : {isCoolDown}", font, brush, 0, 0);

                //描画範囲を可視化
                g.FillRectangle(player_bullet_brush, iconInvalidateSize);
                g.FillRectangle(player_bullet_brush, HPInvalidateSize);
                g.FillRectangle(player_bullet_brush, weaponInvalidateSize);

                //player中心の表示
                FillCircle(g, Brushes.Blue, player.center, 5);
                //playerサイズの表示
                g.DrawRectangle(Pens.Red, player.center.X - (player.img.Width / 2.0f), player.center.Y - (player.img.Height / 2.0f), player.img.Width, player.img.Height);
                //player弾判定の表示
                foreach (clsBullet_P cls_bp in player.Bullets_P)
                {
                    FillCircle(g, player_bullet_brush, cls_bp.center, cls_bp.radius);
                }

                foreach (Enemy enemy in Enemies)
                {
                    //enemy中心の表示
                    FillCircle(g, Brushes.Blue, enemy.center, 5);
                    //enemyサイズの表示
                    g.DrawRectangle(Pens.Red, enemy.center.X - (enemy.img.Width / 2.0f), enemy.center.Y - (enemy.img.Height / 2.0f), enemy.img.Width, enemy.img.Height);
                    //最短距離の表示
                    if (enemy.shortest_route != null)
                    {
                        for (int i = 0; i < enemy.shortest_route.Count - 1; i++)
                        {
                            g.DrawLine(Pens.Magenta, enemy.shortest_route[i], enemy.shortest_route[i + 1]);
                        }
                    }
                    //enemyのtarget座標の表示
                    FillCircle(g, Brushes.Green, enemy.target, 6);
                    //enemyの射線の表示（前方にプレイヤーを捉えているなら赤、捉えていないが射線が通っているなら緑）
                    if (enemy.isPlayerAhead) g.DrawLine(Pens.Red, enemy.center, player.center);
                    else if (enemy.isLineOfSightClear) g.DrawLine(Pens.YellowGreen, enemy.center, player.center);

                    //enemy弾判定の表示
                    foreach (clsBullet_E cls_be in enemy.Bullets_E)
                    {
                        FillCircle(g, enemy_bullet_brush, cls_be.center, cls_be.radius);
                    }
                }
            }
        }
        private void FillCircle(Graphics g, Brush brush, PointF center, float radius)
        {
            g.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }
        private void DrawText(Graphics g, PointF center, string text, Brush brush, int size)
        {
            Font font = new Font("MS UI Gothic", size);
            SizeF textSize = g.MeasureString(text, font);
            PointF drawPoint = new PointF(center.X - textSize.Width / 2f, center.Y - textSize.Height / 2f);
            g.DrawString(text, font, brush, drawPoint);
        }
        private void DrawOutlineText(Graphics g, PointF center, string text, int size, int alpha)
        {
            int outlineSize = 1;
            //黒く縁どる
            DrawText(g, new PointF(center.X + outlineSize, center.Y), text, hokusai[alpha], size);
            DrawText(g, new PointF(center.X - outlineSize, center.Y), text, hokusai[alpha], size);
            DrawText(g, new PointF(center.X, center.Y + outlineSize), text, hokusai[alpha], size);
            DrawText(g, new PointF(center.X, center.Y - outlineSize), text, hokusai[alpha], size);
            //金色文字
            DrawText(g, new PointF(center.X, center.Y), text, textBrush[alpha], size);
        }

        //以下ゲーム関連関数--------------------------------------------------------------------------

        private void UpdateGame()
        {
            //事実上のTick

            //画面座標でマウスポインタの位置を取得する
            sp = Cursor.Position;
            //画面座標をクライアント座標に変換する
            cp = this.PointToClient(sp);

            //ステージ変更を検知
            if (stage_copy != now_stage) StageChange();
            //プレイヤー処理
            if ((!fadeout) && (!fadein)) if (!player.dead) player.Tick(cp, Enemies);
            //敵処理
            if ((!fadeout) && (!fadein))
            {
                for (int i = Enemies.Count - 1; i >= 0; i--)
                {
                    Enemies[i].Tick(player.center, player);
                    if (Enemies[i].dead)
                    {
                        Enemies.RemoveAt(i);
                        stage_enemy--;
                    }
                }
            }
            //カーソルの変更
            if (!dead) CursorChange();
            //フェード
            Fade();
        }
        private void StageChange()
        {
            if (now_stage == Max_stage) clear = true;
            foreach (Enemy_data e_data in All_Enemy)
            {
                if (e_data.stage == now_stage)
                {
                    Enemies.Add(enemy = new Enemy(e_data.x, e_data.y, e_data.speed, e_data.angle, e_data.health, e_data.type));
                    stage_enemy++;
                }
            }
            stage_All_enemy = stage_enemy;
            stage_copy = now_stage;
            if (now_stage == 1)
            {
                StartTimer();
            }
            if (clear)
            {
                playSound.Play(SoundType.clear);
                StopTimer();

                String my_s;
                my_s = TimerLabel.Text;
                now_score = 0;
                //hours
                now_score += int.Parse(my_s.Substring(0, 2)) * 3600000;
                //minutes
                now_score += int.Parse(my_s.Substring(3, 2)) * 60000;
                //seconds
                now_score += int.Parse(my_s.Substring(6, 2)) * 1000;
                //ミリ秒
                now_score += int.Parse(my_s.Substring(9, 2));

                if (now_score < high_score)
                {
                    beforeTimeText = $"新記録 : {TimerLabel.Text}";
                    nowTimeText = $"前ハイスコア : {high_string}";
                    //上書き
                    high_score = now_score;
                    high_string = TimerLabel.Text;
                }
                else
                {
                    beforeTimeText = $"ハイスコア : {high_string}";
                    nowTimeText = $"あなたのスコア : {TimerLabel.Text}";
                }

                try
                {
                    //High_scoreを出力(上書き)
                    CheckFile();
                    using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        sw.WriteLine(high_score);
                        sw.Write(high_string);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            Snum.Text = $"{now_stage} / {Max_stage}";
            foreach (Door_data door in Door_Out) if (door.stage == now_stage)
            {
                player.my.X = door.rect.X;
                player.my.Y = door.rect.Y;
            }
        }
        private void CursorChange()
        {
            if(Player.reload_now || Player.change_now || Player.shot_now || player.weaponCooldown > 0)
            {
                this.Cursor = BLOCK;
                return;
            }
            switch (Key.weapon)
            {
                case "knife":
                    this.Cursor = KN;
                    break;
                case "handgun":
                    this.Cursor = HG;
                    break;
                case "shotgun":
                    this.Cursor = SG;
                    break;
                case "carbine":
                    this.Cursor = AR;
                    break;
                case "sniper":
                    this.Cursor = SR;
                    break;
            }
        }
        private void Fade()
        {
            if (fadeout)
            {
                if (time == 0)
                {
                    if (!player.dead) playSound.Play(SoundType.Stagein);
                }

                time += (int)(fade_speed * Main.deltaTime);
                if (time > 255)
                {
                    time = 255;
                    if (!player.dead)
                    {
                        fadein = true;
                        fadeout = false;
                        now_stage++;
                    }
                    else if (!dead)
                    {
                        playSound.Play(SoundType.down);
                        playSound.Play(SoundType.zombieparty);
                        dead = true;
                        this.Cursor = Cursors.Default;
                        StopTimer();
                    }
                }
            }
            else if (fadein)
            {
                if (time >= 255) playSound.Play(SoundType.Stageout);
                time -= (int)(fade_speed * Main.deltaTime);
                if (time < 0)
                {
                    time = 0;
                    fadein = false;
                    fadeout = false;
                }
            }

        }
    }
}
