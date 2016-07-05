namespace Mazes.Core
{
    public interface IMaze
    {
        bool CanIMove();
        bool AmIOut();
        bool ShouldBeReset { get; set; }
    }

    public interface IBuildableMaze
    {
        void AddVerticalWall(int width, int height);
        void AddHorizontalWall(int width, int height);
    }    
}
