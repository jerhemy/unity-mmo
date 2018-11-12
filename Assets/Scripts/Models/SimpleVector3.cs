using UnityEngine;

namespace UnityMMO.Models
{
    public struct SimpleVector3
    {
        public double x;
        public double y;
        public double z;
        
        public SimpleVector3(double _x, double _y, double _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public Vector3 toVector3()
        {
            return new Vector3((float)x, (float)y, (float)z);
        }
    }
}