using System;
using Mazes.Core;
using System.Collections.Generic;

namespace SampleMazeSolver
{
    public class MazeSolver : IMazeSolver, IDisposable
    {
        private IMaze maze { get; set; }
        private IMouse mouse { get; set; }
        //private Node root { get; set; }
        public bool TurnedLeft { get; set; }
        public void Init(IMaze maze, IMouse mouse)
        {
            this.mouse = mouse;
            this.maze = maze;
            //root = new Node(null);
        }

        public void YourTurn()
        {
            mouse.TurnRight();
            if (maze.CanIMove())
            {
                mouse.Move();
            }
            else
            {
                mouse.TurnLeft();
                if (maze.CanIMove())
                    mouse.Move();
                else
                {
                    mouse.TurnLeft();
                    mouse.Move();
                }
            }
            
        }

        public void YouWin()
        {
            throw new NotImplementedException();
        }

        public void YouLoose()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public class Node
        {
            public Node Parent { get; set; }
            public List<Node> Children { get; set; }
            public Node(Node parent)
            {
                Parent = parent;

            }
        }
    }
}
