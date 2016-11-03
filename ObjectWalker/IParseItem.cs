namespace ObjectUtils
{
    public interface IParseItem
    {
        string TypeName { get; }
        string FieldName { get; }
        string Value { get; }
    }
}