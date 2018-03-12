namespace GitBitterLib
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// https://gist.github.com/meziantou/10311113
    /// Edited to use SecureString where possible, removed enumerate
    /// </summary>
    public class CredentialManagerWindows : ICredentialManager
    {
        private static readonly int MaximumCredentialBlobSize = 100;
        
        private enum CredentialPersistence : uint
        {
            Session = 1,
            LocalMachine,
            Enterprise
        }

        public Credential ReadCredential(string applicationName)
        {
            IntPtr credPtr;
            bool read = CredRead(applicationName, CredentialType.Generic, 0, out credPtr);
            if (read)
            {
                using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(credPtr))
                {
                    CREDENTIAL cred = critCred.GetCredential();
                    return ReadCredential(cred);
                }
            }

            throw new NoCredentialsSetForApplication(applicationName);
        }

        public Credential ReadCredentialOrNull(string applicationName)
        {
            try
            {
                return ReadCredential(applicationName);
            }
            catch (NoCredentialsSetForApplication)
            {
                return null;
            }
        }

        public int WriteCredential(string applicationName, SecureString userName, SecureString password)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("key is null or empty");
            }

            if (userName == null || userName.Length == 0)
            {
                throw new ArgumentNullException("userName is null or empty");
            }

            if (password == null || password.Length == 0)
            {
                throw new ArgumentNullException("password is null or empty");
            }

            string passwordAsstring = password.ToInSecureString();
            byte[] byteArray = Encoding.Unicode.GetBytes(passwordAsstring);

            if (byteArray.Length > MaximumCredentialBlobSize)
            {
                throw new ArgumentOutOfRangeException(string.Format("The password message has exceeded {0} bytes.", MaximumCredentialBlobSize));
            }

            CREDENTIAL cred = new CREDENTIAL();
            cred.TargetName = System.Runtime.InteropServices.Marshal.StringToCoTaskMemUni(applicationName);
            cred.CredentialBlob = System.Runtime.InteropServices.Marshal.StringToCoTaskMemUni(passwordAsstring);
            cred.CredentialBlobSize = (uint)Encoding.Unicode.GetBytes(passwordAsstring).Length;
            cred.AttributeCount = 0;
            cred.Attributes = IntPtr.Zero;
            cred.Comment = IntPtr.Zero;
            cred.TargetAlias = System.Runtime.InteropServices.Marshal.StringToCoTaskMemUni(applicationName);
            cred.Type = CredentialType.Generic;
            cred.Persist = CredentialPersistence.LocalMachine;
            cred.UserName = System.Runtime.InteropServices.Marshal.StringToCoTaskMemUni(userName.ToInSecureString());

            bool written = CredWrite(ref cred, 0);
            int lastError = Marshal.GetLastWin32Error();

            if (!written)
            {
                return lastError;
            }

            return 0;
        }

        private static Credential ReadCredential(CREDENTIAL credential)
        {
            string applicationName = Marshal.PtrToStringUni(credential.TargetName);
            string userName = Marshal.PtrToStringUni(credential.UserName);

            var secret = SecureStringHelper.PtrToSecureString(credential.CredentialBlob, (int)credential.CredentialBlobSize / 2);

            return new Credential(credential.Type, applicationName, userName, secret);
        }

        public void ResetCredential(string applicationName)
        {
            if (!CredDelete(applicationName, CredentialType.Generic, 0))
            {
                int lastError = Marshal.GetLastWin32Error();

                throw new Exception("Error trying to reset credentials for " + applicationName + " (Errorcode: " + lastError + ")");
            }
        }

        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr);

        [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredWrite([In] ref CREDENTIAL userCredential, [In] uint flags);

        [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
        private static extern bool CredFree([In] IntPtr cred);

        [DllImport("Advapi32.dll", EntryPoint = "CredDelete", SetLastError = true)]
        private static extern bool CredDelete(string target, CredentialType type, int reservedFlag);
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CREDENTIAL
        {
            public uint Flags;
            public CredentialType Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public CredentialPersistence Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }

        private sealed class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
        {
            public CriticalCredentialHandle(IntPtr preexistingHandle)
            {
                SetHandle(preexistingHandle);
            }

            public CREDENTIAL GetCredential()
            {
                if (!IsInvalid)
                {
                    CREDENTIAL credential = (CREDENTIAL)Marshal.PtrToStructure(handle, typeof(CREDENTIAL));
                    return credential;
                }

                throw new InvalidOperationException("Invalid CriticalHandle!");
            }

            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    CredFree(handle);
                    SetHandleAsInvalid();
                    return true;
                }

                return false;
            }
        }
    }
}