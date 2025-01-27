using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Utils
{
    public static class Extensions
    {
        public static bool IsMacOs()
        {
            return Application.platform is RuntimePlatform.OSXEditor or RuntimePlatform.OSXPlayer;
        }

        public static bool IsEqualDay(DateTime lastVisit, DateTime currenVisit)
        {
            return lastVisit != DateTime.MinValue && currenVisit != DateTime.MinValue &&
                   lastVisit.Day == currenVisit.Day;
        }

        public static void ShuffleList<T>(List<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
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



        
    }
}