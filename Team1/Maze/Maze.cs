using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mazes.Core;

namespace Mazes.Core
{
    public class Maze : IMaze, IBuildableMaze, IMouse, IDisposable
    {
        private bool[,] HWalls;
        private bool[,] VWalls;
        private Position[] visitedPositions { get; set; }
        private int x;
        private int y;
        private int Width;
        private int Height;
        private Position startPosition;
        private Direction direction;
        private Position[] Moves = new Position[]
                                       {
                                           new Position(0,-1),
                                           new Position(1, 0),
                                           new Position(0, 1),
                                           new Position(-1,0)
                                       };
        private Position[] WallToCheck = new Position[]
                                       {
                                           new Position(0, 0),
                                           new Position(1, 0),
                                           new Position(0, 1),
                                           new Position(0, 0)
                                       };

        public Maze(IMazeBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            Width = builder.Width;
            Height = builder.Height;
            HWalls = new bool[Width, Height + 1];
            VWalls = new bool[Width + 1, Height];
            ClearMaze();
            builder.Build(this);
            MazeHasBeenBuilt(Width, Height);
            startPosition = builder.MazeStartPosition;
            x = startPosition.X;
            y = startPosition.Y;
            direction = Direction.East;
            MouseHasMoved(new Position(x, y));
            MouseHasTurned(direction);
            ShouldBeReset = true;
        }

        private void ClearMaze()
        {
            for (var w = 0; w <= Width; w++)
                for (var h = 0; h < Height; h++)
                    VWalls[w, h] = false;
            for (var w = 0; w < Width; w++)
                for (var h = 0; h <= Height; h++)
                    HWalls[w, h] = false;
        }

        public void Draw(IMazeDrawer drawer)
        {
            if (drawer == null)
                throw new ArgumentNullException("drawer");
            drawer.StartLayout(Width, Height);
            for (var w = 0; w < Width; w++)
                for (var h = 0; h <= Height; h++)
                    if (HWalls[w, h])
                        drawer.DrawWall(new Position(w, h), new Position(w + 1, h));
            for (var w = 0; w <= Width; w++)
                for (var h = 0; h < Height; h++)
                    if (VWalls[w, h])
                        drawer.DrawWall(new Position(w, h), new Position(w, h + 1));
            drawer.LayoutDone();
        }

        #region Implementation of IMaze

        public bool CanIMove()
        {
            switch (direction)
            {
                case Direction.North:
                case Direction.South:
                    return !HWalls[x+WallToCheck[(int)direction].X, y+WallToCheck[(int)direction].Y];
                case Direction.East:
                case Direction.West:
                    return !VWalls[x + WallToCheck[(int)direction].X, y + WallToCheck[(int)direction].Y];
                default:
                    break;
            }
            return false;
        }

        public bool AmIOut()
        {
            return x >= Width || y >= Height;
        }

        public bool ShouldBeReset { get; set; }

        public int NbTurnsAndMoves => NbMoves + NbTurns;

        public int NbTurns { get; set; }
        public int NbMoves { get; set; }

        #endregion

        #region Implementation of IBuildableMaze

        public void AddHorizontalWall(int width, int height)
        {
            if (width < 0 || height < 0 || width >= Width || height > Height)
                throw new IndexOutOfRangeException("Wall position is outside maze");
            HWalls[width, height] = true;
        }

        public void AddVerticalWall(int width, int height)
        {
            if (width < 0 || height < 0 || width > Width || height >= Height)
                throw new IndexOutOfRangeException("Wall position is outside maze");
            VWalls[width, height] = true;
        }

        #endregion

        #region Implementation of IMouse

        public void TurnLeft()
        {
            switch (direction)
            {
                case Direction.North:
                    direction = Direction.West;
                    break;
                case Direction.East:
                    direction = Direction.North;
                    break;
                case Direction.South:
                    direction = Direction.East;
                    break;
                case Direction.West:
                    direction = Direction.South;
                    break;
                default:
                    break;
            }
            MouseHasTurned(direction);
        }

        public void TurnRight()
        {
            switch (direction)
            {
                case Direction.North:
                    direction = Direction.East;
                    break;
                case Direction.East:
                    direction = Direction.South;
                    break;
                case Direction.South:
                    direction = Direction.West;
                    break;
                case Direction.West:
                    direction = Direction.North;
                    break;
                default:
                    break;
            }
            MouseHasTurned(direction);
        }

        public void Move()
        {
            if (!CanIMove())
                return;

            var next = Moves[(int)direction];
            x += next.X;
            y += next.Y;

            MouseHasMoved(new Position(x, y));

            if (AmIOut())
                MouseHasExitedMaze();
        }
        #endregion

        #region Watchers

        private List<IMazeWatcher> watchers = new List<IMazeWatcher>();

        public void Subscribe(IMazeWatcher watcher)
        {
            if (watcher == null)
                throw new ArgumentNullException("watcher");
            lock (watchers)
            {
                if (watchers.Contains(watcher))
                    return;
                watchers.Add(watcher);
            }
        }

        public void Unsubscribe(IMazeWatcher watcher)
        {
            if (watcher == null)
                throw new ArgumentNullException("watcher");
            lock (watchers)
            {
                if (!watchers.Contains(watcher))
                    return;
                watchers.Remove(watcher);
                watcher.YouHaveBeenUnsubscribed();
            }
        }

        private void MouseWantsToMove()
        {
            lock (watchers)
            {
                foreach (var mazeWatcher in watchers)
                {
                    mazeWatcher.MouseWantsToMove();
                }
            }
        }

        private void MouseHasMoved(Position newPosition)
        {
            lock (watchers)
            {
                foreach (var mazeWatcher in watchers)
                {
                    mazeWatcher.MouseHasMoved(newPosition);
                }
            }
            NbMoves++;
        }

        private void MouseHasTurned(Direction newDirection)
        {
            lock (watchers)
            {
                foreach (var mazeWatcher in watchers)
                {
                    mazeWatcher.MouseHasTurned(newDirection);
                }
            }
            NbTurns++;
        }
        private void MouseHasExitedMaze()
        {
            lock (watchers)
            {
                foreach (var mazeWatcher in watchers)
                {
                    mazeWatcher.MouseHasExitedMaze();
                }
                if (ShouldBeReset)
                    Reset();
            }
        }

        public void Reset()
        {
            x = startPosition.X;
            y = startPosition.Y;
            direction = Direction.East;
        }

        private void MazeHasBeenBuilt(int width, int height)
        {
            lock (watchers)
            {
                foreach (var mazeWatcher in watchers)
                {
                    mazeWatcher.MazeHasBeenBuilt(width, height);
                }
            }
        }
        #endregion

        public void Dispose()
        {
            for (var i = watchers.Count - 1; i >= 0; i--)
            {
                var watcher = watchers[i];
                watchers.RemoveAt(i);
                watcher.YouHaveBeenUnsubscribed();
            }
        }
    }
}
