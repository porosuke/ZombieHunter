using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static shooting.ImageManager;
using static shooting.Player;
using static System.Windows.Forms.AxHost;

namespace shooting
{
    public class ImageManager
    {
        private const float imageScale = 0.5f;
        private const int tileSize = 100;

        private Image playerMapchip, normalEnemyMapchip, specialEnemyMapchip, bossEnemyMapchip, weaponMapchip, stageMapchip;
        public Image playerBaseImg, playerDeadImg, enemyBaseImg;
        public Image playerSmallBulletImg, playerLargeBulletImg, enemyBulletImg, bossBulletImg;
        public Image knifeStageIcon, hgStageIcon, sgStageIcon, arStageIcon, srStageIcon,
                     healthStageIcon, hgBulletIcon, sgBulletIcon, arBulletIcon, srBulletIcon;
        public Image knifeIconUI, hgIconUI, sgIconUI, arIconUI, srIconUI,
                     knifeUI, hgUI, sgUI, arUI, srUI;

        public List<List<ImagePos>> stageBaseMaps = new List<List<ImagePos>>();
        public List<List<ImagePos>> stageItemMaps = new List<List<ImagePos>>();
        public List<ImagePos> normalEnemyAnimation = new List<ImagePos>();
        public List<ImagePos> specialEnemyAnimation = new List<ImagePos>();
        public List<ImagePos> bossEnemyAnimation = new List<ImagePos>();
        public List<ImagePos> knifeChangeAnim = new List<ImagePos>();
        public List<ImagePos> knifeSwingAnim = new List<ImagePos>();
        public List<ImagePos> hgChangeAnim = new List<ImagePos>();
        public List<ImagePos> hgShotAnim = new List<ImagePos>();
        public List<ImagePos> hgReloadAnim = new List<ImagePos>();
        public List<ImagePos> sgChangeAnim = new List<ImagePos>();
        public List<ImagePos> sgShotAnim = new List<ImagePos>();
        public List<ImagePos> sgReloadAnim = new List<ImagePos>();
        public List<ImagePos> arChangeAnim = new List<ImagePos>();
        public List<ImagePos> arShotAnim = new List<ImagePos>();
        public List<ImagePos> arReloadAnim = new List<ImagePos>();
        public List<ImagePos> srChangeAnim = new List<ImagePos>();
        public List<ImagePos> srShotAnim = new List<ImagePos>();
        public List<ImagePos> srReloadAnim = new List<ImagePos>();

        private List<ImagePos> sgAnimList, srAnimList;

        private Image tDoor___, bDoor___, monument, stair___;
        public struct ImagePos
        {
            public Image img;
            public Point position;
        }

        public ImageManager()
        {
            LoadMapchip();
            MakePlayerImg();
            MakeEnemyImg();
            MakeUIImg();
            MakeStageMap();
        }

        private Bitmap Division(Image baseImg, int x, int y, int width, int height, float scale)
        {
            //画像切り出し
            Rectangle sourceRectange = new Rectangle(new Point(x, y), new Size(width, height));
            Bitmap bitmap1 = new Bitmap((int)(width * scale * imageScale), (int)(height * scale * imageScale));
            Graphics graphics = Graphics.FromImage(bitmap1);
            graphics.DrawImage(baseImg, new Rectangle(0, 0, (int)(width * scale * imageScale), (int)(height * scale * imageScale)), sourceRectange, GraphicsUnit.Pixel);
            graphics.Dispose();
            return bitmap1;
        }
        private void LoadMapchip()
        {
            playerMapchip = Properties.Resources.player;
            normalEnemyMapchip = Properties.Resources.zombie_normal;
            specialEnemyMapchip = Properties.Resources.zombie_special;
            bossEnemyMapchip = Properties.Resources.zombie_boss;
            weaponMapchip = Properties.Resources.weapon;
            stageMapchip = Properties.Resources.stage;
        }
        private void MakePlayerImg()
        {
            float playerScale = 0.5f;
            playerBaseImg = Division(playerMapchip, 20, 280, 200, 200, playerScale);
            playerDeadImg = Division(playerMapchip, 16, 60, 207, 215, playerScale);

            //アニメーションの切り出し
            List<ImagePos> knAnimList = MakePlayerAnimation(0, playerScale, 1, 11);
            List<ImagePos> hgAnimList = MakePlayerAnimation(1, playerScale, 0, 11);
            sgAnimList =                MakePlayerAnimation(2, playerScale, 0, 11);
            List<ImagePos> arAnimList = MakePlayerAnimation(3, playerScale, 0, 11);
            srAnimList =                MakePlayerAnimation(4, playerScale, 0, 11);
            //武器ごとに割り振る
            knifeChangeAnim = SetPlayerAnimation(knAnimList, new int[] { 0, 1, 2, 3, 4 });
            knifeSwingAnim = SetPlayerAnimation(knAnimList, new int[] { 5, 6, 7, 8, 9, 10 });
            hgChangeAnim = SetPlayerAnimation(hgAnimList, new int[] { 0, 1, 2, 3 });
            hgShotAnim = SetPlayerAnimation(hgAnimList, new int[] { 4, 5, 7, 7, 6, 4, 4 });
            hgReloadAnim = SetPlayerAnimation(hgAnimList, new int[] { 8, 9, 10, 11, 2, 3 });
            sgChangeAnim = SetPlayerAnimation(sgAnimList, new int[] { 0, 1, 2, 3 });
            sgShotAnim = SetPlayerAnimation(sgAnimList, new int[] {4, 5, 5, 6, 6, 4 });
            sgReloadAnim = SetPlayerAnimation(sgAnimList, new int[] { 7, 8, 9, 10, 11, 9, 8, 7 });
            arChangeAnim = SetPlayerAnimation(arAnimList, new int[] { 0, 1, 2, 3 });
            arShotAnim = SetPlayerAnimation(arAnimList, new int[] { 4, 5, 5, 6, 6, 4 });
            arReloadAnim = SetPlayerAnimation(arAnimList, new int[] { 7, 8, 8, 9, 9, 9, 9, 8, 8, 8, 9, 10, 10, 10, 10});
            srChangeAnim = SetPlayerAnimation(srAnimList, new int[] { 0, 1, 2, 3 });
            srShotAnim = SetPlayerAnimation(srAnimList, new int[] { 4, 5, 7, 6, 4, 4 });
            srReloadAnim = SetPlayerAnimation(srAnimList, new int[] { 8, 9, 10, 11, 9, 8 });
            //弾の切り出し
            playerLargeBulletImg = Division(playerMapchip, 0, 0, 60, 60, 0.5f);
            playerSmallBulletImg = Division(playerMapchip, 60, 0, 30, 30, 0.8f);
        }
        private List<ImagePos> MakePlayerAnimation(int yIndex, float scale, int startIndex, int endIndex)
        {
            int width = 400;
            int height = 500;
            int gapX = 120;
            int gapY = 380;

            List<ImagePos> animList = new List<ImagePos>();
            for(int i = startIndex; i <= endIndex; i++)
            {
                ImagePos imagePos = new ImagePos();
                imagePos.position = new Point((int)(gapX * scale * imageScale), (int)(gapY * scale * imageScale));
                imagePos.img = Division(playerMapchip, width * i, height * yIndex, width, height, scale);
                animList.Add(imagePos);
            }
            return animList;
        }
        private List<ImagePos> SetPlayerAnimation(List<ImagePos> animList, int[] indexs)
        {
            List<ImagePos> imagePos = new List<ImagePos>();

            for(int i = 0; i < indexs.Length; i++)
            {
                imagePos.Add(animList[indexs[i]]);
            }
            return imagePos;
        }
        public void ChangeReloadAnimation(string weapon, int reloadNum)
        {
            //指定されたNumに応じてアニメーションを差し替える
            if(weapon == "shotgun")
            {
                sgReloadAnim.Clear();
                sgReloadAnim.Add(sgAnimList[7]);
                sgReloadAnim.Add(sgAnimList[8]);
                sgReloadAnim.Add(sgAnimList[9]);
                for (int i = 0; i < reloadNum; i++)
                {
                    sgReloadAnim.Add(sgAnimList[10]);
                    sgReloadAnim.Add(sgAnimList[10]);
                    sgReloadAnim.Add(sgAnimList[11]);
                    sgReloadAnim.Add(sgAnimList[11]);
                }
                sgReloadAnim.Add(sgAnimList[9]);
                sgReloadAnim.Add(sgAnimList[8]);
                sgReloadAnim.Add(sgAnimList[7]);
            }
            if(weapon == "sniper")
            {
                srReloadAnim.Clear();
                srReloadAnim.Add(srAnimList[8]);
                srReloadAnim.Add(srAnimList[9]);
                for (int i = 0; i < reloadNum; i++)
                {
                    srReloadAnim.Add(srAnimList[10]);
                    sgReloadAnim.Add(sgAnimList[10]);
                    srReloadAnim.Add(srAnimList[11]);
                    sgReloadAnim.Add(sgAnimList[11]);
                }
                srReloadAnim.Add(srAnimList[9]);
                srReloadAnim.Add(srAnimList[8]);
            }
        }
        private void MakeEnemyImg()
        {
            float enemyScale = 0.5f;
            enemyBaseImg = Division(normalEnemyMapchip, 80, 180, 200, 200, enemyScale);
            //アニメーションの切り出し
            SetEnemyAnimation(360 * 1, 400 * 0, enemyScale);
            SetEnemyAnimation(360 * 2, 400 * 0, enemyScale);
            SetEnemyAnimation(360 * 3, 400 * 0, enemyScale);
            SetEnemyAnimation(360 * 0, 400 * 1, enemyScale);
            SetEnemyAnimation(360 * 0, 400 * 1, enemyScale);
            SetEnemyAnimation(360 * 2, 400 * 0, enemyScale);
            SetEnemyAnimation(360 * 1, 400 * 0, enemyScale);
            //引っ掻き
            SetEnemyAnimation(360 * 1, 400 * 1, enemyScale);
            SetEnemyAnimation(360 * 2, 400 * 1, enemyScale);
            SetEnemyAnimation(360 * 3, 400 * 1, enemyScale);
            for(int i = 0; i < 10; i++) SetEnemyAnimation(360 * 3, 400 * 1, enemyScale);
            //弾の切り出し
            enemyBulletImg = Division(specialEnemyMapchip, 0, 0, 60, 60, 0.8f);
            bossBulletImg = Division(bossEnemyMapchip, 0, 0, 60, 60, 0.6f);
        }
        private void SetEnemyAnimation(int x, int y, float scale)
        {
            int width = 360;
            int height = 400;
            int gapX = 180;
            int gapY = 280;

            ImagePos imagePos = new ImagePos();
            imagePos.position = new Point((int)(gapX * scale * imageScale), (int)(gapY * scale * imageScale));

            imagePos.img = Division(normalEnemyMapchip, x, y, width, height, scale);
            normalEnemyAnimation.Add(imagePos);
            imagePos.img = Division(specialEnemyMapchip, x, y, width, height, scale);
            specialEnemyAnimation.Add(imagePos);
            imagePos.img = Division(bossEnemyMapchip, x, y, width, height, scale);
            bossEnemyAnimation.Add(imagePos);
        }
        private void MakeUIImg()
        {
            knifeStageIcon =    Division(weaponMapchip, tileSize * 0 + 4, tileSize * 0 + 3, tileSize, tileSize, 1);
            hgStageIcon =       Division(weaponMapchip, tileSize * 1 + 3, tileSize * 0 + 3, tileSize, tileSize, 1);
            sgStageIcon =       Division(weaponMapchip, tileSize * 2 + 0, tileSize * 0 + 3, tileSize, tileSize, 1);
            arStageIcon =       Division(weaponMapchip, tileSize * 3 - 2, tileSize * 0 + 3, tileSize, tileSize, 1);
            srStageIcon =       Division(weaponMapchip, tileSize * 4 - 3, tileSize * 0 + 3, tileSize, tileSize, 1);
            healthStageIcon =   Division(weaponMapchip, tileSize * 0 + 4, tileSize * 1 - 2, tileSize, tileSize, 1);
            hgBulletIcon =      Division(weaponMapchip, tileSize * 1 + 3, tileSize * 1 - 2, tileSize, tileSize, 1);
            sgBulletIcon =      Division(weaponMapchip, tileSize * 2 + 0, tileSize * 1 - 2, tileSize, tileSize, 1);
            arBulletIcon =      Division(weaponMapchip, tileSize * 3 - 2, tileSize * 1 - 2, tileSize, tileSize, 1);
            srBulletIcon =      Division(weaponMapchip, tileSize * 4 - 3, tileSize * 1 - 2, tileSize, tileSize, 1);

            knifeIconUI =       Division(weaponMapchip, tileSize * 0 + 4, tileSize * 0 + 3, tileSize, tileSize, 0.7f);
            hgIconUI =          Division(weaponMapchip, tileSize * 1 + 3, tileSize * 0 + 3, tileSize, tileSize, 0.7f);
            sgIconUI =          Division(weaponMapchip, tileSize * 2 + 0, tileSize * 0 + 3, tileSize, tileSize, 0.7f);
            arIconUI =          Division(weaponMapchip, tileSize * 3 - 2, tileSize * 0 + 3, tileSize, tileSize, 0.7f);
            srIconUI =          Division(weaponMapchip, tileSize * 4 - 3, tileSize * 0 + 3, tileSize, tileSize, 0.7f);
            knifeUI =           Division(weaponMapchip, 500 * 1, 200 * 0, 500, 200, 0.8f);
            hgUI =              Division(weaponMapchip, 500 * 0, 200 * 1, 500, 200, 0.8f);
            sgUI =              Division(weaponMapchip, 500 * 1, 200 * 1, 500, 200, 0.8f);
            arUI =              Division(weaponMapchip, 500 * 0, 200 * 2, 500, 200, 0.8f);
            srUI =              Division(weaponMapchip, 500 * 1, 200 * 2, 500, 200, 0.8f);
        }
        private void MakeStageMap()
        {
            //パーツ(8文字に合わせる)------------------------------------------------------------------------
            //共通パーツ----------------------------------------------------------------------------
            //t->top, r->right, b->bottom, l->left
            Image dummy___ =  null;
            tDoor___ =       Division(stageMapchip, tileSize * 0, tileSize * 0, tileSize, tileSize, 1);
            Image rDoor___ = Division(stageMapchip, tileSize * 1, tileSize * 0, tileSize, tileSize, 1);
            bDoor___ =       Division(stageMapchip, tileSize * 2, tileSize * 0, tileSize, tileSize, 1);
            Image lDoor___ = Division(stageMapchip, tileSize * 3, tileSize * 0, tileSize, tileSize, 1);
            Image blood1__ = Division(stageMapchip, tileSize * 4, tileSize * 0, tileSize, tileSize, 1);
            Image blood2__ = Division(stageMapchip, tileSize * 5, tileSize * 0, tileSize, tileSize, 1);
            Image blood3__ = Division(stageMapchip, tileSize * 6, tileSize * 0, tileSize, tileSize, 1);
            Image blood4__ = Division(stageMapchip, tileSize * 7, tileSize * 0, tileSize, tileSize, 1);
            //床パーツ------------------------------------------------------------------------------
            //w->wooden, s-> stone, E->edge, C->crack, S->stain, w->white
            Image grass___ = Division(stageMapchip, tileSize * 0, tileSize * 1, tileSize, tileSize, 1);
            Image wFloor__ = Division(stageMapchip, tileSize * 1, tileSize * 1, tileSize, tileSize, 1);
            Image wEFloor_ = Division(stageMapchip, tileSize * 2, tileSize * 1, tileSize, tileSize, 1);
            Image wCFloor_ = Division(stageMapchip, tileSize * 3, tileSize * 1, tileSize, tileSize, 1);
            Image sFloor__ = Division(stageMapchip, tileSize * 4, tileSize * 1, tileSize, tileSize, 1);
            Image sCFloor_ = Division(stageMapchip, tileSize * 5, tileSize * 1, tileSize, tileSize, 1);
            Image sSFloor_ = Division(stageMapchip, tileSize * 6, tileSize * 1, tileSize, tileSize, 1);
            Image wTile___ = Division(stageMapchip, tileSize * 7, tileSize * 1, tileSize, tileSize, 1);
            //壁パーツ------------------------------------------------------------------------------
            //h->horizontal, v->vertical, c->corner, b->brick
            Image hFence__ = Division(stageMapchip, tileSize * 0, tileSize * 2, tileSize, tileSize, 1);
            Image vFence__ = Division(stageMapchip, tileSize * 1, tileSize * 2, tileSize, tileSize, 1);
            Image cFence__ = Division(stageMapchip, tileSize * 2, tileSize * 2, tileSize, tileSize, 1);
            Image wWall___ = Division(stageMapchip, tileSize * 3, tileSize * 2, tileSize, tileSize, 1);
            Image wCWall__ = Division(stageMapchip, tileSize * 4, tileSize * 2, tileSize, tileSize, 1);
            Image sWall___ = Division(stageMapchip, tileSize * 5, tileSize * 2, tileSize, tileSize, 1);
            Image sCWall__ = Division(stageMapchip, tileSize * 6, tileSize * 2, tileSize, tileSize, 1);
            Image bWall___ = Division(stageMapchip, tileSize * 7, tileSize * 2, tileSize, tileSize, 1);
            //障害物パーツ---------------------------------------------------------------------------
            //b->broken
            Image rock____ = Division(stageMapchip, tileSize * 0, tileSize * 3, tileSize, tileSize, 1);
            Image shelf___ = Division(stageMapchip, tileSize * 1, tileSize * 3, tileSize, tileSize, 1);
            Image bShelf__ = Division(stageMapchip, tileSize * 2, tileSize * 3, tileSize, tileSize, 1);
            Image box_____ = Division(stageMapchip, tileSize * 3, tileSize * 3, tileSize, tileSize, 1);
            Image bBox____ = Division(stageMapchip, tileSize * 4, tileSize * 3, tileSize, tileSize, 1);
            Image grate___ = Division(stageMapchip, tileSize * 5, tileSize * 3, tileSize, tileSize, 1);
            Image bGrate__ = Division(stageMapchip, tileSize * 6, tileSize * 3, tileSize, tileSize, 1);
            monument =       Division(stageMapchip, tileSize * 7, tileSize * 3, tileSize, tileSize, 1);
            //装飾品パーツ---------------------------------------------------------------------------
            Image bush____ = Division(stageMapchip, tileSize * 0, tileSize * 4, tileSize, tileSize, 1);
            Image paper___ = Division(stageMapchip, tileSize * 1, tileSize * 4, tileSize, tileSize, 1);
            Image dish____ = Division(stageMapchip, tileSize * 2, tileSize * 4, tileSize, tileSize, 1);
            Image candle__ = Division(stageMapchip, tileSize * 3, tileSize * 4, tileSize, tileSize, 1);
            Image cloth___ = Division(stageMapchip, tileSize * 4, tileSize * 4, tileSize, tileSize, 1);
            Image Web_____ = Division(stageMapchip, tileSize * 5, tileSize * 4, tileSize, tileSize, 1);
            stair___ =       Division(stageMapchip, tileSize * 6, tileSize * 4, tileSize, tileSize, 1);
            Image flower__ = Division(stageMapchip, tileSize * 7, tileSize * 4, tileSize, tileSize, 1);

            //Stage0(外)---------------------------------------------------------------------------
            //Stage0の床・壁
            SetStageBaseMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { grass___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, grass___},
                { grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___},
                { grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___},
                { grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___},
                { grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___},
                { grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___},
                { grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___},
                { grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___},
                { grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___},
                { grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___},
            });
            //Stage0の障害物・装飾
            SetStageItemMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { cFence__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, tDoor___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, cFence__},
                { vFence__, dummy___, dummy___, dummy___, bush____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, vFence__},
                { vFence__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, rock____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bush____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, rock____, dummy___, vFence__},
                { vFence__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, vFence__},
                { vFence__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bush____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, rock____, dummy___, dummy___, dummy___, dummy___, dummy___, vFence__},
                { vFence__, dummy___, rock____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, vFence__},
                { vFence__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, vFence__},
                { vFence__, dummy___, dummy___, dummy___, dummy___, bush____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bush____, dummy___, dummy___, vFence__},
                { vFence__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, rock____, dummy___, dummy___, dummy___, dummy___, bush____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, vFence__},
                { cFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, hFence__, cFence__},
            });

            //Stage1(書斎)---------------------------------------------------------------------
            //Stage1の床・壁
            SetStageBaseMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___},
                { wWall___, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wCWall__},
                { wWall___, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wWall___},
                { wWall___, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wWall___},
                { wCWall__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wWall___},
                { wWall___, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wWall___},
                { wWall___, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wWall___},
                { wWall___, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wWall___},
                { wWall___, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wWall___},
                { wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___},
            });
            //Stage1の障害物・装飾
            SetStageItemMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, tDoor___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, paper___, dummy___, dummy___, dummy___, dummy___, dummy___, blood1__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, paper___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, shelf___, shelf___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, shelf___, bShelf__, dummy___, dummy___, dummy___, dummy___, dummy___, shelf___, shelf___, bShelf__, shelf___, shelf___, shelf___, shelf___, shelf___, bShelf__, shelf___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, shelf___, shelf___, dummy___, dummy___, dummy___, dummy___, dummy___, shelf___, shelf___, shelf___, shelf___, shelf___, bShelf__, shelf___, shelf___, shelf___, shelf___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, paper___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bDoor___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
            });

            //Stage2(食堂)---------------------------------------------------------------------
            //Stage2の床・壁
            SetStageBaseMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___},
                { wWall___, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, wWall___},
                { wWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, wWall___},
                { wWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, wWall___},
                { wCWall__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, wWall___},
                { wWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, wCWall__},
                { wWall___, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, wWall___},
                { wWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, wWall___},
                { wWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, wWall___},
                { wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___},
            });
            //Stage2の障害物・装飾
            SetStageItemMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dish____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, shelf___, shelf___, bShelf__, shelf___, shelf___, shelf___, shelf___, shelf___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, shelf___, shelf___, shelf___, shelf___, shelf___, shelf___, shelf___, bShelf__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dish____, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bShelf__, shelf___, shelf___, bShelf__, shelf___, shelf___, shelf___, shelf___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, blood2__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dish____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dish____, dummy___, shelf___, bShelf__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, shelf___, shelf___, blood4__, dummy___, dummy___, dummy___, dummy___, rDoor___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bDoor___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
            });

            //Stage3(祭壇)---------------------------------------------------------------------
            //Stage3の床・壁
            SetStageBaseMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sCWall__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sWall___},
                { sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___},
            });
            //Stage3の障害物・装飾
            SetStageItemMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, candle__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, blood2__, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, box_____, box_____, box_____, box_____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, box_____, box_____, bBox____, box_____, dummy___, dummy___, dummy___, blood3__, dummy___, dummy___, dummy___, dummy___, dummy___, box_____, box_____, box_____, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, candle__, dummy___, dummy___, bBox____, box_____, box_____, box_____, dummy___, dummy___, dummy___, box_____, box_____, bBox____, box_____, dummy___, dummy___, box_____, box_____, bBox____, dummy___, dummy___, dummy___, dummy___, rDoor___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, blood1__, dummy___, dummy___, dummy___, box_____, box_____, box_____, box_____, dummy___, dummy___, box_____, box_____, box_____, candle__, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, candle__, dummy___, dummy___, bBox____, box_____, box_____, dummy___, dummy___, dummy___, dummy___, dummy___},
                { lDoor___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, box_____, box_____, box_____, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
            });

            //Stage4(寝室)---------------------------------------------------------------------
            //Stage4の床・壁
            SetStageBaseMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___},
                { wWall___, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wCFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wWall___},
                { wWall___, wFloor__, wCFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wEFloor_, wWall___},
                { wWall___, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wCWall__},
                { wWall___, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wWall___},
                { wCWall__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wFloor__, wFloor__, wCWall__},
                { wWall___, wFloor__, wEFloor_, wCFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wWall___},
                { wWall___, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wEFloor_, wCFloor_, wFloor__, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wWall___},
                { wWall___, wFloor__, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wEFloor_, wFloor__, wFloor__, wFloor__, wFloor__, wFloor__, wCFloor_, wFloor__, wEFloor_, wFloor__, wFloor__, wWall___},
                { wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wWall___, wCWall__, wWall___},
            });
            //Stage4の障害物・装飾
            SetStageItemMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, tDoor___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, box_____, box_____, box_____, bBox____, blood1__, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, blood3__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, cloth___, dummy___, dummy___, dummy___, dummy___, bBox____, box_____, box_____, box_____, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, bShelf__, shelf___, bShelf__, shelf___, dummy___, dummy___, dummy___, dummy___, dummy___, blood4__, dummy___, dummy___, box_____, box_____, box_____, box_____, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, cloth___, dummy___, shelf___, bShelf__, shelf___, shelf___, dummy___, dummy___, dummy___, shelf___, shelf___, dummy___, dummy___, dummy___, box_____, box_____, bBox____, box_____, dummy___, dummy___, dummy___, dummy___, dummy___},
                { lDoor___, dummy___, dummy___, dummy___, dummy___, shelf___, shelf___, shelf___, bShelf__, dummy___, dummy___, dummy___, shelf___, bShelf__, dummy___, dummy___, dummy___, box_____, box_____, box_____, box_____, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, shelf___, bShelf__, shelf___, shelf___, dummy___, dummy___, dummy___, dummy___, cloth___, dummy___, dummy___, dummy___, bBox____, box_____, box_____, box_____, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, shelf___, shelf___, bShelf__, shelf___, dummy___, blood2__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, cloth___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, bShelf__, shelf___, shelf___, bShelf__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
            });

            //Stage5(牢獄)---------------------------------------------------------------------
            //Stage5の床・壁
            SetStageBaseMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sSFloor_, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sSFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCWall__},
                { sCWall__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sSFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sSFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sCFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sSFloor_, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sSFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sSFloor_, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sFloor__, sWall___},
                { sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__},
            });
            //Stage5の障害物・装飾
            SetStageItemMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, tDoor___, dummy___},
                { dummy___, Web_____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, blood2__, dummy___, grate___, bGrate__, Web_____, dummy___, bGrate__, grate___, dummy___, dummy___, bGrate__, grate___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, grate___, grate___, dummy___, dummy___, grate___, bGrate__, dummy___, Web_____, grate___, grate___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, blood4__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bGrate__, grate___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, grate___, bGrate__, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, grate___, grate___, dummy___, dummy___, dummy___, dummy___, grate___, grate___, dummy___, dummy___, dummy___, dummy___, blood1__, dummy___, grate___, grate___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, grate___, bGrate__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, grate___, grate___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, grate___, grate___, dummy___, dummy___, bGrate__, grate___, dummy___, dummy___, grate___, bGrate__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, bGrate__, grate___, Web_____, dummy___, grate___, grate___, dummy___, dummy___, grate___, grate___, Web_____, dummy___, blood3__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, blood4__, dummy___},
                { dummy___, bDoor___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
            });

            //Stage6(広間)---------------------------------------------------------------------
            //Stage6の床・壁
            SetStageBaseMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___},
                { sWall___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, sWall___},
                { sWall___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, sWall___},
                { sWall___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, sWall___},
                { sCWall__, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, sCWall__},
                { sWall___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, sWall___},
                { sWall___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, sWall___},
                { sWall___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, sCWall__},
                { sWall___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, wTile___, sWall___},
                { sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sCWall__, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___, sWall___},
            });
            //Stage6の障害物・装飾
            SetStageItemMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, tDoor___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, shelf___, bShelf__, shelf___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, blood2__, stair___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bBox____, bBox____, box_____, dummy___},
                { dummy___, bShelf__, shelf___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, blood2__, dummy___, dummy___, dummy___, dummy___, dummy___, bBox____, box_____, dummy___},
                { dummy___, bShelf__, dummy___, blood1__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bBox____, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, blood4__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, bBox____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, box_____, bBox____, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, blood3__, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, bBox____, bBox____, box_____, dummy___, blood3__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bDoor___, dummy___},
            });

            //Stage7(クリア画面)---------------------------------------------------------------------
            //Stage7の床・壁
            SetStageBaseMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___},
                { bWall___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, bWall___},
                { bWall___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, bWall___},
                { bWall___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, bWall___},
                { bWall___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, bWall___},
                { bWall___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, bWall___},
                { bWall___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, bWall___},
                { bWall___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, bWall___},
                { bWall___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, grass___, bWall___},
                { bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___, bWall___},
            });
            //Stage7の障害物・装飾
            SetStageItemMap(new Image[(int)(WayPoint.stage_height / (tileSize * imageScale)), (int)(WayPoint.stage_width / (tileSize * imageScale))]
            {
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___},
                { dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, monument, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, flower__, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
                { dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, bDoor___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___, dummy___},
            });
        }
        private void SetStageBaseMap(Image[,] map)
        {
            int height = map.GetLength(0);
            int width = map.GetLength(1);
            List<ImagePos> imageObjects = new List<ImagePos>();
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    ImagePos imagePos = new ImagePos();
                    imagePos.img = map[y, x];
                    imagePos.position = new Point(x * (int)(tileSize * imageScale), y * (int)(tileSize * imageScale));
                    imageObjects.Add(imagePos);
                }
            }
            stageBaseMaps.Add(imageObjects);
        }
        private void SetStageItemMap(Image[,] map)
        {
            int stageNum = stageItemMaps.Count();
            int height = map.GetLength(0);
            int width = map.GetLength(1);
            List<ImagePos> imageObjects = new List<ImagePos>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ImagePos imagePos = new ImagePos();
                    imagePos.img = map[y, x];
                    imagePos.position = new Point(x * (int)(tileSize * imageScale), y * (int)(tileSize * imageScale));
                    //中央配置はオフセットをかける
                    if(imagePos.img != null)
                    {
                        switch (stageNum)
                        {
                            case 0:
                                if (imagePos.img == tDoor___) imagePos.position.X += (int)(tileSize * imageScale / 2.0f);
                                break;
                            case 1:
                                if (imagePos.img == tDoor___ || imagePos.img == bDoor___) imagePos.position.X += (int)(tileSize * imageScale / 2.0f);
                                break;
                            case 2:
                                if (imagePos.img == bDoor___) imagePos.position.X += (int)(tileSize * imageScale / 2.0f);
                                break;
                            case 6:
                                if (imagePos.img == tDoor___ || imagePos.img == stair___) imagePos.position.X += (int)(tileSize * imageScale / 2.0f);
                                if(imagePos.img == stair___) imagePos.position.Y += (int)(tileSize * imageScale / 2.0f);
                                break;
                            case 7:
                                if (imagePos.img == bDoor___ || imagePos.img == monument) imagePos.position.X += (int)(tileSize * imageScale / 2.0f);
                                break;
                        }
                    }
                    imageObjects.Add(imagePos);
                }
            }
            stageItemMaps.Add(imageObjects);
        }
    }
}
