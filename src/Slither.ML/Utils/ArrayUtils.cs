using System;

namespace Slither.ML.Utils
{
    public static class ArrayUtils
    {
        public static T[] Repeat<T>(this T[] array, int times)
        {
            var length = array.Length;
            var result = new T[length * times];
            for (int i = 0; i < times; i++)
            {
                Array.Copy(array, 0, result, i * length, length);
            }
            return result;
        }

        public static T[] StackAndShift<T>(this T[] array, T[] value)
        {
            var length = array.Length;
            var shiftLength = value.Length;
            var result = new T[length];
            Array.Copy(array, shiftLength, result, 0, length - shiftLength);
            Array.Copy(value, 0, result, length - shiftLength, shiftLength);
            return result;
        }

    }
}