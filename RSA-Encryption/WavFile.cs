using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Numerics;

namespace RSA_Encryption
{
    public class WavFile
    {
        public double[] Left;
        public double[] Right;

        public byte[] Samples;

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

                Samples = byteArray;

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

        public void EditPartOfFile(int from, int until, Channel channel)
        {
            using (FileStream fs = File.Open(FilePath, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);
                Random random = new Random();
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
                                // random transformation - to improve?
                                bytes[j] = (byte)(random.Next(1, 255));
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


        public void Encrypt()
        {
            using (FileStream fs = File.Open(FilePath, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);
                using (var fileStream = new FileStream(FilePathCopy, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var bw = new BinaryWriter(fileStream))
                {
                    bw.Write(reader.ReadBytes(44));

                    byte[] key = new byte[] { 0x22 };

                    for (int i = 0; i < Subchunk2Size; i ++)
                    {
                        for (int j = 0; j < key.Length; j++)
                        {
                             Samples[i] = (byte)(Samples[i] ^ key[j]);
                             bw.Write(Samples[i]);
                        }
                    }
                }
            }
        }

        public void EncryptXOR()
        {
            using (FileStream fs = File.Open(FilePath, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);
                using (var fileStream = new FileStream(FilePathCopy, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var bw = new BinaryWriter(fileStream))
                {
                    bw.Write(reader.ReadBytes(44));

                    string key = "1iGW5@~A&:7(GQ.-D$6.X+l%2(kP[<[H|rz.zjwi1(YxRYT.5%?!5blw>2V3Oiz&qD~q1*5vaTz@xadMZu]VV9aM]w{Y~6:fR54;";

                    for (int i = 0; i < Subchunk2Size; i++)
                    {
                        // get next byte from sample
                        byte b = Samples[i];

                        // cast to a uint
                        uint byteCode = (uint)b;

                        // get index of proper character from key
                        int keyIndex = i % key.Length;

                        // take the key character
                        char keyChar = key[keyIndex];

                        // cast it to a uint
                        uint keyCode = (uint)keyChar;

                        // perform XOR
                        uint combinedCode = byteCode ^ keyCode;

                        // cast back to a char
                        byte combinedByte = (byte)combinedCode;

                        // write result
                        Samples[i] = combinedByte;
                        bw.Write(combinedByte);
                        
                    }
                }
            }
        }

        public void EncryptRSAv2(EncryptDecrypt ED, bool enc)
        {
            using (FileStream fs = File.Open(FilePath, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);
                using (var fileStream = new FileStream(FilePathCopy, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var bw = new BinaryWriter(fileStream))
                {
                    bw.Write(reader.ReadBytes(44));
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    // create byte array
                    byte[] byteArray = new byte[Subchunk2Size];
                    for (int i = 0; i < Subchunk2Size; i++)
                    {
                        byteArray[i] = Samples[i];
                    }
                    Console.WriteLine("Subchunk2Size: " + Subchunk2Size);
                    string message = encoding.GetString(byteArray);

                    // load message
                    Console.WriteLine("Loading audio file...");
                    ED.LoadWavFile(message);

                    // encrypt
                    string encryptedStr;

                    if(enc == true)
                    {
                        Console.WriteLine("Starting encryption...");
                        encryptedStr = ED.EncryptStr();
                    } else
                    {
                        Console.WriteLine("Starting decryption...");
                        encryptedStr = ED.DecryptStr();
                    }
                   
                    Console.WriteLine("Converting encrypted str to byte array...");
                    byte[] encByteArray = encoding.GetBytes(encryptedStr);

                    Console.WriteLine("Writing result...");
                    Console.WriteLine("New chunk size: " + encByteArray.Length + " compared to S2S: " + Subchunk2Size);
                    Samples = new byte[encByteArray.Length];
                    Subchunk2Size = encByteArray.Length;
                    for (int i = 0; i < Subchunk2Size; i++)
                    {
                        if (i % (Subchunk2Size / 10) == 0)
                            Console.WriteLine("Progress: " + i / (Subchunk2Size / 10) + "/10");
                        
                        // write result
                        byte sample;
                        if (i < encByteArray.Length)
                            sample = encByteArray[i];
                        else sample = Samples[i];

                        Samples[i] = sample;
                        bw.Write(sample);

                    }
                    for (int i = 0; i < 20; i++)
                    {
                        Console.WriteLine("Po: " + Samples[i]);
                    }
                }
            }
        }

        long IntPow(long x, long pow)
        {
            long ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
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
                case Channel.Both:
                        return true;
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
