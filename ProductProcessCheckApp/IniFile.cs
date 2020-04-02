using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ProductProcessCheckApp
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    class IniFile
    {
        string Path;
        string AppName = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string FileName = null)
        {
            // This will get the current WORKING directory (i.e. \bin\Debug)
            //string workingDirectory = System.Environment.CurrentDirectory;
            //string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;

            string projectPath = System.IO.Path.GetFullPath(@"..\..\");
            Path = projectPath + FileName ?? (AppName + ".ini");
            //Path = new FileInfo(IniPath ?? AppName + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? AppName, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? AppName, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? AppName);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? AppName);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}
