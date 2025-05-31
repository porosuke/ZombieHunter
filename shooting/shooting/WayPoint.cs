using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace shooting
{
    public class WayPoint
    {
        //経路探索関連
        public const int Wall = 50;
        private const int zombie_half = 24;
        private const int point_ex = 5;
        public const int stage_width = 1300;
        public const int stage_height = 500;
        private const int max_distance = int.MaxValue;
        private const int min_distance = (point_ex - 1) * (point_ex - 1);

        //全ステージの点を格納するリスト
        public List<List<Point>> All_Points = new List<List<Point>>();
        //全障害物を格納するリスト
        public List<Rectangle[]> All_Rect = new List<Rectangle[]>();
        public List<Rectangle[]> All_Rect_ex = new List<Rectangle[]>();
        //次の向かう先を格納するリスト
        public List<int[,]> All_NextTable = new List<int[,]>();
        //ノード間の距離を格納するリスト
        public List<int[,]> All_DistTable = new List<int[,]>();

        //初期化------------------------------------------------------------------------------------

        public WayPoint()
        {
            //使用するサイズを入力
            Stopwatch sw = new Stopwatch();
            sw.Start();

            AddObject();
            MakeTable();
            SearchPoint();

            sw.Stop();
            Console.WriteLine($"総処理時間：{sw.Elapsed.ToString()}");
        }

        //最短経路構築-------------------------------------------------------------------------------

        private void AddObject()
        {
            Rectangle[] rect0 =
            {
                new Rectangle(0,0,stage_width,Wall),
                new Rectangle(0,0,Wall,stage_height),
                new Rectangle(0,stage_height - Wall,stage_width,Wall),
                new Rectangle(stage_width - Wall,0,Wall,stage_height),
                new Rectangle(100,250,50,50),
                new Rectangle(400,100,50,50),
                new Rectangle(500,400,50,50),
                new Rectangle(950,200,50,50),
                new Rectangle(1150,100,50,50),
            };
            Rectangle[] rect1 =
            {
                new Rectangle(0,0,stage_width,Wall),
                new Rectangle(0,0,Wall,stage_height),
                new Rectangle(0,stage_height - Wall,stage_width,Wall),
                new Rectangle(stage_width - Wall,0,Wall,stage_height),
                new Rectangle(200,150,100,150),
                new Rectangle(550,200,500,100),

            };
            Rectangle[] rect2 =
            {
                new Rectangle(0,0,stage_width,Wall),
                new Rectangle(0,0,Wall,stage_height),
                new Rectangle(0,stage_height - Wall,stage_width,Wall),
                new Rectangle(stage_width - Wall,0,Wall,stage_height),
                new Rectangle(300,150,400,150),
                new Rectangle(900,350,100,100),

            };
            Rectangle[] rect3 =
            {
                new Rectangle(0,0,stage_width,Wall),
                new Rectangle(0,0,Wall,stage_height),
                new Rectangle(0,stage_height - Wall,stage_width,Wall),
                new Rectangle(stage_width - Wall,0,Wall,stage_height),
                new Rectangle(250,150,200,150),
                new Rectangle(600,250,200,100),
                new Rectangle(900,200,150,250),

            };
            Rectangle[] rect4 =
            {
                new Rectangle(0,0,stage_width,Wall),
                new Rectangle(0,0,Wall,stage_height),
                new Rectangle(0,stage_height - Wall,stage_width,Wall),
                new Rectangle(stage_width - Wall,0,Wall,stage_height),
                new Rectangle(250,150,200,300),
                new Rectangle(600,200,100,100),
                new Rectangle(850,50,200,300),
            };
            Rectangle[] rect5 =
            {
                new Rectangle(0,0,stage_width,Wall),
                new Rectangle(0,0,Wall,stage_height),
                new Rectangle(0,stage_height - Wall,stage_width,Wall),
                new Rectangle(stage_width - Wall,0,Wall,stage_height),
                new Rectangle(150,350,100,100),
                new Rectangle(350,200,100,250),
                new Rectangle(550,350,100,100),
                new Rectangle(650,50,100,200),
                new Rectangle(850,50,100,100),
                new Rectangle(1050,50,100,200),
            };
            Rectangle[] rect6 =
            {
                new Rectangle(0,0,stage_width,Wall),
                new Rectangle(0,0,Wall,stage_height),
                new Rectangle(0,stage_height - Wall,stage_width,Wall),
                new Rectangle(stage_width - Wall,0,Wall,stage_height),
                new Rectangle(50,50,150,50),
                new Rectangle(1100,50,150,50),
                new Rectangle(50,400,150,50),

                new Rectangle(50,100,100,50),
                new Rectangle(1150,100,100,50),
                new Rectangle(50,350,100,50),

                new Rectangle(50,150,50,50),
                new Rectangle(50,300,50,50),
                new Rectangle(1200,150,50,50),
            };
            Rectangle[] rect7 =
            {
                new Rectangle(0,0,stage_width,Wall),
                new Rectangle(0,0,Wall,stage_height),
                new Rectangle(0,stage_height - Wall,stage_width,Wall),
                new Rectangle(stage_width - Wall,0,Wall,stage_height),
                new Rectangle(625, 100, 50, 50),
            };
            //作成したらここに追加
            All_Rect.Add(rect0);
            All_Rect.Add(rect1);
            All_Rect.Add(rect2);
            All_Rect.Add(rect3);
            All_Rect.Add(rect4);
            All_Rect.Add(rect5);
            All_Rect.Add(rect6);
            All_Rect.Add(rect7);

            //Allrect内のすべてを拡張する(expansion)敵の半径を指定
            Rect_ex(zombie_half, rect0);
            Rect_ex(zombie_half, rect1);
            Rect_ex(zombie_half, rect2);
            Rect_ex(zombie_half, rect3);
            Rect_ex(zombie_half, rect4);
            Rect_ex(zombie_half, rect5);
            Rect_ex(zombie_half, rect6);
            Rect_ex(zombie_half, rect7);
        }
        private void Rect_ex(int Enemy_r, Rectangle[] rect)
        {
            //敵の半径だけ拡張した障害物データを作成し、All_Rect_exに追加
            Rectangle[] rect_ex = new Rectangle[rect.Length];
            for (int i = 0; i < rect.Length; i++)
            {
                rect_ex[i] = new Rectangle(rect[i].X - Enemy_r, rect[i].Y - Enemy_r, rect[i].Width + Enemy_r * 2, rect[i].Height + Enemy_r * 2);
            }
            All_Rect_ex.Add(rect_ex);
        }
        private void MakeTable()
        {
            //ステージ数の数だけ繰り返す
            for (int i = 0; i < All_Rect_ex.Count; i++)
            {
                List<Point> points = new List<Point>();
                //外壁しかないならスキップ
                if (All_Rect_ex[i].Length > 4)
                {
                    //そのステージの障害物の数だけ繰り返す
                    for (int j = 0; j < All_Rect_ex[i].Length; j++)
                    {
                        if (j > 3)
                        {
                            //矩形の各頂点について繰り返す
                            for (int k = 0; k < 4; k++)
                            {
                                Point point = new Point();
                                switch (k)
                                {
                                    //左上
                                    case 0:
                                        point = new Point(All_Rect_ex[i][j].Left - point_ex, All_Rect_ex[i][j].Top - point_ex);
                                        break;
                                    //右上
                                    case 1:
                                        point = new Point(All_Rect_ex[i][j].Right + point_ex, All_Rect_ex[i][j].Top - point_ex);
                                        break;
                                    //左下
                                    case 2:
                                        point = new Point(All_Rect_ex[i][j].Left - point_ex, All_Rect_ex[i][j].Bottom + point_ex);
                                        break;
                                    //右下
                                    case 3:
                                        point = new Point(All_Rect_ex[i][j].Right + point_ex, All_Rect_ex[i][j].Bottom + point_ex);
                                        break;
                                }

                                //ステージ内の他の障害物に完全に含まれていないかチェックする
                                //もし障害物にめり込んでいなかったらカウントを増やす（1つでもめり込めばアウト→breakで抜ける）
                                int count = 0;
                                foreach (Rectangle rect in All_Rect[i])
                                {
                                    if ((((rect.X <= point.X) && (rect.Y <= point.Y)) && ((rect.Right >= point.X) && (rect.Bottom >= point.Y)))) break; else count++;
                                }
                                //もしすべての障害物をくぐり抜けたら点を作成する
                                if (count == All_Rect[i].Length) points.Add(point);
                            }
                        }
                    }
                }
                All_Points.Add(points);
                Console.WriteLine($"stage{i} make PointTable completed");
                Console.WriteLine($"stage{i} has {All_Points[i].Count} points");
            }
        }
        private void SearchPoint()
        {
            int[,] NextTable;
            int[,] DistTable;
            int tableSize;
            //ステージの数だけ繰り返す
            for (int stageIndex = 0; stageIndex < All_Rect.Count; stageIndex++)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine($"Start create stage{stageIndex} table");

                //初期化
                tableSize = All_Points[stageIndex].Count;
                NextTable = new int[tableSize, tableSize];
                DistTable = new int[tableSize, tableSize];

                Console.WriteLine($"Start create DistTable");
                //初期化
                for (int startIndex = 0; startIndex < tableSize; startIndex++)
                {
                    for (int endIndex = 0; endIndex < tableSize; endIndex++) DistTable[startIndex, endIndex] = -1;
                }
                //重みつきテーブルを作成
                for (int startIndex = 0; startIndex < tableSize; startIndex++)
                {
                    for (int endIndex = 0; endIndex < tableSize; endIndex++)
                    {
                        //まだ未決定なら計算
                        if (DistTable[startIndex, endIndex] < 0)
                        {
                            Point startPoint = All_Points[stageIndex][startIndex];
                            Point endPoint = All_Points[stageIndex][endIndex];
                            //start-endが同じならコスト0
                            if (startIndex == endIndex)
                            {
                                DistTable[startIndex, endIndex] = 0;
                            }
                            //衝突するならコスト無限
                            else if (CheckCollisionRect(All_Rect_ex[stageIndex], startPoint, endPoint))
                            {
                                DistTable[startIndex, endIndex] = int.MaxValue;
                            }
                            //衝突しないなら距離をはかる
                            else
                            {
                                DistTable[startIndex, endIndex] = DistanceSquared(startPoint, endPoint);
                            }

                            //反転した側（end-start）にも適用できる
                            DistTable[endIndex, startIndex] = DistTable[startIndex, endIndex];
                        }
                    }
                }
                //整合性確認
                if (DistTableErrorCheck(DistTable))
                {
                    Console.WriteLine("Create DistTable ERROR!!");
                    MessageBox.Show("Creation of distance weighted table failed. Check for initialisation errors.\n" +
                        "距離重み付テーブルの作成に失敗しました。初期化に誤りがないか確認してください。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }

                //次の一手テーブルを初期化
                for (int startIndex = 0; startIndex < tableSize; startIndex++)
                {
                    for (int endIndex = 0; endIndex < tableSize; endIndex++)
                    {
                        if (startIndex == endIndex || DistTable[startIndex, endIndex] == int.MaxValue)
                        {
                            // 自身 or 到達不可
                            NextTable[startIndex, endIndex] = -1;
                        }
                        else
                        {
                            // 直通なので「次の一手」は end
                            NextTable[startIndex, endIndex] = endIndex;
                        }
                    }
                }

                Console.WriteLine($"Start create NextTable");
                //ワーシャル-フロイド法により、次の一手テーブルと、特定点間の最短総距離を求める
                for (int relay = 0; relay < tableSize; relay++)
                {
                    for (int start = 0; start < tableSize; start++)
                    {
                        if (DistTable[start, relay] == int.MaxValue || NextTable[start, relay] == -1)
                            continue;

                        for (int end = 0; end < tableSize; end++)
                        {
                            if (DistTable[relay, end] == int.MaxValue || NextTable[relay, end] == -1)
                                continue;

                            int newDist = DistTable[start, relay] + DistTable[relay, end];

                            if (newDist < DistTable[start, end])
                            {
                                DistTable[start, end] = newDist;
                                NextTable[start, end] = NextTable[start, relay]; // 経由点へ向かう第一歩
                            }
                        }
                    }
                }

                //整合性確認
                if (NextTableErrorCheck(NextTable))
                {
                    Console.WriteLine("Create NextTable ERROR!!");
                    MessageBox.Show("Creation of the next target table failed. Check for unreachable locations.\n" +
                        "次目標テーブルの作成に失敗しました。到達不可能な場所がないか確認してください。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
                else
                {
                    All_DistTable.Add(DistTable);
                    All_NextTable.Add(NextTable);
                }
            }
        }
        private bool DistTableErrorCheck(int[,] table)
        {
            bool isError = false;
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (table[i, j] == -1)
                    {
                        isError = true;
                        Console.WriteLine($"Error distance from Node {i} to Node {j}");
                    }
                }
            }
            return isError;
        }
        private bool NextTableErrorCheck(int[,] table)
        {
            bool isError = false;
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (i != j && table[i, j] == -1)
                    {
                        isError = true;
                        Console.WriteLine($"No path from Node {i} to Node {j}");
                    }
                }
            }
            return isError;
        }
        public bool CheckCollisionRect(Rectangle[] rectangle, PointF c, PointF p)
        {
            Rectangle[] rect = rectangle;

            //対象ステージの障害物を走査する（ステージ壁は無視）
            for (int i = 4; i < rect.Length; i++)
            {
                if (CheckCollision(c, p, rect[i])) return true;
            }
            //全てに当たらなかったなら
            return false;
        }
        public bool CheckCollision(PointF Start, PointF End, RectangleF rect)
        {
            //まず衝突可能性をチェック
            float minX = Math.Min(Start.X, End.X);
            float maxX = Math.Max(Start.X, End.X);
            float minY = Math.Min(Start.Y, End.Y);
            float maxY = Math.Max(Start.Y, End.Y);
            //Start->Endの線分を囲むバウンディングボックスを作成
            RectangleF linebound = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            //BBと障害物がぶつかる可能性があるか？
            if (linebound.IntersectsWith(rect))
            {
                //スラブ法でチェック（衝突したらtrueを返して終了）
                PointF boxMin = rect.Location; // 左上
                PointF boxMax = new PointF(rect.Right, rect.Bottom); // 右下
                return (LineIntersectsAABB(Start, End, boxMin, boxMax));
            }
            return false;
        }
        public bool LineIntersectsAABB(PointF a, PointF b, PointF boxMin, PointF boxMax)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;

            float tMin = 0.0f;
            float tMax = 1.0f;

            // X軸方向のスラブ判定
            if (dx != 0)
            {
                float t1 = (boxMin.X - a.X) / dx;
                float t2 = (boxMax.X - a.X) / dx;
                if (t1 > t2) (t1, t2) = (t2, t1);

                tMin = Math.Max(tMin, t1);
                tMax = Math.Min(tMax, t2);
                if (tMin > tMax) return false;
            }
            else if (a.X < boxMin.X || a.X > boxMax.X)
            {
                return false; // 線分は矩形のX範囲外で平行
            }

            // Y軸方向のスラブ判定
            if (dy != 0)
            {
                float t1 = (boxMin.Y - a.Y) / dy;
                float t2 = (boxMax.Y - a.Y) / dy;
                if (t1 > t2) (t1, t2) = (t2, t1);

                tMin = Math.Max(tMin, t1);
                tMax = Math.Min(tMax, t2);
                if (tMin > tMax) return false;
            }
            else if (a.Y < boxMin.Y || a.Y > boxMax.Y)
            {
                return false; // 線分は矩形のY範囲外で平行
            }

            return true;
        }
        public int DistanceSquared(Point start, Point end)
        {
            //平方根を考慮しない距離を返す
            return ((start.X - end.X) * (start.X - end.X)) + ((start.Y - end.Y) * (start.Y - end.Y));
        }
        public List<Point> Route(int now_stage, Point Start, Point End)
        {
            List<Point> shortest_route = new List<Point>();
            //まっすぐ行けるかチェック
            if (!CheckCollisionRect(All_Rect_ex[now_stage], Start, End))
            {
                shortest_route.Add(Start);
                shortest_route.Add(End);
            }
            else
            {
                List<int> startCandidatesIndex = new List<int>();
                List<int> startCandidatesDistance = new List<int>();
                List<int> endCandidatesIndex = new List<int>();
                List<int> endCandidatesDistance = new List<int>();

                //Start/End（非ノード）から直接行けるノードを求める
                for (int i = 0; i < All_Points[now_stage].Count; i++)
                {
                    //Startから直接行ける
                    if (!CheckCollisionRect(All_Rect_ex[now_stage], Start, All_Points[now_stage][i]))
                    {
                        int dist = DistanceSquared(Start, All_Points[now_stage][i]);
                        if (dist >= min_distance)
                        {
                            startCandidatesIndex.Add(i);
                            startCandidatesDistance.Add(dist);
                        }
                    }
                    //Endから直接行ける
                    if (!CheckCollisionRect(All_Rect_ex[now_stage], End, All_Points[now_stage][i]))
                    {
                        endCandidatesIndex.Add(i);
                        endCandidatesDistance.Add(DistanceSquared(End, All_Points[now_stage][i]));
                    }
                }

                int minDistance = max_distance;
                int startNodeIndex = 0;
                int endNodeIndex = 0;
                //総当たりで最短となるノードのインデックスを計算
                for (int startIndex = 0; startIndex < startCandidatesIndex.Count; startIndex++)
                {
                    for (int endIndex = 0; endIndex < endCandidatesIndex.Count; endIndex++)
                    {
                        int totalDistance = startCandidatesDistance[startIndex] +
                            All_DistTable[now_stage][startCandidatesIndex[startIndex], endCandidatesIndex[endIndex]] +
                            endCandidatesDistance[endIndex];
                        if (totalDistance < minDistance)
                        {
                            minDistance = totalDistance;
                            startNodeIndex = startCandidatesIndex[startIndex];
                            endNodeIndex = endCandidatesIndex[endIndex];
                        }
                    }
                }

                //この時点で、次に向かうべき場所は All_Points[now_stage][startNodeIndex] に決まる
                //これはshortest_route[1]に等しい

                //経路を辿る
                shortest_route.Add(Start);
                while (startNodeIndex != endNodeIndex)
                {
                    shortest_route.Add(All_Points[now_stage][startNodeIndex]);
                    startNodeIndex = All_NextTable[now_stage][startNodeIndex, endNodeIndex];

                    if (startNodeIndex == -1 || endNodeIndex == -1)
                    {
                        Console.WriteLine("Error: Start or End cannot reach any node.");
                    }
                }
                shortest_route.Add(All_Points[now_stage][endNodeIndex]);
                shortest_route.Add(End);
            }
            return shortest_route;
        }
    }
}
