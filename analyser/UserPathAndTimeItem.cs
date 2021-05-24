using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyser
{
    public class UserPathAndTimeItem
    {
        public double MovingDistance { get; }
        public double WeightMovingDistance { get; }
        public double OperationMillisecond { get; }
        public double PureOperationMillisecond { get; }
        //public double MovingDistancePerMillisecond { get; }

        public UserPathAndTimeItem(
            double movingDistance,
            double weightMovingDistance,
            double operationMillisecond,
            double pureOperationMillisecond
        //double movingDistancePerMillisecond
        )
        {
            MovingDistance = movingDistance;
            WeightMovingDistance = weightMovingDistance;
            OperationMillisecond = operationMillisecond;
            PureOperationMillisecond = pureOperationMillisecond;
            //MovingDistancePerMillisecond = movingDistancePerMillisecond;
        }

        public static UserPathAndTimeItem operator +(UserPathAndTimeItem t1, UserPathAndTimeItem t2)
        {
            if (t1 != null && t2 != null)
            {
                return new UserPathAndTimeItem(t1.MovingDistance + t2.MovingDistance, t1.WeightMovingDistance + t2.WeightMovingDistance, t1.OperationMillisecond + t2.OperationMillisecond, t1.PureOperationMillisecond + t2.PureOperationMillisecond);
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

        public static UserPathAndTimeItem operator -(UserPathAndTimeItem t1, UserPathAndTimeItem t2)
        {
            if (t1 != null && t2 != null)
            {
                return new UserPathAndTimeItem(t1.MovingDistance - t2.MovingDistance, t1.WeightMovingDistance - t2.WeightMovingDistance, t1.OperationMillisecond - t2.OperationMillisecond, t1.PureOperationMillisecond - t2.PureOperationMillisecond);
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
    }
}
