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
            heatMap.Create(10, 35, 1920, 1080);

            Console.WriteLine("aaaaaaaaaa");
        }
    }
}
