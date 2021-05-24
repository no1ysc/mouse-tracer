using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace common
{
    public struct MouseEventPoint
    {
        public MouseEventPoint(string strTimeSapn, string mouseEvent, string x, string y)
        {
            this.timeSpan = TimeSpan.Parse(strTimeSapn);
            this.mouseEvent = (MouseEvent)Enum.Parse(typeof(MouseEvent), mouseEvent);
            this.x = int.Parse(x);
            this.y = int.Parse(y);
        }
        public TimeSpan timeSpan;
        public MouseEvent mouseEvent;
        public int x;
        public int y;

        public override string ToString()
        {
            return $"{this.timeSpan}, {this.mouseEvent}, {this.x}, {this.y}";
        }

        public static List<MouseEventPoint> Parse(string filePath)
        {
            List<MouseEventPoint> ret = new List<MouseEventPoint>();

            string[] lines = System.IO.File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] rows = Regex.Split(line, ", ");

                if (rows.Length != 4 )
                {
                    continue;
                }
                
                ret.Add(new MouseEventPoint(rows[0], rows[1], rows[2], rows[3]));
            }

            return ret;
        }
    }
}
