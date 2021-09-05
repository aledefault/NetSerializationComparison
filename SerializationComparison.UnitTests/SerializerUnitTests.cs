using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SerializationComparison.UnitTests
{
    public class SerializerUnitTests
    {
        [Theory]
        [MemberData(nameof(GetNullAndSerializers))]
        public void Should_return_null_when_entry_is_null(object entry, ISerializerBehaviour serializerBehaviour)
        {
            var serializer = new Serializer(serializerBehaviour);
            var result = serializer.Serialize(entry);

            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(GetNonSerializableObjects))]
        public void Should_return_null_when_the_object_is_not_serializable(object entry, ISerializerBehaviour serializerBehaviour)
        {
            var serializer = new Serializer(serializerBehaviour);
            var result = serializer.Serialize(entry);

            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(GetSimpleObjects))]
        public void Should_serialize_serializable_simple_object(
            SimpleObject entry,
            ISerializerBehaviour serializerBehaviour)
        {
            var serializer = new Serializer(serializerBehaviour);
            var serialized = serializer.Serialize(entry);
            var result = serializer.Deserialize<SimpleObject>(serialized);

            Assert.NotNull(result);
            Assert.Equal(entry.Id, result.Id);
            Assert.Equal(entry.Name, result.Name);
        }

        [Theory]
        [MemberData(nameof(GetComplexObjects))]
        public void Should_serialize_serializable_complex_object(
            ComplexObject entry,
            ISerializerBehaviour serializerBehaviour)
        {
            var serializer = new Serializer(serializerBehaviour);
            var serialized = serializer.Serialize(entry);
            var result = serializer.Deserialize<ComplexObject>(serialized);

            Assert.NotNull(result);
            Assert.Equal(entry, result);
        }

        public static IEnumerable<object[]> GetNullAndSerializers() =>
            new[]
            {
                new object[]
                {
                    null,
                    new BinaryFormatterSerializerBehaviour()
                },
                new object[]
                {
                    null,
                    new XmlSerializerSerializerBehaviour()
                },
                new object[]
                {
                    null,
                    new DataContractSerializerBehaviour()
                },
                new object[]
                {
                    null,
                    new TextJsonSerializerBehaviour()
                },
                new object[]
                {
                    null,
                    new ProtoBufSerializerBehaviour()
                },
                new object[]
                {
                    null,
                    new MessagePackSerializerBehaviour()
                }
            };

        public static IEnumerable<object[]> GetSimpleObjects() =>
            new[]
            {
                new object[]
                {
                    new SimpleObject { Id = 1, Name = "Gordon" },
                    new BinaryFormatterSerializerBehaviour()
                },
                new object[]
                {
                    new SimpleObject { Id = 1, Name = "Gordon" },
                    new XmlSerializerSerializerBehaviour()
                },
                new object[]
                {
                    new SimpleObject { Id = 1, Name = "Gordon" },
                    new DataContractSerializerBehaviour()
                },
                new object[]
                {
                    new SimpleObject { Id = 1, Name = "Gordon" },
                    new TextJsonSerializerBehaviour()
                },
                new object[]
                {
                    new SimpleObject { Id = 1, Name = "Gordon" },
                    new ProtoBufSerializerBehaviour()
                },
                new object[]
                {
                    new SimpleObject { Id = 1, Name = "Gordon" },
                    new MessagePackSerializerBehaviour()
                }
            };

        public static IEnumerable<object[]> GetComplexObjects() =>
            new[]
            {
                new object[]
                {
                    GetComplexObject(),
                    new BinaryFormatterSerializerBehaviour()
                },
                new object[]
                {
                    GetComplexObject(),
                    new XmlSerializerSerializerBehaviour()
                },
                new object[]
                {
                    GetComplexObject(),
                    new DataContractSerializerBehaviour()
                },
                new object[]
                {
                    GetComplexObject(),
                    new TextJsonSerializerBehaviour()
                },
                new object[]
                {
                    GetComplexObject(),
                    new ProtoBufSerializerBehaviour()
                },
                new object[]
                {
                    GetComplexObject(),
                    new MessagePackSerializerBehaviour()
                },
            };

        private static ComplexObject GetComplexObject()
        {
            var random = new Random();
            return new ComplexObject
            {
                Name = "Gordon's Jar",
                Cookies = new List<Cookie>
                {
                    new Cookie
                    {
                        Toppings = new List<Topping>
                        {
                            new ChocolateTopping
                            {
                                Origin = "Xen"
                            },
                            new PeanutTopping
                            {
                                Fat = random.Next(0, 101)
                            },
                            new SomethingGreenTopping
                            {
                                IsItSafe = false
                            }
                        }
                    }
                }
            };
        }

        public static IEnumerable<object[]> GetNonSerializableObjects() =>
            new[]
            {
                new object[]
                {
                    new
                    {
                        Id = 1,
                        Name = "Gordon"
                    },
                    new BinaryFormatterSerializerBehaviour()
                },
                new object[]
                {
                    new
                    {
                        Id = 1,
                        Name = "Gordon"
                    },
                    new XmlSerializerSerializerBehaviour()
                },
                new object[]
                {
                    new
                    {
                        Id = 1,
                        Name = "Gordon"
                    },
                    new DataContractSerializerBehaviour()
                },
                new object[]
                {
                    new
                    {
                        Id = 1,
                        Name = "Gordon"
                    },
                    new TextJsonSerializerBehaviour()
                },
                new object[]
                {
                    new
                    {
                        Id = 1,
                        Name = "Gordon"
                    },
                    new ProtoBufSerializerBehaviour()
                },
                new object[]
                {
                    new
                    {
                        Id = 1,
                        Name = "Gordon"
                    },
                    new MessagePackSerializerBehaviour()
                }
            };
    }
}
