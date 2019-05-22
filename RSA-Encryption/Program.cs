using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;

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

            // ----- RSA ALGORITHM -----
            Console.WriteLine("---------------");

            // generate large prime numbers (p, q)
            int bitsLength = 64;
            PrimeGenerator primeGenerator = new PrimeGenerator();
            BigInteger p = primeGenerator.generatePrimeNumber(bitsLength / 8); // 1 byte = 8 bits
            Console.WriteLine("Generated p = " + p);
            BigInteger q = primeGenerator.generatePrimeNumber(bitsLength / 8); // 1 byte = 8 bits
            Console.WriteLine("Generated q = " + q);

            // encryption / decryption helper instance
            EncryptDecrypt ED = new EncryptDecrypt();
            ED.p = 53;
            ED.q = 59;
            ED.e = 3; // should be picked randomly
            ED.d = 2011;

            

            wavFile.EncryptRSAv2(ED, true);
            Console.WriteLine("Playing encrypted sound...");
            wavFile.Play();
            wavFile.EncryptRSAv2(ED, false);
            Console.WriteLine("Playing decrypted sound...");
            wavFile.Play();

            Console.ReadKey();
        }
    }
}
