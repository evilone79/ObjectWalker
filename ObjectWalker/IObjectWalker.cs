namespace ObjectWalker
{
    public enum ItemType
    {
        Dictionary,
        Collection,
        NameValue,
        Name
    }
    public interface IObjectWalker
    {
        void OnStart();
        void WalkDown(string name);
        void WalkLevel(string name, string value, ItemType type);
        void WalkUp();
        void OnFinish();
    }

    public interface IItemFormatter
    {
        string GetItemText(string name, string value, ItemType type);
    }

    public abstract class TextWalker:IObjectWalker
    {
        protected IItemFormatter Formatter;

        protected TextWalker(IItemFormatter formatter)
        {
            Formatter = formatter;
        }

        public abstract void OnStart();
        public abstract void WalkDown(string name);
        public void WalkLevel(string name, string value, ItemType type)
        {
            WalkLevel(Formatter.GetItemText(name, value, type));
        }
        public abstract void WalkLevel(string text);
        public abstract void WalkUp();
        public abstract void OnFinish();
    }
}