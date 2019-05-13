using System;
using System.IO;

namespace RSA_Encryption
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "example.wav";
            string fileCopy = "example_copy.wav";

            WavFile wavFile = new WavFile(fileName, fileCopy);
            wavFile.ReadWav();
            //wavFile.EditPartOfFile(2, 6, Channel.Left);
            //wavFile.Play();
            wavFile.EncryptXOR();
            wavFile.Play();
            wavFile.EncryptXOR();
            wavFile.Play();
            Console.ReadKey();
        }
    }
}
