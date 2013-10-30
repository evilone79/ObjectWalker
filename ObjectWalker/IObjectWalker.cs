namespace ObjectWalker
{
    public interface IObjectWalker
    {
        void WalkDown(string text);
        void WalkLevel(string text);
        void WalkUp();
    }

    
}