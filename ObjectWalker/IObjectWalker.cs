namespace ObjectWalker
{
    public interface IObjectWalker
    {
        void OnStart();
        void StepDown();
        void WalkLevel(IParseItem item);
        void StepUp();
        void OnFinish();
    }

    
}