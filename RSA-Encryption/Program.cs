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
            wavFile.EditWavFile(2, 10, Channel.Right);
            wavFile.Play();
            Console.WriteLine(wavFile.GetSeconds());
            Console.ReadKey();
        }
    }
}
