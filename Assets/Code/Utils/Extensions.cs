using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Utils
{
    public static class Extensions
    {
        
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static T ToDeserialized<T>(this string json)
        {
          return  JsonConvert.DeserializeObject<T>(json);
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
        
        public static Vector3 AsVector3(this Vector2 vector) =>
            new Vector3(vector.x, vector.y,0);     
        public static Vector2 AsVector2(this Vector3 vector) =>
            new Vector2(vector.x, vector.y);
    }
}