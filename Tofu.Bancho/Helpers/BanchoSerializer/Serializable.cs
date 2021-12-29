using System;
using System.IO;
using System.Linq;
using System.Reflection;
using EeveeTools.Helpers;

namespace Tofu.Bancho.Helpers.BanchoSerializer {
    public abstract class Serializable {
        public Serializable() {}
        public Serializable(Stream readStream) => this.ReadFromStream(readStream);

        public virtual void ReadFromStream(Stream readStream, bool resetSeek = true) {
            IOrderedEnumerable<FieldInfo> properties =
                this.GetType()
                    .GetFields()
                    //Where the `BanchoSerialize` Attribute exists
                    .Where(property => Attribute.IsDefined(property, typeof(BanchoSerialize)))
                    //Order by Decleration Order, this is to ensure that All objects from top to bottom get serialized, instead of being in random order
                    .OrderBy(property => ((BanchoSerialize)
                                 property.GetCustomAttributes(typeof(BanchoSerialize), false)
                                         .Single())
                                         .Order
                    );

            if(resetSeek)
                readStream.Seek(0, SeekOrigin.Begin);

            using BanchoReader reader = new(readStream);

            foreach (FieldInfo fieldType in properties) {
                //For Arrays we require a little bit more code to handle multiple reads
                if (fieldType.FieldType.IsArray) {
                    #region Array Serialization

                    string baseType = fieldType.FieldType.Name[..^2];

                    //Enum Handling, gets underlying enum type and uses that.
                    if (fieldType.FieldType.GetElementType() != null) {
                        // ReSharper disable once PossibleNullReferenceException
                        if (fieldType.FieldType.GetElementType().IsEnum) {
                            // ReSharper disable once PossibleNullReferenceException
                            baseType = fieldType.FieldType.GetElementType().GetEnumUnderlyingType().Name;
                        }
                    }

                    switch (baseType) {
                        case "String": {
                            string[] array = (string[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadString();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "Byte": {
                            byte[] array = (byte[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadByte();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "Int32": {
                            int[] array = (int[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadInt32();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "Int16": {
                            short[] array = (short[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadInt16();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "Int64": {
                            long[] array = (long[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadInt64();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "UInt32": {
                            uint[] array = (uint[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadUInt32();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "UInt16": {
                            ushort[] array = (ushort[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadUInt16();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "UInt64": {
                            ulong[] array = (ulong[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadUInt64();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "Single": {
                            float[] array = (float[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadSingle();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "Double": {
                            double[] array = (double[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadDouble();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        case "Boolean": {
                            bool[] array = (bool[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                array[i] = reader.ReadBoolean();

                            fieldType.SetValue(this, array);

                            break;
                        }
                        default: {
                            Serializable[] array = (Serializable[]) fieldType.GetValue(this);

                            if(array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++) {
                                Serializable tempSerializable = (Serializable)Activator.CreateInstance(fieldType.FieldType.GetElementType());
                                tempSerializable.ReadFromStream(readStream, false);

                                array[i] = tempSerializable;
                            }

                            fieldType.SetValue(this, array);

                            break;
                        }
                    }

                    #endregion
                } else {
                    string propType = fieldType.FieldType.Name;

                    if (fieldType.FieldType.IsEnum) {
                        propType = fieldType.FieldType.GetEnumUnderlyingType().Name;
                    }

                    switch (propType) {
                        case "Byte":
                            fieldType.SetValue(this, reader.ReadByte());
                            break;
                        case "Boolean":
                            fieldType.SetValue(this, reader.ReadBoolean());
                            break;
                        case "Int32":
                            fieldType.SetValue(this, reader.ReadInt32());
                            break;
                        case "Int16":
                            fieldType.SetValue(this, reader.ReadInt16());
                            break;
                        case "Int64":
                            fieldType.SetValue(this, reader.ReadInt64());
                            break;
                        case "UInt32":
                            fieldType.SetValue(this, reader.ReadUInt32());
                            break;
                        case "UInt16":
                            fieldType.SetValue(this, reader.ReadUInt16());
                            break;
                        case "UInt64":
                            fieldType.SetValue(this, reader.ReadUInt64());
                            break;
                        case "String":
                            fieldType.SetValue(this, reader.ReadString());
                            break;
                        case "Single":
                            fieldType.SetValue(this, reader.ReadSingle());
                            break;
                        case "Double":
                            fieldType.SetValue(this, reader.ReadDouble());
                            break;
                        default:
                            Serializable serializable = (Serializable)Activator.CreateInstance(fieldType.FieldType);
                            serializable?.ReadFromStream(readStream, false);

                            fieldType.SetValue(this, serializable);
                            break;
                    }
                }
            }
        }

        public virtual void WriteToStream(Stream writeStream) {
            IOrderedEnumerable<FieldInfo> properties =
                this.GetType()
                    .GetFields()
                    //Where the `BanchoSerialize` Attribute exists
                    .Where(property => Attribute.IsDefined(property, typeof(BanchoSerialize)))
                    //Order by Decleration Order, this is to ensure that All objects from top to bottom get serialized, instead of being in random order
                    .OrderBy(property => ((BanchoSerialize)
                                             property.GetCustomAttributes(typeof(BanchoSerialize), false)
                                                 .Single())
                                 .Order
                    );

            using BanchoWriter writer = new(writeStream);

            foreach (FieldInfo fieldType in properties) {
                if (fieldType.FieldType.IsArray) {
                    #region Array Serialization

                    string baseType = fieldType.FieldType.Name[..^2];

                    //Enum Handling, gets underlying enum type and uses that.
                    if (fieldType.FieldType.GetElementType() != null) {
                        // ReSharper disable once PossibleNullReferenceException
                        if (fieldType.FieldType.GetElementType().IsEnum) {
                            // ReSharper disable once PossibleNullReferenceException
                            baseType = fieldType.FieldType.GetElementType().GetEnumUnderlyingType().Name;
                        }
                    }

                    switch (baseType) {
                        case "String": {
                            string[] array = (string[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "Byte": {
                            byte[] array = (byte[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "Int32": {
                            int[] array = (int[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "Int16": {
                            short[] array = (short[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "Int64": {
                            long[] array = (long[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "UInt32": {
                            uint[] array = (uint[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "UInt16": {
                            ushort[] array = (ushort[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "UInt64": {
                            ulong[] array = (ulong[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "Single": {
                            float[] array = (float[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "Double": {
                            double[] array = (double[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        case "Boolean": {
                            bool[] array = (bool[]) fieldType.GetValue(this);

                            if (array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++)
                                writer.Write(array[i]);

                            break;
                        }
                        default: {
                            Serializable[] array = (Serializable[]) fieldType.GetValue(this);

                            if(array == null)
                                throw new NullReferenceException("Initialize Serializable Array fields!!!!!!");

                            for (int i = 0; i != array.Length; i++) {
                                Serializable tempSerializable = (Serializable)Activator.CreateInstance(fieldType.FieldType.GetElementType());
                                tempSerializable?.WriteToStream(writeStream);
                            }

                            break;
                        }
                    }

                    #endregion
                } else {
                    string propType = fieldType.FieldType.Name;

                    if (fieldType.FieldType.IsEnum) {
                        propType = fieldType.FieldType.GetEnumUnderlyingType().Name;
                    }

                    switch (propType) {
                        case "Byte":
                            byte byteValue = (byte)fieldType.GetValue(this);
                            writer.Write(byteValue);
                            break;
                        case "Boolean":
                            bool boolValue = (bool)fieldType.GetValue(this);
                            writer.Write(boolValue);
                            break;
                        case "Int32":
                            int intValue = (int)fieldType.GetValue(this);
                            writer.Write(intValue);
                            break;
                        case "Int16":
                            short shortValue = (short)fieldType.GetValue(this);
                            writer.Write(shortValue);
                            break;
                        case "Int64":
                            long longValue = (long)fieldType.GetValue(this);
                            writer.Write(longValue);
                            break;
                        case "UInt32":
                            uint uintValue = (uint)fieldType.GetValue(this);
                            writer.Write(uintValue);
                            break;
                        case "UInt16":
                            ushort ushortValue = (ushort)fieldType.GetValue(this);
                            writer.Write(ushortValue);
                            break;
                        case "UInt64":
                            ulong ulongValue = (ulong)fieldType.GetValue(this);
                            writer.Write(ulongValue);
                            break;
                        case "String":
                            string stringValue = (string)fieldType.GetValue(this);
                            writer.Write(stringValue);
                            break;
                        case "Single":
                            float floatValue = (float)fieldType.GetValue(this);
                            writer.Write(floatValue);
                            break;
                        case "Double":
                            double doubleValue = (double)fieldType.GetValue(this);
                            writer.Write(doubleValue);
                            break;
                        default:
                            Serializable serializable = (Serializable)Activator.CreateInstance(fieldType.FieldType);
                            serializable?.WriteToStream(writeStream);
                            break;
                    }
                }
            }
        }

        public byte[] ToBytes() {
            MemoryStream stream = new();

            this.WriteToStream(stream);

            return stream.ToArray();
        }


    }
}
