using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyser
{
    class Program
    {
        static void Main(string[] args)
        {
            DataContainer dataContainer = new DataContainer(TimeSpan.FromSeconds(1));
            dataContainer.ReadFromRootPath(@"D:\무나논문작업\04.기초데이터");

            string resultRootPath = @"D:\무나논문작업\05.분석\02.수행시간";
            DirectoryInfo directoryInfo = new DirectoryInfo(resultRootPath);

            string subDirectoryName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            directoryInfo.CreateSubdirectory(subDirectoryName);

            //UserPathAndTime userPathAndTime = new UserPathAndTime(dataContainer);
            //userPathAndTime.Calculate(150, 20);
            //userPathAndTime.save(Path.Combine(resultRootPath, subDirectoryName, "userTime.xlsx"));
            //userPathAndTime.saveDetail(Path.Combine(resultRootPath, subDirectoryName, "userTimeDetail.xlsx"));


            HeatMap heatMap = new HeatMap(dataContainer, Path.Combine(resultRootPath, subDirectoryName));
            //heatMap.Create(10, 35, 1920, 1080);
            heatMap.Create(10, 35, 1920, 1080, new Dictionary<string, List<DataId>>()
            {
                {
                    "기술계층-오답",
                    new List<DataId>()
                    {
                        new DataId(){ UserName = "02", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "02", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "02", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "03", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "03", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "04", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "04", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "04", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "05", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "05", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "05", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "06", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "07", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "07", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "07", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "08", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "10", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "10", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "10", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "11", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "11", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "11", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "12", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "12", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "12", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "16", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "16", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "17", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "17", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "17", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "18", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "18", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "18", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "19", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "19", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "19", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "20", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "20", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "21", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "22", QuestionName = "11", SectionName = "3" },
                    }
                },
                {
                    "기술계층-정답",
                    new List<DataId>()
                    {
                        new DataId(){ UserName = "01", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "01", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "01", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "09", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "09", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "09", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "13", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "13", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "13", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "14", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "14", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "14", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "15", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "15", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "16", QuestionName = "10", SectionName = "3" },
                        new DataId(){ UserName = "21", QuestionName = "11", SectionName = "3" },
                        new DataId(){ UserName = "21", QuestionName = "12", SectionName = "3" },
                        new DataId(){ UserName = "22", QuestionName = "10", SectionName = "3" },
                    }
                },
                {
                    "주제검색-오답",
                    new List<DataId>()
                    {
                        new DataId(){ UserName = "01", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "01", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "02", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "02", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "02", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "03", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "03", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "03", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "04", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "04", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "04", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "05", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "05", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "05", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "06", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "07", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "07", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "09", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "09", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "09", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "10", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "10", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "11", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "11", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "11", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "13", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "13", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "13", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "15", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "16", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "16", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "17", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "17", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "18", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "19", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "19", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "19", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "20", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "20", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "21", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "21", QuestionName = "14", SectionName = "3" },
                    }
                },
                {
                    "주제검색-정답",
                    new List<DataId>()
                    {
                        new DataId(){ UserName = "01", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "06", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "06", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "07", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "08", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "08", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "12", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "12", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "12", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "14", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "14", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "14", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "16", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "18", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "18", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "20", QuestionName = "15", SectionName = "3" },
                        new DataId(){ UserName = "22", QuestionName = "13", SectionName = "3" },
                        new DataId(){ UserName = "22", QuestionName = "14", SectionName = "3" },
                        new DataId(){ UserName = "22", QuestionName = "15", SectionName = "3" },
                    }
                }
            });

            Console.WriteLine("aaaaaaaaaa");
        }
    }
}

