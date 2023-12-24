using UnityEngine;

namespace Code.Utils
{
    public static class Extensions
    {
        
        public static string ToJson(this object obj) => 
            JsonUtility.ToJson(obj);
        public static T ToDeserialized<T>(this string json) => 
            JsonUtility.FromJson<T>(json);
    }
}