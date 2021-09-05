# .NET Serializers benchmark comparison

This is just a simple comparison between differents serializers available for .NET.
The benchmark measure serializations and deserialization of a collections of 100.000 simple objects and 1.000 complex objects. The simple objects are objects with only 2 properties. The complex objects are nested objects with collections and abstractions.

## Performance

The benchmark was run on:

* Windows 10 Pro x64.
* AMD Ryzen 5 1600 3.2GHz.
* 16 GB RAM.

*The measurement unit is milliseconds*

### Serialize 100000 simple objects:

* BinaryFormatter: 315
* XmlSerializer: 255
* DataContractSerializer: 181
* TextJsonSerializer: 120
* protobuf-net: 295
* MessagePack-CSharp : 216

### Deserialize 100000 simple objects:

* BinaryFormatter: 316
* XmlSerializer: 318
* DataContractSerializer: 317
* TextJsonSerializer: 94
* protobuf-net: 48
* MessagePack-CSharp : 63

### Serialize 1000 complex objects:

* BinaryFormatter: 20700
* XmlSerializer: 9994
* DataContractSerializer: 12883
* TextJsonSerializer: 2300
* protobuf-net: 4962
* MessagePack-CSharp : 1307

### Deserialize 1000 complex objects:

* BinaryFormatter: 34767
* XmlSerializer: 21020
* DataContractSerializer: 29528
* TextJsonSerializer: 8565
* protobuf-net: 7545
* MessagePack-CSharp : 4410