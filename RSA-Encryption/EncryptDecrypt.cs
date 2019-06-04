using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace RSA_Encryption
{
    public class EncryptDecrypt
    {
         public BigInteger p;
         public BigInteger q;
         public BigInteger e;
         public BigInteger d;
         public BigInteger n;
         string audioFile = "";

        public string Encryption(string message)
        {
            Console.WriteLine("| e = " + e);
            Console.WriteLine("| n = " + n);

            char[] soundCharArray = message.ToCharArray();
            String tmp = "";
            String uncrypted = "";

            Console.WriteLine("Total sound length: " + soundCharArray.Length);

            for (int i = 0; i < soundCharArray.Length; i++)
            {
                uncrypted += soundCharArray[i];
                if (i % (soundCharArray.Length / 10) == 0)
                    Console.WriteLine("Progress: " + i / (soundCharArray.Length / 10) + "/10");

                if (tmp == "")
                {
                    tmp += RSA.Mod(soundCharArray[i], e, n);
                }
                else
                {
                    tmp += "-" + RSA.Mod(soundCharArray[i], e, n);
                    if (i == 50)
                    {
                        Console.WriteLine("The first 50 not encrypted samples:" +  uncrypted);
                        Console.WriteLine("The first 50 encrypted samples:" +  tmp);
                    }
                }
            }
            return tmp;
        }

        public string Decryption(String message)
        {
            Console.WriteLine("| d = " + d);
            Console.WriteLine("| n = " + n);

            char[] soundCharArray = message.ToCharArray();
            string tempStr = "";
            string decryptedStr = "";
            int j = 0;

            Console.WriteLine("Total sound length: " + soundCharArray.Length);
            try
            {
                Console.WriteLine("----- vs length: " + soundCharArray.Length);
                for (int i = 0; i < soundCharArray.Length; i++)
                {
                    if (i % (soundCharArray.Length / 100) == 0)
                        Console.WriteLine("Progress: " + i / (soundCharArray.Length / 100) + "/100");

                    tempStr = "";

                    for (j = i; soundCharArray[j] != '-'; j++)
                    {
                        tempStr = tempStr + soundCharArray[j];
                    }
                    i = j;
                    decryptedStr = decryptedStr + ((char)RSA.Mod(BigInteger.Parse(tempStr), d, n)).ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot decode this sound!", ex);
            }
            return decryptedStr;
        }

        public string EncryptStr()
        {
            Console.WriteLine("Calculating n value...");
            n = RSA.N_val(p, q);
            string tmp = Encryption(audioFile);
            return tmp;
        }

        public string DecryptStr()
        {
            Console.WriteLine("Calculating n value...");
            n = RSA.N_val(p, q);
            string tmp = Decryption(audioFile);
            return tmp;
        }

        public void LoadWavFile(string message)
        {
            audioFile = message;
        }

    }
}
