namespace EntityContext;

public abstract class DataProvider
{
    protected Type _type;
    public DataProvider(Type type)
    {
        _type = type;
    }
    public abstract void Serialize(object graph, string filePath);
    public abstract object Deserialize(string filePath);
    public static bool ClearFile(string filePath)
    {
        if (File.Exists(filePath)) 
        { 
            File.WriteAllText(filePath, "");
        }
        return File.Exists(filePath);
    }
}