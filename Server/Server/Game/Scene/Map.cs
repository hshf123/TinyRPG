using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Game
{
    public struct Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;
    }

    public struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }

    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y) { this.x = x; this.y = y; }

        public static Vector2Int up { get { return new Vector2Int(0, 1); } }
        public static Vector2Int right { get { return new Vector2Int(1, 0); } }
        public static Vector2Int down { get { return new Vector2Int(0, -1); } }
        public static Vector2Int left { get { return new Vector2Int(-1, 0); } }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b) { return new Vector2Int(a.x + b.x, a.y + b.y); }
    }

    public class Map
    {
        // 좌표로 어디로 연결된 포탈인지 찾아주기 위한 Dictionary였던 것
        public Dictionary<Vector2Int, string> PortalPos { get; private set; } = new Dictionary<Vector2Int, string>();

        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }

        public int SizeX { get { return MaxX - MinX + 1; } }
        public int SizeY { get { return MaxY - MinY + 1; } }

        bool[,] _collision;
        GameObject[,] _objects;

        // 갈 수 있는 영역인지 체크
        public bool CanGo(Vector2Int cellPos, bool checkObjects = true)
        {
            // 최대 범위 체크
            if (cellPos.x < MinX || cellPos.x > MaxX)
                return false;
            if (cellPos.y < MinY || cellPos.y > MaxY)
                return false;

            // 충돌 범위 체크
            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y;
            // collision 배열이 true면 갈 수 없는거고 false면 갈 수 있는 곳
            // checkObject가 false면 플레이어도 체크해야한다.
            return !_collision[y, x] && (!checkObjects || _objects[y, x] == null);
        }

        public GameObject Find(Vector2Int pos)
        {
            if (pos.x < MinX || pos.x > MaxX)
                return null;
            if (pos.y < MinY || pos.y > MaxY)
                return null;

            int x = pos.x - MinX;
            int y = MaxY - pos.y;
            return _objects[y, x];
        }

        public bool ApplyMove(GameObject go, Vector2Int destPos)
        {
            ApplyLeave(go);
            if (CanGo(destPos) == false)
                return false;

            { // 이동하고자 하는 위치에 플레이어를 위치시켜준다.
                int x = destPos.x - MinX;
                int y = MaxY - destPos.y;
                _objects[y, x] = go;
            }

            // 실제 좌표 이동
            go.PosInfo.PosX = destPos.x;
            go.PosInfo.PosY = destPos.y;
            return true;
        }

        public bool ApplyLeave(GameObject go)
        {
            PositionInfo posInfo = go.Info.PosInfo;
            if (posInfo.PosX < MinX || posInfo.PosX > MaxX)
                return false;
            if (posInfo.PosY < MinY || posInfo.PosY > MaxY)
                return false;

            { // 원래 있던 좌표를 지워주고
                int x = posInfo.PosX - MinX;
                int y = MaxY - posInfo.PosY;
                if (_objects[y, x] == go)
                    _objects[y, x] = null;
            }

            return true;
        }

        // 포탈인지 체크
        public bool IsPortal(Vector2Int cellPos)
        {
            // 포탈인지 아닌지
            int x = cellPos.x - MinX;
            int y = MaxY - cellPos.y;
            return PortalPos.ContainsKey(new Vector2Int(x, y)); // portal 배열이 true면 포탈 false면 길
        }

        public void LoadMap(string name, string path = "../../../../../Common/MapData")
        {

            // Collision 관련 파일
            string collision = name + "_Collision";
            string collisionText = File.ReadAllText($"{path}/{collision}.txt");
            StringReader reader = new StringReader(collisionText);

            MinX = int.Parse(reader.ReadLine());
            MaxX = int.Parse(reader.ReadLine());
            MinY = int.Parse(reader.ReadLine());
            MaxY = int.Parse(reader.ReadLine());
            int xCount = MaxX - MinX + 1;
            int yCount = MaxY - MinY + 1;
            _collision = new bool[yCount, xCount];
            _objects = new GameObject[yCount, xCount];

            for (int y = 0; y < yCount; y++)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < xCount; x++)
                {
                    _collision[y, x] = (line[x] == '1' ? true : false);
                }
            }

            //Portal 관련 파일
            string portal = name + "_Portal";
            string portalText = File.ReadAllText($"{path}/{portal}.txt");

            reader = new StringReader(portalText);

            int count = int.Parse(reader.ReadLine());
            for (int j = 0; j < count; j++)
            {
                string linkName = reader.ReadLine(); // 어디로 연결 되어 있는지
                int x = int.Parse(reader.ReadLine());
                int y = int.Parse(reader.ReadLine());
                PortalPos.Add(new Vector2Int(x, y), linkName);
            }
        }

        #region A*

        int[] _deltaY = new int[] { 1, -1, 0, 0 };
        int[] _deltaX = new int[] { 0, 0, -1, 1 };
        int[] _cost = new int[] { 10, 10, 10, 10 };

        public List<Vector2Int> FindPath(Vector2Int startCellPos, Vector2Int destCellPos, bool ignoreDestCollision = false/*충돌을 무시할지*/)
        {
            List<Pos> path = new List<Pos>();

            // 점수 매기기
            // F = G + H
            // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
            // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
            // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

            // (y, x) 이미 방문했는지 여부 (방문을 했으면 true)
            bool[,] closed = new bool[SizeY, SizeX];

            // (y, x) 가는 길을 한 번이라도 발견을 했는지
            // 발견 X => MaxValue
            // 발견 O => F = G + H
            int[,] open = new int[SizeY, SizeX];
            for (int y = 0; y < SizeY; y++)
                for (int x = 0; x < SizeX; x++)
                    open[y, x] = Int32.MaxValue;

            Pos[,] parent = new Pos[SizeY, SizeX];

            // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위해 PriorityQueue를 사용한다.
            PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

            // CellPos -> ArrayPos;
            Pos pos = Cell2Pos(startCellPos);
            Pos dest = Cell2Pos(destCellPos);

            // 시작점 발견
            open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
            pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
            parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

            while (pq.Count > 0)
            {
                // 제일 좋은 후보를 찾는다
                PQNode node = pq.Pop();
                // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문된 경우 스킵
                if (closed[node.Y, node.X])
                    continue;

                // 방문한다
                closed[node.Y, node.X] = true;
                // 목적지에 도착했으면 바로 종료
                if (node.Y == dest.Y && node.X == dest.X)
                    break;

                // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
                for (int i = 0; i < _deltaY.Length; i++)
                {
                    Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                    // 유효 범위를 벗어났으면 스킵
                    // 벽으로 막혀서 갈 수 없으면 스킵
                    if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                    {
                        if (CanGo(Pos2Cell(next)) == false) // CellPos
                            continue;
                    }

                    // 이미 방문한 곳이면 스킵
                    if (closed[next.Y, next.X])
                        continue;

                    // 비용 계산
                    int g = 0;// node.G + _cost[i];
                    int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                    // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
                    if (open[next.Y, next.X] < g + h)
                        continue;

                    // 예약 진행
                    open[dest.Y, dest.X] = g + h;
                    pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
                    parent[next.Y, next.X] = new Pos(node.Y, node.X);
                }
            }

            return CalcCellPathFromParent(parent, dest);
        }

        List<Vector2Int> CalcCellPathFromParent(Pos[,] parent, Pos dest)
        {
            List<Vector2Int> cells = new List<Vector2Int>();

            int y = dest.Y;
            int x = dest.X;
            while (parent[y, x].Y != y || parent[y, x].X != x)
            {
                cells.Add(Pos2Cell(new Pos(y, x)));
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            cells.Add(Pos2Cell(new Pos(y, x)));
            cells.Reverse();

            return cells;
        }

        Pos Cell2Pos(Vector2Int cell)
        {
            return new Pos(MaxY - cell.y, cell.x - MinX);
        }

        Vector2Int Pos2Cell(Pos pos)
        {
            return new Vector2Int(pos.X + MinX, MaxY - pos.Y);
        }
        #endregion
    }
}
