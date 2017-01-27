namespace GitBitterLib
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Create a New INI file to store or load data
    /// source: https://www.codeproject.com/Articles/1966/An-INI-file-handling-class-using-C
    /// </summary>
    public class IniFile
    {
        public string Path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="inipath"></PARAM>
        public IniFile(string inipath)
        {
            Path = inipath;
        }

        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="section"></PARAM>
        /// Section name
        /// <PARAM name="key"></PARAM>
        /// Key Name
        /// <PARAM name="value"></PARAM>
        /// Value Name
        public void IniWriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.Path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="section"></PARAM>
        /// <PARAM name="key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, string.Empty, temp, 255, this.Path);
            return temp.ToString();
        }
    }
}
