namespace ObjectWalker
{
    public interface IObjectWalker
    {
        void OnStart();
        void WalkDown(string text);
        void WalkLevel(string text);
        void WalkUp();
        void OnFinish();
    }

    
}