using MessagePack;
using ProtoBuf;
using System;

namespace SerializationComparison
{
    [MessagePackObject]
    [Serializable]
    [ProtoContract]
    public class SimpleObject
    {
        [ProtoMember(1)]
        [Key(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        [Key(2)]
        public string Name { get; set; }
    }
}
