using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace WeatherCalendar
{
    public static class AnimationHelper
    {
        public enum AnimationSpeed
        {
            Low,

            Normal,

            Fast
        }

        private static readonly Random random = new Random(Environment.TickCount);

        public static bool GetNextBool()
        {
            return random.Next(2) == 0;
        }

        public static int GetNextInterval(AnimationSpeed speed = AnimationSpeed.Normal)
        {
            switch (speed)
            {
                case AnimationSpeed.Low:
                    return random.Next(1500, 1000 * 1000);
                case AnimationSpeed.Normal:
                    return random.Next(1500, 1000 * 100);
                case AnimationSpeed.Fast:
                    return random.Next(1500, 1000 * 10);
            }

            return random.Next(1500, 1000 * 60 * 10);
        }

        public static Vector3D GetNextVector3D()
        {
            return new Vector3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
        }

        public static double GetNextAngle()
        {
            return random.NextDouble()*720 - 360;
        }

        public static int GetNextInterval(int min, int max)
        {
            return random.Next(min, max);
        }

        public static Color GetNextColor()
        {
            return Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }

        public static EasingFunctionBase GetEasingFunction()
        {
            EasingFunctionBase result = null;
            switch (random.Next(5))
            {
                case 0:
                    result = new BackEase() { EasingMode = EasingMode.EaseInOut, Amplitude = 0.8 };
                    break;
                case 1:
                    result = new BounceEase() { EasingMode = EasingMode.EaseOut, Bounces = 3, Bounciness = 8 };
                    break;
                case 2:
                    result = new CircleEase() { EasingMode = EasingMode.EaseInOut };
                    break;
                case 3:
                    result = new CubicEase() { EasingMode = EasingMode.EaseIn };
                    break;
                case 4:
                    result = new ElasticEase() { EasingMode = EasingMode.EaseOut, Oscillations = 3, Springiness = 4 };
                    break;
                case 5:
                    result = new SineEase() { EasingMode = EasingMode.EaseInOut };
                    break;
                default:
                    result = new BackEase() { EasingMode = EasingMode.EaseInOut, Amplitude = 0.8 };
                    break;
            }

            return result;
        }
    }
}
