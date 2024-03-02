using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml;

namespace Aviator.Core.EditorData.Documents
{
    public class CBSSolutionHandler
    {
        public string SolutionName { get; set; }
        public string ProjectName => SolutionName;
        public string FilePath { get; set; }

        public Guid SolutionGuid = Guid.NewGuid();
        public Guid ProjectGuid = Guid.NewGuid();

        public List<string> resourcesPath = [];
        public Dictionary<string, string> libraries = [];

        private XmlWriter Writer;

        public CBSSolutionHandler(string solutionName, string filePath)
        {
            SolutionName = solutionName;
            FilePath = filePath;
        }

        public void GenerateSolution()
        {
            using (StreamWriter sw = new(FilePath))
            {
                sw.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00\n" +
                    "# Visual Studio Version 17\n" +
                    "VisualStudioVersion = 17.9.34607.119\n" +
                    "MinimumVisualStudioVersion = 10.0.40219.1");
                sw.WriteLine($"Project(\"{{{ProjectGuid.ToString().ToUpper()}}}\") = \"{ProjectName}\"," +
                    $"\"{SolutionName}\\{ProjectName}.csproj\", \"{{{ProjectGuid.ToString().ToUpper()}}}\"");
                sw.WriteLine("EndProject");
                sw.WriteLine("Global");
                sw.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
                sw.WriteLine("\t\tDebug|Any CPU = Debug|Any CPU");
                sw.WriteLine("\t\tRelease|Any CPU = Release|Any CPU");
                sw.WriteLine("\tEndGlobalSection");
                sw.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
                sw.WriteLine($"\t\t{{{ProjectGuid.ToString().ToUpper()}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                sw.WriteLine($"\t\t{{{ProjectGuid.ToString().ToUpper()}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                sw.WriteLine($"\t\t{{{ProjectGuid.ToString().ToUpper()}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                sw.WriteLine($"\t\t{{{ProjectGuid.ToString().ToUpper()}}}.Release|Any CPU.Build.0 = Release|Any CPU");
                sw.WriteLine("\tEndGlobalSection");
                sw.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
                sw.WriteLine("\t\tHideSolutionNode = FALSE");
                sw.WriteLine("\tEndGlobalSection");
                sw.WriteLine("\tGlobalSection(ExtensibilityGlobals) = postSolution");
                sw.WriteLine($"\t\tSolutionGuid = {{{SolutionGuid.ToString().ToUpper()}}}");
                sw.WriteLine("\tEndGlobalSection");
                sw.WriteLine("EndGlobal");
            }
        }

        /// <summary>
        /// Starts the creation of a Project file, including the Sdk used and the PropertyGroup.
        /// </summary>
        /// <param name="writer"></param>
        public void StartProjectGen(XmlWriter writer)
        {
            Writer = writer;

            Writer.WriteStartElement("Project");
            Writer.WriteAttributeString("Sdk", "Microsoft.NET.Sdk");

            Writer.WriteStartElement("PropertyGroup");

            Writer.WriteElementString("OutputType", "WinExe");
            Writer.WriteElementString("TargetFramework", "net8.0");
            Writer.WriteElementString("ImplicitUsings", "enable");
            Writer.WriteElementString("Nullable", "enable");
            Writer.WriteElementString("StartupObject", "");
            Writer.WriteElementString("AllowUnsafeBlocks", "True");
            Writer.WriteElementString("BaseOutputPath", @"..\bin");

            Writer.WriteEndElement();
        }

        /// <summary>
        /// Adds a "Resource" with a relative (to the project) file path.<br/>
        /// The <paramref name="filePath"/> <c>MUST</c> be relative to the *.avtr project file.
        /// </summary>
        /// <param name="filePath">the path to the resource file.</param>
        public void AddFileResource(string filePath)
        {
            resourcesPath.Add(filePath);
        }

        public void AddLibrary(string name, string version)
        {
            libraries.Add(name, version);
        }

        /// <summary>
        /// Write all the necessary xml nodes to load the files added by <see cref="AddFileResource(string)"/><br/>
        /// Must be called AFTER adding all resource.
        /// </summary>
        public void WriteFileResources()
        {
            // Remove strings
            Writer.WriteStartElement("ItemGroup");
            foreach (string path in resourcesPath)
            {
                Writer.WriteStartElement("None");
                Writer.WriteAttributeString("Remove", path);
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();

            // Include and copy content for building
            Writer.WriteStartElement("ItemGroup");
            foreach (string path in resourcesPath)
            {
                Writer.WriteStartElement("Content");
                Writer.WriteAttributeString("Include", path);
                Writer.WriteElementString("CopyToOutputDirectory", "PreserveNewest");
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
        }

        /// <summary>
        /// Write all the libraries xml nodes of the libraries added by <see cref="AddLibrary(string, string)"/><br/>
        /// Must be called AFTER adding all libraries.
        /// </summary>
        public void WriteLibraries()
        {
            // Write libraries
            Writer.WriteStartElement("ItemGroup");
            foreach (KeyValuePair<string, string> library in libraries)
            {
                Writer.WriteStartElement("PackageReference");
                Writer.WriteAttributeString("Include", library.Key);
                Writer.WriteAttributeString("Version", library.Value);
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();

            // Write target
            Writer.WriteStartElement("Target");
            Writer.WriteAttributeString("Name", "RestoreDotnetTools");
            Writer.WriteAttributeString("BeforeTargets", "Restore");

            Writer.WriteStartElement("Message");
            Writer.WriteAttributeString("Text", "Restoring dotnet tools");
            Writer.WriteAttributeString("Importance", "High");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Exec");
            Writer.WriteAttributeString("Command", "dotnet tool restore");
            Writer.WriteEndElement();

            Writer.WriteEndElement();
        }

        /// <summary>
        /// Ends the creation of a Project file. <c>MUST</c> be called after finished.
        /// </summary>
        /// <param name="writer"></param>
        public void EndProjectGen()
        {
            Writer.WriteEndDocument();
        }

        public void GenerateEntryPoint()
        {

        }
    }
}
