using UnityEngine;

namespace _Scripts.Utility
{
    public static class Extensions
    {
        public static Vector3 ClampX(this Vector3 source, float min, float max)
        {
            source.x = Mathf.Clamp(source.x, min, max);
            return source;
        }

        public static Vector3 OnlyX(this Vector3 source)
        {
            return new Vector3(source.x, 0f, 0f);
        }  
        
        public static Vector3 OnlyY(this Vector3 source)
        {
            return new Vector3(0f, source.y, 0f);
        }
        
        public static Vector3 OverrideY(this Vector3 source, float y)
        {
            return new Vector3(source.x, y, source.z);
        }
    }
}