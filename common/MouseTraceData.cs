using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{
    public class MouseTraceData
    {
        public int ClickCount { get; private set; } = 0;
        public int WheelCount { get; private set; } = 0;
        public double PathLength { get; private set; } = 0d;
        public TimeSpan PureOperationTime { get; private set; } = TimeSpan.Zero;

        public MouseEventPoint[] InputData { get; private set; }
        private TimeSpan RestTimePerEvent;
        public MouseTraceData(List<MouseEventPoint> inputData, TimeSpan restTimePerEvent)
        {
            InputData = inputData.ToArray();
            RestTimePerEvent = restTimePerEvent;

            calculate();
        }

        MouseTraceData(int clickCount, int wheelCount, double pathLength)
        {
            ClickCount = clickCount;
            WheelCount = wheelCount;
            PathLength = pathLength;
        }

        private void calculate()
        {
            if (InputData.Length == 0)
            {
                return;
            }

            MouseEventPoint prevData = InputData[0];
            ApplyCount(prevData);
            for (int i = 1; i < InputData.Length; i++)
            {
                MouseEventPoint currentData = InputData[i];
                ApplyCount(currentData);
                ApplyEuclideanDistanceWithRestTime(prevData, currentData);

                prevData = currentData;
            }
        }

        private void ApplyEuclideanDistanceWithRestTime(MouseEventPoint m1, MouseEventPoint m2)
        {
            if (Math.Abs(m2.timeSpan.TotalMilliseconds - m1.timeSpan.TotalMilliseconds) > RestTimePerEvent.TotalMilliseconds)
            {
                return;
            }

            double distance = Math.Sqrt((Math.Pow(m1.x - m2.x, 2) + Math.Pow(m1.y - m2.y, 2)));
            PureOperationTime += (m2.timeSpan - m1.timeSpan);
            PathLength += distance;
        }

        private void ApplyCount(MouseEventPoint mep)
        {
            switch (mep.mouseEvent)
            {
                case MouseEvent.Click:
                case MouseEvent.DoubleClick:
                    ClickCount += 1;
                    break;
                case MouseEvent.Wheel:
                    WheelCount += 1;
                    break;
            }
        }

        public static MouseTraceData operator +(MouseTraceData t1, MouseTraceData t2)
        {
            if (t1 != null && t2 != null)
            {
                return new MouseTraceData(t1.ClickCount + t2.ClickCount, t1.WheelCount + t2.WheelCount, t1.PathLength + t2.PathLength);
            }
            else if (t1 != null)
            {
                return t1;
            }
            else
            {
                return t2;
            }
        }

        public static MouseTraceData operator -(MouseTraceData t1, MouseTraceData t2)
        {
            if (t1 != null && t2 != null)
            {
                return new MouseTraceData(t1.ClickCount - t2.ClickCount, t1.WheelCount - t2.WheelCount, t1.PathLength - t2.PathLength);
            } else if (t1 != null)
            {
                return t1;
            } else
            {
                return t2;
            }
        }
    }
}
