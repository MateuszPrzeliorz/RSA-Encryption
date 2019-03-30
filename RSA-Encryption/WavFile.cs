﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RSA_Encryption
{
    public class WavFile
    {
        public double[] Left;
        public double[] Right;

        public string FileName { get; set; }

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

        public WavFile(string fileName)
        {
            FileName = fileName;
        }

        public void ReadWav()
        {
            WriteMetaData();

            using (FileStream fs = File.Open(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, @"data\", FileName), FileMode.Open))
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
                int samples = (NumChannels == 2) ? (byteArray.Length) / 4 : (byteArray.Length) / 2;

                Left = new double[samples];
                Right = (NumChannels == 2) ? new double[samples] : null;

                // Write to double array/s:
                int i = 0, offset = 0;
                while (offset < byteArray.Length)
                {
                    Left[i] = BytesToDouble(byteArray[offset], byteArray[offset + 1]);
                    offset += 2;
                    if (NumChannels == 2)
                    {
                        Right[i] = BytesToDouble(byteArray[offset], byteArray[offset + 1]);
                        offset += 2;
                    }
                    i++;
                }
            }
        }

        public void WriteMetaData()
        {
            using (FileStream fs = File.Open(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, @"data\", FileName), FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);
                Console.WriteLine($"{nameof(ChunkID)}: {BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty).ConvertHex()}");
                Console.WriteLine($"{nameof(ChunkSize)}: {reader.ReadInt32()}");
                Console.WriteLine($"{nameof(Format)}: {BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty).ConvertHex()}");
                Console.WriteLine($"{nameof(Subchunk1ID)}: {BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty).ConvertHex()}");
                Console.WriteLine($"{nameof(Subchunk1Size)}: {reader.ReadInt32()}");
                Console.WriteLine($"{nameof(AudioFormat)}: {reader.ReadInt16()}");
                Console.WriteLine($"{nameof(NumChannels)}: {reader.ReadInt16()}");
                Console.WriteLine($"{nameof(SampleRate)}: {reader.ReadInt32()}");
                Console.WriteLine($"{nameof(ByteRate)}: {reader.ReadInt32()}");
                Console.WriteLine($"{nameof(BlockAlign)}: {reader.ReadInt16()}");
                Console.WriteLine($"{nameof(BitsPerSample)}: {reader.ReadInt16()}");
                Console.WriteLine($"{nameof(Subchunk2ID)}: {BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty).ConvertHex()}");
                Console.WriteLine($"{nameof(Subchunk2Size)}: {reader.ReadInt32()}\n");
            }
        }

        private double BytesToDouble(byte firstByte, byte secondByte)
        {
            int s = BitConverter.ToUInt16(new byte[2] { (byte)secondByte, (byte)firstByte }, 0);
            //int s = (secondByte << 8) | firstByte;
            return s / 32768.0;
        }
    }
}
