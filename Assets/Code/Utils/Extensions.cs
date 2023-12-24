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
    }
}