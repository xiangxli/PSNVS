using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSNVS
{
    public static class KeyGeneration
    {

        private static byte PKV_GetKeyByte(Int64 seed, byte a, byte b, byte c)
        {
            a = (byte)(a % 25);
            b = (byte)(b % 3);

            if (a % 2 == 0)
            {
                return (byte)(((seed >> a) & 0x000000FF) ^ ((seed >> b) | c));
            }
            else
            {
                return  (byte)(((seed >> a) & 0x000000FF) ^ ((seed >> b) & c));
            }
        }

        private static string PKV_GetChecksum(String s)
        {
            UInt16 left = 0x0056;
            UInt16 right = 0x00AF;
            UInt16 sum;

            if (s.Length > 0)
            { 
                for (int i =0; i < s.Length; i++)
                {
                    right += (UInt16)(Convert.ToInt32(s[i]));
                    if (right > 0x00FF)
                    {
                        right -= 0x00FF;
                    }
                    left += right;
                    if (left > 0x00FF)
                    {
                        left -= 0x00FF;
                    }
                }
            }
            sum = (UInt16)((left << 8) + right);
            return String.Format("{0:X4}", sum);
        }

        public static string PKV_MakeKey(Int64 seed)
        {
            byte[] keyBytes = new byte[4];
            keyBytes[0] = PKV_GetKeyByte(seed, 24, 3, 200);
            keyBytes[1] = PKV_GetKeyByte(seed, 10, 0, 56);
            keyBytes[2] = PKV_GetKeyByte(seed, 1, 2, 91);
            keyBytes[3] = PKV_GetKeyByte(seed, 7, 1, 100);

            StringBuilder sb= new StringBuilder();
            sb.Append(String.Format("{0:X}", seed));

            for (int i = 0; i < 4; i++)
            {
                sb.Append(String.Format("{0:X2}",keyBytes[i]));
            }

            string checksum = PKV_GetChecksum(sb.ToString());
            sb.Append(checksum);

            int len = sb.Length - 4;
            while (len > 1)
            {
                sb.Insert(len, '-');
                len -= 4;
            }

            return sb.ToString();
        }

    }
}
