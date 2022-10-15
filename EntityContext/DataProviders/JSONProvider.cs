using System.Text.Json;

namespace EntityContext;

public class JSONProvider : DataProvider
{
    public JSONProvider(Type type)
        : base(type)
    {

    }

    public override void Serialize(object graph, string filePath)
    {
        using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            JsonSerializer.Serialize(fileStream, graph, _type);
        }
    }

    public override object Deserialize(string filePath)
    {
        object obj;
        using (var fileStream = File.OpenRead(filePath))
        {
            obj = JsonSerializer.Deserialize(fileStream, _type);
            return obj;
        }
    }
}
