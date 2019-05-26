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
         string loadAudio = "";
         string cipher = "";

        private PrimeGenerator primeGenerator;

        public string Encryption(string message)
        {
            Console.WriteLine("| e = " + e);
            Console.WriteLine("| n = " + n);
            string hex = message;
            char[] vs = hex.ToCharArray();
            String tmp = "";

            for (int i = 0; i < vs.Length; i++)
            {
                if (tmp == "")
                {
                    tmp = tmp + RSA.Mod(vs[i], e, n);
                }
                else
                {
                    tmp = tmp + "-" + RSA.Mod(vs[i], e, n);
                }
            }
            return tmp;
        }

        public string Decryption(String image)
        {
            Console.WriteLine("| d = " + d);
            Console.WriteLine("| n = " + n);
            char[] vs = image.ToCharArray();
            int i = 0;
            int j = 0;
            string tmp = "";
            string tmp2 = "";
            try
            {
                for (; i < vs.Length; i++)
                {
                    tmp = "";

                    for (j = i; vs[j] != '-'; j++)
                    {
                        tmp = tmp + vs[j];
                    }
                    i = j;

                    tmp2 = tmp2 + ((char)RSA.Mod(Convert.ToInt16(tmp), d, n)).ToString();
                }
            }
            catch (Exception ex)
            { }
            return tmp2;
        }

        public string EncryptStr()
        {
            Console.WriteLine("Calculating n value...");
            n = RSA.N_val(p, q);
            string tmp = Encryption(loadAudio);
            return tmp;
        }

        public string DecryptStr()
        {
            Console.WriteLine("Calculating n value...");
            n = RSA.N_val(p, q);
            string tmp = Decryption(loadAudio);
            return tmp;
        }

        public void LoadWavFile(string message)
        {
            loadAudio = message;
        }

    }
}
