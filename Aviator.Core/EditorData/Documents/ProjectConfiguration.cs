using Aviator.Core.EditorData.Nodes.Attributes;
using IniParser.Model;
using IniParser;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Aviator.Core.EditorData.Documents
{
    public class ProjectConfiguration : INotifyPropertyChanged
    {
        public string PathToConfiguration { get; set; }

        #region Config Values

        private string luaSTGExecutablePath;
        public string LuaSTGExecutablePath
        {
            get => luaSTGExecutablePath;
            set
            {
                luaSTGExecutablePath = value;
                RaisePropertyChanged("LuaSTGExecutablePath");
            }
        }

        private ECompileTarget compileTarget;
        public ECompileTarget CompileTarget
        {
            get => compileTarget;
            set
            {
                compileTarget = value;
                RaisePropertyChanged("CompileTarget");
            }
        }

        private string projectInternalName;
        public string ProjectInternalName
        {
            get => projectInternalName;
            set
            {
                projectInternalName = value;
                RaisePropertyChanged("ProjectInternalName");
            }
        }

        #endregion

        public ProjectConfiguration(string pathToConfiguration)
        {
            PathToConfiguration = Path.Combine(Path.GetDirectoryName(pathToConfiguration), $"{Path.GetFileNameWithoutExtension(pathToConfiguration)}.ini");
            InitConfiguration();
            ReadConfiguration();
        }

        public void InitConfiguration()
        {
            if (File.Exists(PathToConfiguration))
                return;
            FileIniDataParser parser = new();
            IniData data = new();
            data.Sections.AddSection("Cfg");
            data["Cfg"].AddKey("LuaSTGExecutablePath", "");
            data["Cfg"].AddKey("CompileTarget", $"{(int)ECompileTarget.LuaSTG}");
            data["Cfg"].AddKey("ProjectInternalName", "");

            parser.WriteFile(PathToConfiguration, data);
        }

        public void WriteConfiguration()
        {
            FileIniDataParser parser = new();
            IniData data = parser.ReadFile(PathToConfiguration);

            data["Cfg"]["LuaSTGExecutablePath"] = LuaSTGExecutablePath;
            data["Cfg"]["CompileTarget"] = $"{(int)CompileTarget}";
            data["Cfg"]["ProjectInternalName"] = ProjectInternalName;

            parser.WriteFile(PathToConfiguration, data);
        }

        public void ReadConfiguration()
        {
            FileIniDataParser parser = new();
            IniData data = parser.ReadFile(PathToConfiguration);

            LuaSTGExecutablePath = data["Cfg"]["LuaSTGExecutablePath"];
            CompileTarget = (ECompileTarget)int.Parse(data["Cfg"]["CompileTarget"]);
            ProjectInternalName = data["Cfg"]["ProjectInternalName"];
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}