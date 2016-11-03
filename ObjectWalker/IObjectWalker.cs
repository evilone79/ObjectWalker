namespace ObjectUtils
{
    public interface IObjectWalker
    {
        void OnStart(string rootTypeName);
        void OnBeginContainer(IParseItem item);
        void OnField(IParseItem item);
        void OnEndContainer();
        void OnFinish();
    }

    
}