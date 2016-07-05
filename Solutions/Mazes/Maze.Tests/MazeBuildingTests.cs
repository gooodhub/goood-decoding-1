using Mazes.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleMazeBuilder;

namespace Mazes.Tests
{
    [TestClass]
    public class MazeBuildingTests
    {
        private Maze impl;
        private IMaze Maze;
        private IMouse Mouse;
        private LogWatcher watcher;

        [TestInitialize]
        public void Init()
        {            
            SimpleMazeBuilder builder = new SimpleMazeBuilder();
            Maze impl = new Maze(builder);
            watcher = new LogWatcher();
            impl.Subscribe(watcher);
            Maze = impl as IMaze;
            Mouse = impl as IMouse;
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void AtTheStartICanMove()
        {
            Assert.IsTrue(Maze.CanIMove());
        }

        [TestMethod]
        public void IfITurnLeftICanNotMove()
        {
            Mouse.TurnLeft();
            Assert.IsFalse(Maze.CanIMove());
        }

        [TestMethod]
        public void IfITurnRightICanMove()
        {
            Mouse.TurnRight();
            Assert.IsTrue(Maze.CanIMove());
        }
        
        [TestMethod]
        public void IfITurnRightAndRightICanNotMove()
        {
            Mouse.TurnRight();
            Mouse.TurnRight();
            Assert.IsFalse(Maze.CanIMove());
        }

        [TestMethod]
        public void IfITurnLeftAndLeftICanNotMove()
        {
            Mouse.TurnRight();
            Mouse.TurnRight();
            Assert.IsFalse(Maze.CanIMove());
        }

        [TestMethod]
        public void IfITurnLeftAndLeftAndLeftICanMove()
        {
            Mouse.TurnLeft();
            Mouse.TurnLeft();
            Mouse.TurnLeft();
            Assert.IsTrue(Maze.CanIMove());
        }

        [TestMethod]
        public void IfITakeTheSimplestRouteIAmOut()
        {
            Maze.ShouldBeReset = false;
            Mouse.TurnRight();
            Mouse.Move();
            Mouse.TurnLeft();
            Mouse.Move();
            Mouse.TurnRight();
            Mouse.Move();
            Mouse.TurnLeft();
            Mouse.Move();
            Mouse.TurnRight();
            Mouse.Move();
            var exited = Maze.AmIOut();
            Assert.IsTrue(exited);
            Maze.ShouldBeReset = true;
        }
    }
}
