using System.IO;

namespace FR.Core;

static class BinaryReaderExtension
{
    public static T ReadStruct<T>(this BinaryReader br) where T : struct
    {
        int size = SharpDX.Utilities.SizeOf<T>();

        using SharpDX.DataStream ds = new(size, true, true);
        ds.WriteRange(br.ReadBytes(size));
        ds.Position = 0;
        return ds.Read<T>();
    }

    public static void Write<T>(this BinaryWriter bw, T @struct) where T : struct
    {
        int size = SharpDX.Utilities.SizeOf<T>();

        using SharpDX.DataStream ds = new(size, true, true);
        ds.Write(@struct);
        ds.Position = 0;
        bw.Write(ds.ReadRange<byte>(size));
    }
}
