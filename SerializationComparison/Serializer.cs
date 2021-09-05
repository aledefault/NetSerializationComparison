using MessagePack;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace SerializationComparison
{
    public interface ISerializerBehaviour
    {
        byte[] Serialize(object entry);
        T Deserialize<T>(byte[] entry);
    }

    public class Serializer
    {
        public ISerializerBehaviour SerializerBehaviour { get; set; }

        public Serializer(ISerializerBehaviour serializeBehaviour) => SerializerBehaviour = serializeBehaviour;

        public byte[] Serialize(object entry)
        {
            if (entry == null)
                return null;

            if (!entry.GetType().IsSerializable)
                return null;

            return SerializerBehaviour.Serialize(entry);
        }

        public T Deserialize<T>(byte[] entry)
        {
            if (entry == null)
                return default;

            return SerializerBehaviour.Deserialize<T>(entry);
        }
    }

    public class BinaryFormatterSerializerBehaviour : ISerializerBehaviour
    {
        public byte[] Serialize(object entry)
        {
            using var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, entry);

            return stream.ToArray();
        }

        public T Deserialize<T>(byte[] entry)
        {
            using var stream = new MemoryStream(entry);
            return (T)new BinaryFormatter().Deserialize(stream);
        }
    }


    public class XmlSerializerSerializerBehaviour : ISerializerBehaviour
    {
        public byte[] Serialize(object entry)
        {
            using var stream = new MemoryStream();
            new XmlSerializer(entry.GetType()).Serialize(stream, entry);
            return stream.ToArray();
        }

        public T Deserialize<T>(byte[] entry)
        {
            using var stream = new MemoryStream(entry);
            return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
        }
    }


    public class DataContractSerializerBehaviour : ISerializerBehaviour
    {
        public byte[] Serialize(object entry)
        {
            using var stream = new MemoryStream();
            var serializer = new DataContractSerializer(entry.GetType());
            serializer.WriteObject(stream, entry);
            return stream.ToArray();
        }

        public T Deserialize<T>(byte[] entry)
        {
            using var stream = new MemoryStream(entry);
            var serializer = new DataContractSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }
    }

    public class TextJsonSerializerBehaviour : ISerializerBehaviour
    {
        public byte[] Serialize(object entry) => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(entry);
       
        public T Deserialize<T>(byte[] entry) => System.Text.Json.JsonSerializer.Deserialize<T>(entry);
    }

    public class ProtoBufSerializerBehaviour : ISerializerBehaviour
    {
        public byte[] Serialize(object entry)
        {
            using var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, entry);
            return stream.ToArray();
        }

        public T Deserialize<T>(byte[] entry)
        {
            using var stream = new MemoryStream(entry);
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }
    }

    public class MessagePackSerializerBehaviour : ISerializerBehaviour
    {
        public byte[] Serialize(object entry) => MessagePackSerializer.Serialize(entry);

        public T Deserialize<T>(byte[] entry) => MessagePackSerializer.Deserialize<T>(entry);
    }
}
