using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Utils
{
    public static class Extensions
    {
        public  static bool IsEqualDay(DateTime lastVisit, DateTime currenVisit)
        {
            return  lastVisit != DateTime.MinValue && currenVisit != DateTime.MinValue && lastVisit.Day == currenVisit.Day;
        }
        
        public static void ShuffleList<T>(List<T> list)
        {
            var n = list.Count;
            for (var i = n - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        
        public static void ShuffleArray<T>(T[] array)
        {
            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
        
        public static Vector3 AsVector3(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }

        public static Vector2 AsVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }


        public static bool Equal(this Color32 color1, Color32 color2)
        {
            return color1.r == color2.r && color1.g == color2.g && color1.b == color2.b && color1.a == color2.a;
        }
        
        public static bool Equal(this Color32 color1, Color32 color2, byte sensitivity = 0)
        {
            byte rDiff = (byte)Mathf.Abs(color1.r - color2.r);
            byte gDiff = (byte)Mathf.Abs(color1.g - color2.g);
            byte bDiff = (byte)Mathf.Abs(color1.b - color2.b);
            byte aDiff = (byte)Mathf.Abs(color1.a - color2.a);

            return rDiff <= sensitivity && gDiff <= sensitivity && bDiff <= sensitivity && aDiff <= sensitivity;
        }
    }
}