using common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyser
{
    public class DataContainer
    {
        public DataContainer(TimeSpan restTimePerEvent)
        {
            RestTimePerEvent = restTimePerEvent;
        }

        public TimeSpan RestTimePerEvent { get; private set; }

        /// <summary>
        /// UserName, QuestionName, SectionName
        /// </summary>
        public Dictionary<string, Dictionary<string, Dictionary<string, MouseTraceData>>> MouseTraces { get; private set; } = new Dictionary<string, Dictionary<string, Dictionary<string, MouseTraceData>>>();
        

        /// <summary>
        /// UserName, QuestionName, SectionName
        /// </summary>
        public Dictionary<string, Dictionary<string, Dictionary<string, TimeInformation>>> TimeInformations { get; private set; } = new Dictionary<string, Dictionary<string, Dictionary<string, TimeInformation>>>();

        private void readFromMouseTraceRecordFiles(string userName, FileInfo[] files)
        {
            MouseTraces[userName] = new Dictionary<string, Dictionary<string, MouseTraceData>>();
            foreach(var file in files)
            {
                string[] fileNameSplit = file.Name.Split('.')[0].Split('-');
                string questionName = fileNameSplit[0];
                string sectionName = fileNameSplit[1];

                if (!MouseTraces[userName].ContainsKey(questionName))
                {
                    MouseTraces[userName][questionName] = new Dictionary<string, MouseTraceData>();
                }
                MouseTraces[userName][questionName][sectionName] = new MouseTraceData(MouseEventPoint.Parse(file.FullName), RestTimePerEvent);
            }
        }

        private void readFromTimeInformationFiles(string userName, FileInfo[] files)
        {
            TimeInformations[userName] = new Dictionary<string, Dictionary<string, TimeInformation>>();
            foreach (var file in files)
            {
                string[] fileNameSplit = file.Name.Split('.')[0].Split('-');
                string questionName = fileNameSplit[0];
                string sectionName = fileNameSplit[1];

                if (!TimeInformations[userName].ContainsKey(questionName))
                {
                    TimeInformations[userName][questionName] = new Dictionary<string, TimeInformation>();
                }
                TimeInformations[userName][questionName][sectionName] = TimeInformation.ParseFromTi(file.FullName);
            }
        }

        public void ReadFromRootPath(string rootPath)
        {
            DirectoryInfo di = new DirectoryInfo(rootPath);
            Console.WriteLine($"Start total user count is {di.GetDirectories().Length}.");

            foreach (var dir in di.GetDirectories())
            {
                Console.WriteLine($"Start {dir.Name}.");
                readFromTimeInformationFiles(dir.Name, dir.GetFiles("*.ti"));
                readFromMouseTraceRecordFiles(dir.Name, dir.GetFiles("*.mtr"));
                Console.WriteLine($"Done {dir.Name}.");
            }

            Console.WriteLine($"All Done.");
        }
    }
}
