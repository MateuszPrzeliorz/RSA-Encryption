using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace RSA_Encryption
{
    class RSA
    {
        public static BigInteger Square(BigInteger s)
        {
            return (s * s);
        }

        public static BigInteger Mod(BigInteger a, BigInteger b, BigInteger c)
        {
            if (b == 0)
            {
                return 1;
            }
            else if (b % 2 == 0)
            {
                var d = Mod(a, b / 2, c);
                return (d * d) % c;
            }
            else
            {
                return ((a % c) * Mod(a, b - 1, c)) % c;
            }
        }

        public static BigInteger N_val(BigInteger parameter1, BigInteger paramater2)
        {
            return (parameter1 * paramater2);
        }

        public static BigInteger PHI_val(BigInteger p1, BigInteger p2)
        {
            return ((p1-1) * (p2-1));
        }
    }
}
