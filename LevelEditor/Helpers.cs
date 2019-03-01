using System;
using System.Collections.Generic;

namespace LevelEditor
{
    public static class ExtensionMethods
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) { return min; }
            else if (val.CompareTo(max) > 0) { return max; }
            else { return val; }
        }

        public static bool Contains<T>(this Array val, T goal) where T : IEquatable<T>
        {
            foreach(T i in val) { if(i.Equals(goal)) { return true; } }
            return false;
        }
    }
}
