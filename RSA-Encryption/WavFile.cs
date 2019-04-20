﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace RSA_Encryption
{
    public class WavFile
    {
        public double[] Left;
        public double[] Right;

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FilePathCopy { get; set; }

        public int HeaderSize { get; set; }
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

        public WavFile(string fileName, string fileCopy)
        {
            FileName = fileName;
            FilePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, @"data\", fileName);
            FilePathCopy = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, @"data\", fileCopy);
        }

        public void ReadWav()
        {
            WriteMetaData();
            using (FileStream fs = File.Open(FilePath, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);

                HeaderSize = 44;
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

        public void EditWavFile(int from, int until, Channel channel)
        {
            using (FileStream fs = File.Open(FilePath, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);
                using (var fileStream = new FileStream(FilePathCopy, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var bw = new BinaryWriter(fileStream))
                {
                    bw.Write(reader.ReadBytes(HeaderSize));
                    for (int i = 0; i < Subchunk2Size ; i+=2)
                    {
                        var test = reader.ReadBytes(2);

                        if (i >= GetBytesForSeconds(from) && i <= GetBytesForSeconds(until) && IsChosenChannel(channel, i))
                        {
                            byte[] bytes = new byte[test.Length];

                            for (int j = 0; j < test.Length; j++)
                            {
                                bytes[j] = (byte)(j + 1);
                            }
                            test = bytes;
                        }
                        
                        bw.Write(test);
                    }
                }
            }
        }

        public void WriteMetaData()
        {
            using (FileStream fs = File.Open(FilePath, FileMode.Open))
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

        public string GetSeconds()
        {
            return String.Concat("This audio last ", (Subchunk2Size / ByteRate), " seconds.");
        }

        public void Play()
        {
            using (var audioFile = new AudioFileReader(FilePathCopy))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private int GetBytesForSeconds(int seconds)
        {
            return seconds * ByteRate;
        }

        private bool IsChosenChannel(Channel channel, int byteNum)
        {
            bool result;
            switch(channel)
            {
                case Channel.Left:
                    {
                        return result = byteNum % 4 == 0 ? true : false;
                    }
                case Channel.Right:
                    {
                        return result = (byteNum % 4 != 0 && byteNum % 2 == 0) ? true : false;
                    }
                default:
                    return false;
            }
        }

        private double BytesToDouble(byte firstByte, byte secondByte)
        {
            var s = BitConverter.ToUInt16(new byte[2] { (byte)secondByte, (byte)firstByte }, 0);
            //return s;
            return s / 32768.0;
        }
    }
}
