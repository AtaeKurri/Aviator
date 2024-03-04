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

        private string cBSOutputPath;
        public string CBSOutputPath
        {
            get => cBSOutputPath;
            set
            {
                cBSOutputPath = value;
                RaisePropertyChanged("CBSOutputPath");
            }
        }

        private string modVersion;
        public string ModVersion
        {
            get => modVersion;
            set
            {
                modVersion = value;
                RaisePropertyChanged("ModVersion");
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
            data.Sections.AddSection("Common");
            data["Common"].AddKey("CompileTarget", $"{(int)ECompileTarget.LuaSTG}");
            data["Common"].AddKey("ProjectInternalName", "");
            
            data.Sections.AddSection("LSTG");
            data["LSTG"].AddKey("LuaSTGExecutablePath", "");
            data["LSTG"].AddKey("ModVersion", "4096");

            data.Sections.AddSection("CBS");
            data["CBS"].AddKey("CBSOutputPath", "");

            parser.WriteFile(PathToConfiguration, data);
        }

        public void WriteConfiguration()
        {
            FileIniDataParser parser = new();
            IniData data = parser.ReadFile(PathToConfiguration);

            data["Common"]["CompileTarget"] = $"{(int)CompileTarget}";
            data["Common"]["ProjectInternalName"] = ProjectInternalName;
            data["LSTG"]["LuaSTGExecutablePath"] = LuaSTGExecutablePath;
            data["LSTG"]["ModVersion"] = ModVersion;
            data["CBS"]["CBSOutputPath"] = CBSOutputPath;

            parser.WriteFile(PathToConfiguration, data);
        }

        public void ReadConfiguration()
        {
            FileIniDataParser parser = new();
            IniData data = parser.ReadFile(PathToConfiguration);

            CompileTarget = (ECompileTarget)int.Parse(data["Common"]["CompileTarget"]);
            ProjectInternalName = data["Common"]["ProjectInternalName"];
            LuaSTGExecutablePath = data["LSTG"]["LuaSTGExecutablePath"];
            ModVersion = data["LSTG"]["ModVersion"];
            CBSOutputPath = data["CBS"]["CBSOutputPath"];
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}