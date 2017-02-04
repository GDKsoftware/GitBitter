namespace GitBitterLib
{
    public interface IIniFile
    {
        void SetFile(string filepath);

        string IniReadValue(string section, string key);

        void IniWriteValue(string section, string key, string value);
    }
}
