using SerializationComparison;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

const int NumberOfSimpleObjects = 100_000;
const int NumberOfComplexObjects = 1000;
var serializers = new List<dynamic>
{
    new
    {
        Title = "BinaryFormatter",
        Serializer = new Serializer(new BinaryFormatterSerializerBehaviour()),
    },
    new
    {
        Title = "XmlSerializer",
        Serializer = new Serializer(new XmlSerializerSerializerBehaviour()),
    },
    new
    {
        Title = "DataContractSerializer",
        Serializer = new Serializer(new DataContractSerializerBehaviour()),
    },
    new
    {
        Title = "TextJsonSerializer",
        Serializer = new Serializer(new TextJsonSerializerBehaviour()),
    },
    new
    {
        Title = "protobuf-net",
        Serializer = new Serializer(new ProtoBufSerializerBehaviour()),
    },
    new
    {
        Title = "MessagePack-CSharp ",
        Serializer = new Serializer(new MessagePackSerializerBehaviour()),
    },
    new
    {
        Title = "Json.NET ",
        Serializer = new Serializer(new JsonNETSerializerBehaviour()),
    }
};

Console.WriteLine("# Serializer comparison");

Console.WriteLine($"\n## Serialize {NumberOfSimpleObjects} simple objects:");
Serialize(GetSimpleObjects());

Console.WriteLine($"\n\n## Deserialize {NumberOfSimpleObjects} simple objects:");
Deserialize(GetSimpleObjects());

Console.WriteLine($"\n\n## Serialize {NumberOfComplexObjects} complex objects:");
Serialize(GetComplexObjects());

Console.WriteLine($"\n\n## Deserialize {NumberOfComplexObjects} complex objects:");
Deserialize(GetComplexObjects());

void Serialize<T>(IEnumerable<T> objects)
{
    var stopwatch = new Stopwatch();
    foreach (var item in serializers)
    {
        WriteTitle(item.Title);
        Serializer serializer = item.Serializer;
        var toSerialize = objects.ToList();

        stopwatch.Restart();
        serializer.Serialize(toSerialize);
        stopwatch.Stop();

        WriteElapse(stopwatch.ElapsedMilliseconds);
    }
}

void Deserialize<T>(IEnumerable<T> objects)
{
    var stopwatch = new Stopwatch();
    foreach (var item in serializers)
    {
        WriteTitle(item.Title);
        Serializer serializer = item.Serializer;
        var serialized = serializer.Serialize(objects.ToList());

        stopwatch.Restart();
        serializer.Deserialize<List<T>>(serialized);
        stopwatch.Stop();

        WriteElapse(stopwatch.ElapsedMilliseconds);
    }
}

IEnumerable<SimpleObject> GetSimpleObjects()
{
    var random = new Random();
    IEnumerable<SimpleObject> result = Enumerable.Range(0, NumberOfSimpleObjects).Select(_ =>
        new SimpleObject
        {
            Id = random.Next(int.MinValue, int.MaxValue),
            Name = "Glados"
        });

    return result;
}

IEnumerable<ComplexObject> GetComplexObjects() => 
    Enumerable.Range(0, NumberOfComplexObjects).Select(_ => ComplexObjectFactory.Make());

void WriteTitle(string title) => Console.Write($"\n* {title}: ");

void WriteElapse(long millisecons) => Console.Write($"{millisecons}");