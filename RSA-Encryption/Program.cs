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
            int bitsLength = 16; // *OF P & Q -----> N will be 2 times bigger!*
            PrimeGenerator primeGenerator = new PrimeGenerator();
            BigInteger p = primeGenerator.generatePrimeNumber(bitsLength / 8); // 1 byte = 8 bits
            Console.WriteLine("Generated p = " + p);
            BigInteger q = primeGenerator.generatePrimeNumber(bitsLength / 8); // 1 byte = 8 bits
            Console.WriteLine("Generated q = " + q);
            BigInteger phi = primeGenerator.getPhi(p, q);
            Console.WriteLine("Calculated Phi = " + phi);
            BigInteger e = primeGenerator.getE(phi, p*q);
            Console.WriteLine("Generated e = " + e);
            BigInteger d = primeGenerator.getD(e, phi);
            Console.WriteLine("Generated d = " + d);

            // encryption / decryption helper instance
            EncryptDecrypt ED = new EncryptDecrypt();
            ED.p = p;
            ED.q = q;
            ED.e = e;
            ED.d = d;

            wavFile.EncryptRSAv2(ED, true);
            Console.WriteLine("Ready to play encrypted sound... | Press random key to play!");
            Console.ReadKey();
            wavFile.Play();

            wavFile.EncryptRSAv2(ED, false);
            Console.WriteLine("Ready to play decrypted sound... | Press random key to play!");
            Console.ReadKey();
            wavFile.Play();

            Console.ReadKey();
        }
    }
}
