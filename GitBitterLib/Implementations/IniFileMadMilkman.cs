namespace GitBitterLib
{
    using System;

    public class IniFileMadMilkman : IIniFile
    {
        private MadMilkman.Ini.IniFile ini = null;
        private string inifilepath = null;

        public IniFileMadMilkman()
        {
        }

        public string IniReadValue(string section, string key)
        {
            try
            {
                return ini.Sections[section].Keys[key].Value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void IniWriteValue(string section, string key, string value)
        {
            if (!ini.Sections.Contains(section))
            {
                ini.Sections.Add(section);
            }

            if (!ini.Sections[section].Keys.Contains(key))
            {
                ini.Sections[section].Keys.Add(key);
            }

            ini.Sections[section].Keys[key].Value = value;

            ini.Save(this.inifilepath);
        }

        public void SetFile(string filepath)
        {
            this.inifilepath = filepath;

            ini = new MadMilkman.Ini.IniFile();
            ini.Load(filepath);
        }
    }
}
