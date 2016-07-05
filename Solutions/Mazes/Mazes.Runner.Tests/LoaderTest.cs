using System.Reflection;
using Mazes.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mazes.Runner.Tests
{
    [TestClass]
    public class LoaderTest
    {
        [TestMethod]
        public void LoadSolverReturnsStupidSolver()
        {
            string assemblyPath = Assembly.GetAssembly(typeof(SampleMazeSolver.MazeSolver)).Location;
            IMazeSolver solver = Loader.Load<IMazeSolver>(assemblyPath);

            Assert.IsNotNull(solver);
            Assert.IsInstanceOfType(solver, typeof(SampleMazeSolver.MazeSolver));
        }

        [TestMethod]
        public void LoadBuilderReturnsSimpleBuilder()
        {
            string assemblyPath = Assembly.GetAssembly(typeof(SampleMazeBuilder.SimpleMazeBuilder)).Location;

            IMazeBuilder builder = Loader.Load<IMazeBuilder>(assemblyPath);

            Assert.IsNotNull(builder);
            Assert.IsInstanceOfType(builder, typeof(SampleMazeBuilder.SimpleMazeBuilder));
        }
    }
}
