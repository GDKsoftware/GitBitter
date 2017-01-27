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
    }
}
