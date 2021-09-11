using MessagePack;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;

namespace SerializationComparison
{
    public static class ComplexObjectFactory
    {
        public static ComplexObject Make()
        {
            var random = new Random();

            return new ComplexObject
            {
                Name = Guid.NewGuid().ToString(),
                Cookies = Enumerable.Range(0, 100).Select(_ => new Cookie
                {
                    Toppings = Enumerable.Range(0, 100).Select(_ =>
                    {
                        int number = new Random().Next(0, 3);

                        Topping topping = number switch
                        {
                            0 => new ChocolateTopping
                            {
                                Origin = Guid.NewGuid().ToString()
                            },
                            1 => new PeanutTopping
                            {
                                Fat = random.Next(0, 101)
                            },
                            2 => new SomethingGreenTopping
                            {
                                IsItSafe = random.Next(1, 3) % 2 == 0
                            },
                            _ => throw new NotImplementedException()
                        };

                        return topping;
                    }).ToList()
                }).ToList()

            };
        }
    }

    [MessagePackObject]
    [Serializable]
    [ProtoContract]
    public class ComplexObject : IEquatable<ComplexObject>
    {
        [Key(1)]
        [ProtoMember(1)]
        public string Name { get; set; }

        [Key(2)]
        [ProtoMember(2)]
        public List<Cookie> Cookies { get; set; }

        public bool Equals(ComplexObject other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Name.Equals(other.Name)
                && ((Cookies is null && other.Cookies is null) 
                 || (Cookies is not null && other.Cookies is not null && Enumerable.SequenceEqual(Cookies, other.Cookies)));
        }

        // For simplicity's sake, the rest of Equals and operators will be omitted.
    }

    [MessagePackObject]
    [Serializable]
    [ProtoContract]
    public class Cookie : IEquatable<Cookie>
    {
        [Key(1)]
        [ProtoMember(1)]
        public List<Topping> Toppings { get; set; }

        public bool Equals(Cookie other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return (Toppings is null && other.Toppings is null) 
                || (Toppings is not null && other.Toppings is not null && Enumerable.SequenceEqual(Toppings, other.Toppings));
        }
    }

    [Serializable]
    [System.Text.Json.Serialization.JsonConverter(typeof(ToppingJsonConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(JsonCreationConverter))]
    [XmlInclude(typeof(ChocolateTopping))]
    [XmlInclude(typeof(PeanutTopping))]
    [XmlInclude(typeof(SomethingGreenTopping))]

    [KnownType(typeof(ChocolateTopping))]
    [KnownType(typeof(PeanutTopping))]
    [KnownType(typeof(SomethingGreenTopping))]

    [MessagePackObject]
    [MessagePack.Union(100, typeof(ChocolateTopping))]
    [MessagePack.Union(200, typeof(PeanutTopping))]
    [MessagePack.Union(300, typeof(SomethingGreenTopping))]

    [ProtoContract]
    [ProtoInclude(100, typeof(ChocolateTopping))]
    [ProtoInclude(200, typeof(PeanutTopping))]
    [ProtoInclude(300, typeof(SomethingGreenTopping))]
    public abstract class Topping : IEquatable<Topping>
    {
        public virtual bool Equals(Topping other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return true;
        }
    }

    [MessagePackObject]
    [Serializable]
    [ProtoContract]
    public class ChocolateTopping : Topping
    {
        [Key(1)]
        [ProtoMember(1)]
        public string Origin { get; set; }

        public override bool Equals(Topping other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (other is not ChocolateTopping chocoOther)
                return false;

            return Origin.Equals(chocoOther.Origin);
        }
    }

    [MessagePackObject]
    [Serializable]
    [ProtoContract]
    public class PeanutTopping : Topping
    {
        [Key(1)]
        [ProtoMember(1)]
        public int Fat { get; set; }

        public override bool Equals(Topping other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (other is not PeanutTopping peanutOther)
                return false;

            return Fat == peanutOther.Fat;
        }
    }

    [MessagePackObject]
    [Serializable]
    [ProtoContract]
    public class SomethingGreenTopping : Topping
    {
        [Key(1)]
        [ProtoMember(1)]
        public bool IsItSafe { get; set; }

        public override bool Equals(Topping other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (other is not SomethingGreenTopping greenOther)
                return false;

            return IsItSafe == greenOther.IsItSafe;
        }
    }

    public class ToppingJsonConverter : System.Text.Json.Serialization.JsonConverter<Topping>
    {
        enum TypeDiscriminator
        {
            Chocolate = 1,
            Peanut = 2,
            SomethingGreen = 3
        }

        public override bool CanConvert(Type typeToConvert) => typeof(Topping).IsAssignableFrom(typeToConvert);

        public override Topping Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new System.Text.Json.JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new System.Text.Json.JsonException();

            string propertyName = reader.GetString();
            if (propertyName != "TypeDiscriminator")
                throw new System.Text.Json.JsonException();

            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
                throw new System.Text.Json.JsonException();

            var typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
            Topping topping = typeDiscriminator switch
            {
                TypeDiscriminator.Chocolate => new ChocolateTopping(),
                TypeDiscriminator.Peanut => new PeanutTopping(),
                TypeDiscriminator.SomethingGreen => new SomethingGreenTopping(),
                _ => throw new System.Text.Json.JsonException()
            };

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return topping;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case nameof(ChocolateTopping.Origin):
                            ((ChocolateTopping)topping).Origin = reader.GetString();
                            break;

                        case nameof(PeanutTopping.Fat):
                            ((PeanutTopping)topping).Fat = reader.GetInt32();
                            break;

                        case nameof(SomethingGreenTopping.IsItSafe):
                            ((SomethingGreenTopping)topping).IsItSafe = reader.GetBoolean();
                            break;
                    }
                }
            }

            throw new System.Text.Json.JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Topping topping, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (topping is ChocolateTopping choco)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Chocolate);
                writer.WriteString(nameof(ChocolateTopping.Origin), choco.Origin);
            }
            else if (topping is PeanutTopping peanut)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Peanut);
                writer.WriteNumber(nameof(peanut.Fat), peanut.Fat);
            }
            else if (topping is SomethingGreenTopping xen)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.SomethingGreen);
                writer.WriteBoolean(nameof(SomethingGreenTopping.IsItSafe), xen.IsItSafe);
            }

            writer.WriteEndObject();
        }
    }

    public class JsonCreationConverter : Newtonsoft.Json.JsonConverter
    {
        enum TypeDiscriminator
        {
            Chocolate = 1,
            Peanut = 2,
            SomethingGreen = 3
        }

        public override bool CanConvert(Type typeToConvert) => typeof(Topping).IsAssignableFrom(typeToConvert);

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (jo["TypeDiscriminator"].Value<int>() == (int)TypeDiscriminator.Chocolate)
                return new ChocolateTopping { Origin = jo["Origin"].Value<string>() };

            if (jo["TypeDiscriminator"].Value<int>() == (int)TypeDiscriminator.Peanut)
                return new PeanutTopping { Fat = jo["Fat"].Value<int>() };

            if (jo["TypeDiscriminator"].Value<int>() == (int)TypeDiscriminator.SomethingGreen)
                return new SomethingGreenTopping { IsItSafe = jo["IsItSafe"].Value<bool>() };

            return null;
        }

        public override void WriteJson(JsonWriter writer, object topping,  Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartObject();

            if (topping is ChocolateTopping choco)
            {
                writer.WritePropertyName("TypeDiscriminator");
                writer.WriteValue((int)TypeDiscriminator.Chocolate);
                writer.WritePropertyName(nameof(ChocolateTopping.Origin));
                writer.WriteValue(choco.Origin);
            }
            else if (topping is PeanutTopping peanut)
            {
                writer.WritePropertyName("TypeDiscriminator");
                writer.WriteValue((int)TypeDiscriminator.Peanut);
                writer.WritePropertyName(nameof(PeanutTopping.Fat));
                writer.WriteValue(peanut.Fat);
            }
            else if (topping is SomethingGreenTopping xen)
            {
                writer.WritePropertyName("TypeDiscriminator");
                writer.WriteValue((int)TypeDiscriminator.SomethingGreen);
                writer.WritePropertyName(nameof(SomethingGreenTopping.IsItSafe));
                writer.WriteValue(xen.IsItSafe);
            }

            writer.WriteEndObject();
        }
    }
}
