using OOP_LAB_3.DataSerialization;

namespace EntityContext;

public class CustomProvider : DataProvider
{
    Type _genericArgumentType;
    public CustomProvider(Type type, Type genericArgumentType = null)
        : base(type)
    {
        _genericArgumentType = genericArgumentType;
    }

    public override void Serialize(object graph, string filePath)
    {
        using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            CustomSerializer cs = new CustomSerializer(_type, _genericArgumentType);
            cs.Serialize(fileStream, graph);
        }
    }
    public override object Deserialize(string filePath)
    {
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            CustomSerializer cs = new CustomSerializer(_type, _genericArgumentType);
            object obj;
            try
            {
                 obj = cs.Deserialize(fileStream);
            }
            catch(Exception)
            {
                throw new System.Runtime.Serialization.SerializationException();
            }
            if(obj is null)
                throw new System.Runtime.Serialization.SerializationException();
            return obj;
        }

    }
}
