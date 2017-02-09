namespace GitBitterLib
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    public static class SecureStringHelper
    {
        public static string ToInSecureString(this SecureString s)
        {
            IntPtr p = IntPtr.Zero;
            try
            {
                p = Marshal.SecureStringToCoTaskMemUnicode(s);
                return Marshal.PtrToStringUni(p);
            }
            finally
            {
                if (p != IntPtr.Zero)
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(p);
                }
            }
        }

        public static SecureString PtrToSecureString(IntPtr p, int length)
        {
            SecureString s = new SecureString();
            for (var i = 0; i < length; i++)
            {
                s.AppendChar((char)Marshal.ReadInt16(p, i * sizeof(short)));
            }

            s.MakeReadOnly();
            return s;
        }

        public static SecureString PtrToSecureString(IntPtr p)
        {
            SecureString s = new SecureString();
            int i = 0;
            while (true)
            {
                char c = (char)Marshal.ReadInt16(p, ((i++) * sizeof(short)));
                if (c == '\u0000')
                {
                    break;
                }

                s.AppendChar(c);
            }

            s.MakeReadOnly();
            return s;
        }

        public static SecureString InsecureToSecureString(this string insec)
        {
            SecureString sec = new SecureString();
            for (var i = 0; i < insec.Length; i++)
            {
                sec.AppendChar(insec[i]);
            }

            sec.MakeReadOnly();
            return sec;
        }
    }
}
