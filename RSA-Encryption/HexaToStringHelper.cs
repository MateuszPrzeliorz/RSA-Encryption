using System;
using System.Collections.Generic;
using System.Text;

namespace RSA_Encryption
{
    public static class HexaToStringHelper
    {
        public static string ConvertHex(this string str)
        {
            string ascii = string.Empty;

            for (int i = 0; i < str.Length; i += 2)
            {
                String hs = string.Empty;

                hs = str.Substring(i,2);
                uint decval = Convert.ToUInt32(hs, 16);
                char character = Convert.ToChar(decval);
                ascii += character;
            }

        return ascii;
        }
    }
}
