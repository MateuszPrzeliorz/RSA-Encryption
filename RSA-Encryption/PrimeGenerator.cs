using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;

namespace RSA_Encryption
{
    class PrimeGenerator
    {
        // checks if number is prime
        // n: int (number to test)
        // k: int (number of tests to do)
        public bool isNumberPrime(BigInteger n, BigInteger k)
        {
            // check if n is not even (except 2)
            if (n == 2 || n == 3)
                return true;
            if (n <= 1 || n % 2 == 0)
                return false;

            // find r and s
            BigInteger s = 0;
            BigInteger r = n - 1;
            BigInteger bitwised = r & 1;
            while(bitwised == 0) // #checkme
            {
                s += 1;
                r /= 2; // floor division
                bitwised = r & 1;
            }
            for(int i=0; i<k; i++)
            {
                using (var rng = RandomNumberGenerator.Create())
                {
                    BigInteger a = RandomInRange(rng, 2, n - 1);
                    BigInteger x = RSA.Mod(a, r, n);
                    if (x != 1 && x != (n-1))
                    {
                        BigInteger j = 1;
                        while (j<s && x != (n - 1))
                        {
                            x = RSA.Mod(x, 2, n);
                            if (x == 1)
                                return false;
                            j += 1;
                        }
                        if (x != (n - 1))
                            return false;
                    }
                }
            }
            return true;
        }

        // length: int (the length of the number to generate, in bits)
        public BigInteger generatePrimeCandidate(int length)
        {
            // generate random bits
            Random rnd = new Random();
            byte[] b = new byte[length];
            rnd.NextBytes(b);

            BigInteger p = new BigInteger(b);

            // apply a mask to set MSB and LSB to 1
            p |= (1 << length - 1) | 1;
            return p;
        }

        public BigInteger generatePrimeNumber(int length)
        {
            BigInteger p = 4;
            while (!isNumberPrime(p, 128))
            {
                p = generatePrimeCandidate(length);
            }

            return p;
        }

        // adapted from https://stackoverflow.com/questions/17357760/how-can-i-generate-a-random-biginteger-within-a-certain-range
        public static BigInteger RandomInRange(RandomNumberGenerator rng, BigInteger min, BigInteger max)
        {
            if (min > max)
            {
                var buff = min;
                min = max;
                max = buff;
            }

            // offset to set min = 0
            BigInteger offset = -min;
            min = 0;
            max += offset;

            var value = randomInRangeFromZeroToPositive(rng, max) - offset;
            return value;
        }

        private static BigInteger randomInRangeFromZeroToPositive(RandomNumberGenerator rng, BigInteger max)
        {
            BigInteger value;
            var bytes = max.ToByteArray();

            // count how many bits of the most significant byte are 0
            // NOTE: sign bit is always 0 because `max` must always be positive
            byte zeroBitsMask = 0b00000000;

            var mostSignificantByte = bytes[bytes.Length - 1];

            // we try to set to 0 as many bits as there are in the most significant byte, starting from the left (most significant bits first)
            // NOTE: `i` starts from 7 because the sign bit is always 0
            for (var i = 7; i >= 0; i--)
            {
                // we keep iterating until we find the most significant non-0 bit
                if ((mostSignificantByte & (0b1 << i)) != 0)
                {
                    var zeroBits = 7 - i;
                    zeroBitsMask = (byte)(0b11111111 >> zeroBits);
                    break;
                }
            }

            do
            {
                rng.GetBytes(bytes);

                // set most significant bits to 0 (because `value > max` if any of these bits is 1)
                bytes[bytes.Length - 1] &= zeroBitsMask;

                value = new BigInteger(bytes);

                // `value > max` 50% of the times, in which case the fastest way to keep the distribution uniform is to try again
            } while (value > max);

            return value;
        }

        public bool checkIfPrime(int tmp)
        {
            if (tmp < 2)
            {
                return false;
            }
            if (tmp % 2 == 0)
            {
                return tmp == 2;
            }

            int sqrt = (int)Math.Sqrt((double)tmp);

            for (int i = 3; i <= sqrt; i += 2)
            {
                if (tmp % i == 0)
                    return false;

            }
            return true;
        }

        public BigInteger getPhi(BigInteger i, BigInteger j)
        {
            return (i - 1) * (j - 1);
        }

        public BigInteger getE(BigInteger phi, BigInteger n)
        {
            Random rnd = new Random();

            // e value should be smaller than phi
            double phiDouble = double.MaxValue;
            if (phi < 2000000000)
                phiDouble = (int)phi;
            else
                phiDouble = 2000000000;
            double upperBorder = Math.Log((phiDouble-1), 2);

            int r = rnd.Next(1, (int)upperBorder);
            BigInteger e = BigInteger.Pow(2, r) + 1;

            // e value should be coprime to n and phi
            while (!(IsCoprime(e, phi) && IsCoprime(e, n)))
            {
                r = rnd.Next(1, (int)upperBorder);
                e = BigInteger.Pow(2, r) + 1;
            }
            return e;
        }

        public BigInteger getD(BigInteger e, BigInteger phi)
        {
            BigInteger d = modInverse(e, phi);
            return d;
        }

        BigInteger modInverse(BigInteger a, BigInteger n)
        {
            BigInteger i = n;
            BigInteger v = 0;
            BigInteger d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }

        public BigInteger GetGCDByModulus(BigInteger value1, BigInteger value2)
        {
            while (value1 != 0 && value2 != 0)
            {
                if (value1 > value2)
                    value1 %= value2;
                else
                    value2 %= value1;
            }
            return (value1 > value2) ? value1 : value2;
        }

        public bool IsCoprime(BigInteger value1, BigInteger value2)
        {
            return GetGCDByModulus(value1, value2) == 1;
        }

        public byte[] HexDecode(string tmp)
        {
            string[] _string = tmp.Split('-');
            byte[] array = new byte[_string.Length];

            for (int i = 0; i < _string.Length; i++)
            {
                array[i] = Convert.ToByte(_string[i], 16);

            }
            return array;
        }
    }
}
