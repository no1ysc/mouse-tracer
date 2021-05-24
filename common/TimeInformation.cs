using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace common
{
    public class TimeInformation
    {
        public TimeSpan StartTime
        {
            get;
            private set;
        }

        public TimeSpan EndTime
        {
            get;
            private set;
        }

        public TimeSpan MouseOffset
        {
            get;
            private set;
        }

        TimeInformation(TimeSpan startTime, TimeSpan endTime, TimeSpan mouseOffset)
        {
            StartTime = startTime;
            EndTime = endTime;
            MouseOffset = mouseOffset;
        }

        public static TimeInformation ParseFromTi(string filePath)
        {
            TimeSpan startTime = TimeSpan.Zero;
            TimeSpan endTime = TimeSpan.Zero;
            TimeSpan mouseOffset = TimeSpan.Zero;

            string[] lines = System.IO.File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] cols = Regex.Split(line.Replace("\"", ""), ";");

                switch (cols[0].ToLower())
                {
                    case "starttimems":
                        startTime = TimeSpan.FromTicks((long)(Double.Parse(cols[1]) * (double)TimeSpan.TicksPerMillisecond));
                        break;
                    case "endtimems":
                        endTime = TimeSpan.FromTicks((long)(Double.Parse(cols[1]) * (double)TimeSpan.TicksPerMillisecond));
                        break;
                    case "mouseoffset":
                        mouseOffset = TimeSpan.Parse(cols[1]);
                        break;
                    default:
                        break;
                }
            }

            return new TimeInformation(startTime, endTime, mouseOffset);
        }
    }
}
