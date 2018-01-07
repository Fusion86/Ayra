//
// Taken from https://stackoverflow.com/a/2624377/2125072, thanks Adam Robinson!
//

using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Ayra.Core.Helpers
{
    public enum Endianness
    {
        BigEndian,
        LittleEndian
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EndianAttribute : Attribute
    {
        public Endianness Endianness { get; private set; }

        public EndianAttribute(Endianness endianness)
        {
            Endianness = endianness;
        }
    }

    public static class EndiannessHelper
    {
        public static void RespectEndianness(Type type, byte[] data)
        {
            var fields = type.GetFields().Where(f => f.IsDefined(typeof(EndianAttribute), false))
                .Select(f => new
                {
                    Field = f,
                    Attribute = (EndianAttribute)f.GetCustomAttributes(typeof(EndianAttribute), false)[0],
                    Offset = Marshal.OffsetOf(type, f.Name).ToInt32()
                }).ToList();

            foreach (var field in fields)
            {
                if ((field.Attribute.Endianness == Endianness.BigEndian && BitConverter.IsLittleEndian) ||
                    (field.Attribute.Endianness == Endianness.LittleEndian && !BitConverter.IsLittleEndian))
                {
                    Array.Reverse(data, field.Offset, Marshal.SizeOf(field.Field.FieldType));
                }
            }
        }
    }
}
