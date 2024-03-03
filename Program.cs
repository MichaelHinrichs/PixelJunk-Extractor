using System.IO;
using System.IO.Compression;

namespace PixelJunk_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            using FileStream pkiwin = File.OpenRead(args[0] + "//shooter.pkiwin");
            BinaryReader br = new(pkiwin);
            Directory.CreateDirectory(args[0] + "//shooter");
            br.BaseStream.Position = 8;
            SUBFILE[] subfiles = new SUBFILE[br.ReadInt32()];
            for (int i = 0; i < subfiles.Length; i++)
            {
                subfiles[i].sizeUncompressed = br.ReadInt32();//unknown
                subfiles[i].sizeCompressed = br.ReadInt32();
                subfiles[i].offset = br.ReadInt32();
                br.ReadInt32();//unknown
            }
            int n = 0;
            using FileStream pkdwin = File.OpenRead(args[0] + "//shooter.pkdwin");
            foreach (SUBFILE sub in subfiles)
            {
                br = new(pkdwin);
                br.BaseStream.Position = sub.offset;
                using FileStream FS = File.Create(args[0] + "//shooter//" + n);
                BinaryWriter bw = new(FS);

                MemoryStream ms = new();
                br.ReadInt16();
                using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes(sub.sizeCompressed)), CompressionMode.Decompress))
                    ds.CopyTo(ms);
                br = new(ms);
                br.BaseStream.Position = 0;
                bw.Write(br.ReadBytes(sub.sizeUncompressed));
                bw.Close();
                n++;
            }
            br.Close();
        }
    }
    struct SUBFILE
    {
        public int sizeUncompressed;
        public int sizeCompressed;
        public int offset;
    }
}
