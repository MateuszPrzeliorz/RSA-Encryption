using System;
using System.IO;

namespace RSA_Encryption
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "example.wav";

            WavFile wavFile = new WavFile(fileName);
            wavFile.ReadWav();
            for (int i = 0; i < wavFile.Left.GetLength(0); i++)
            {
                Console.WriteLine(wavFile.Left[i]);
            }
        }
    }
}
