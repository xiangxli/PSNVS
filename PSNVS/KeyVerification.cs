using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSNVS
{
    public static class KeyVerification
    {
        static byte KEY_GOOD = 0;
        static byte KEY_INVALID = 1;
        static byte KEY_BLACKLISTED = 2;
        static byte KEY_PHONY = 3;

        static string[] BL = new string[]
        {
            "11111111"
        };

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
                return (byte)(((seed >> a) & 0x000000FF) ^ ((seed >> b) & c));
            }
        }

        private static string PKV_GetChecksum(String s)
        {
            UInt16 left = 0x0056;
            UInt16 right = 0x00AF;
            UInt16 sum;

            if (s.Length > 0)
            {
                for (int i = 0; i < s.Length; i++)
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

        public static bool PKV_CheckKeyChecksum(string key)
        { 
            string s = key.Replace("-", "");
            if (s.Length != 20) return false;

            string c = s.Substring(16, 4);

            return c == PKV_GetChecksum(s.Substring(0, 16));
        }

        public static byte PKV_CheckKey(string s)
        {
            string key;
            string kb;
            Int64 seed;

            if (!PKV_CheckKeyChecksum(s))
                return KEY_INVALID;

            key = s.Replace("-", "").ToUpper();

            if (BL.Length > 0)
            {
                foreach (var b in BL)
                {
                    if (key.StartsWith(b))
                        return KEY_BLACKLISTED;
                }
            }

            if (!Int64.TryParse(key.Substring(0, 8), System.Globalization.NumberStyles.HexNumber, null, out seed))
            {
                return KEY_PHONY;
            }

            kb = key.Substring(8, 2);
            byte by = PKV_GetKeyByte(seed, 24, 3, 200);
            if (kb != string.Format("{0:X2}", by))
            {
                return KEY_PHONY;
            }

            kb = key.Substring(10, 2);
            by = PKV_GetKeyByte(seed, 10, 0, 56);
            if (kb != string.Format("{0:X2}", by))
            {
                return KEY_PHONY;
            }

            kb = key.Substring(12, 2);
            by = PKV_GetKeyByte(seed, 1, 2, 91);
            if (kb != string.Format("{0:X2}", by))
            {
                return KEY_PHONY;
            }

            kb = key.Substring(14, 2);
            by = PKV_GetKeyByte(seed, 7, 1, 100);
            if (kb != string.Format("{0:X2}", by))
            {
                return KEY_PHONY;
            }

            return KEY_GOOD;
        }

    }
}
