using System.Runtime.Serialization.Formatters.Binary;

namespace EntityContext;

public class BinaryProvider : DataProvider
{
    public BinaryProvider(Type type)
        : base(type)
    {

    }

    public override void Serialize(object graph, string filePath)
    {
        using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fileStream, graph);
        }
    }

    public override object Deserialize(string filePath)
    {
        object obj;
        using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(fileStream);
            return obj;
        }

    }
}
