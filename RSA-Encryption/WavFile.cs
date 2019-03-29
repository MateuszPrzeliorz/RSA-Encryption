using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RSA_Encryption
{
    public class WavFile
    {
        public float[] Left;
        public float[] Right;

        public int ChunkID { get; set; }
        public int ChunkSize { get; set; }
        public int Format { get; set; }

        public int Subchunk1ID { get; set; }
        public int Subchunk1Size { get; set; }
        public int AudioFormat { get; set; }
        public int NumChannels { get; set; }
        public int SampleRate { get; set; }
        public int ByteRate { get; set; }
        public int BlockAlign { get; set; }
        public int BitsPerSample { get; set; }

        public int Subchunk2ID { get; set; }
        public int Subchunk2Size { get; set; }

        public bool ReadWav(string filename)
        {

            using (FileStream fs = File.Open(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, @"data\", filename), FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);
                ChunkID = reader.ReadInt32();
                ChunkSize = reader.ReadInt32();
                Format = reader.ReadInt32();
                Subchunk1ID = reader.ReadInt32();
                Subchunk1Size = reader.ReadInt32(); 
                AudioFormat = reader.ReadInt16();
                NumChannels = reader.ReadInt16();
                SampleRate = reader.ReadInt32();
                ByteRate = reader.ReadInt32();
                BlockAlign = reader.ReadInt16();
                BitsPerSample = reader.ReadInt16();
                Subchunk2ID = reader.ReadInt32();
                Subchunk2Size = reader.ReadInt32();

                // DATA!
                byte[] byteArray = reader.ReadBytes(Subchunk2Size);

                int bytesForSamp = BitsPerSample / 8;
                int samps = Subchunk2Size / bytesForSamp;


                float[] asFloat = null;
                switch (BitsPerSample)
                {
                    case 64:
                        double[] asDouble = new double[samps];
                        Buffer.BlockCopy(byteArray, 0, asDouble, 0, Subchunk2Size);
                        asFloat = Array.ConvertAll(asDouble, e => (float)e);
                        break;
                    case 32:
                        asFloat = new float[samps];
                        Buffer.BlockCopy(byteArray, 0, asFloat, 0, Subchunk2Size);
                        break;
                    case 16:
                        Int16[] asInt16 = new Int16[samps];
                        Buffer.BlockCopy(byteArray, 0, asInt16, 0, Subchunk2Size);
                        asFloat = Array.ConvertAll(asInt16, e => e / (float)Int16.MaxValue);
                        break;
                    default:
                        return false;
                }

                switch (NumChannels)
                {
                    case 1:
                        Left = asFloat;
                        Right = null;
                        return true;
                    case 2:
                        Left = new float[samps];
                        Right = new float[samps];
                        for (int i = 0, s = 0; i < samps/2; i++)
                        {
                            Left[i] = asFloat[s++];
                            Right[i] = asFloat[s++];
                        }
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}
