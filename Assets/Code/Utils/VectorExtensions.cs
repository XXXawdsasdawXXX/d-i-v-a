using UnityEngine;

namespace Code.Utils
{
    public static class VectorExtensions
    {
        public static Vector3 AsVector3(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }

        public static Vector2 AsVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static float Distance(this Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }
    }
}