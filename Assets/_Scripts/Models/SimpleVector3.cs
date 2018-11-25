using System;
using UnityEngine;

namespace UnityMMO.Models
{
   public struct SimpleVector3 : IEquatable<SimpleVector3>
    {
        // *Undocumented*
        public const float kEpsilon = 0.00001F;
        // *Undocumented*
        public const float kEpsilonNormalSqrt = 1e-15F;
        
        public float x;
        public float y;
        public float z;
        
        public SimpleVector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        // Creates a new vector with given x, y components and sets /z/ to zero.
        public SimpleVector3(float x, float y) { this.x = x; this.y = y; z = 0F; }
               
        public static SimpleVector3 MoveTowards(SimpleVector3 current, SimpleVector3 target, float maxDistanceDelta)
        {
            SimpleVector3 toVector = target - current;
            float dist = toVector.magnitude;
            if (dist <= maxDistanceDelta || dist < float.Epsilon)
                return target;
            return current + toVector / dist * maxDistanceDelta;
        }

        public bool Equals(SimpleVector3 other)
        {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        
        public static float Magnitude(SimpleVector3 vector) { return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z); }

        // Returns the length of this vector (RO).
        public float magnitude { get { return Mathf.Sqrt(x * x + y * y + z * z); } }
        
        // *undoc* --- there's a property now
        public static float SqrMagnitude(SimpleVector3 vector) { return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z; }

        // Returns the squared length of this vector (RO).
        public float sqrMagnitude { get { return x * x + y * y + z * z; } }
        
        
        public static SimpleVector3 operator+(SimpleVector3 a, SimpleVector3 b) { return new SimpleVector3(a.x + b.x, a.y + b.y, a.z + b.z); }
        // Subtracts one vector from another.
        public static SimpleVector3 operator-(SimpleVector3 a, SimpleVector3 b) { return new SimpleVector3(a.x - b.x, a.y - b.y, a.z - b.z); }
        // Negates a vector.
        public static SimpleVector3 operator-(SimpleVector3 a) { return new SimpleVector3(-a.x, -a.y, -a.z); }
        // Multiplies a vector by a number.
        public static SimpleVector3 operator*(SimpleVector3 a, float d) { return new SimpleVector3(a.x * d, a.y * d, a.z * d); }
        // Multiplies a vector by a number.
        public static SimpleVector3 operator*(float d, SimpleVector3 a) { return new SimpleVector3(a.x * d, a.y * d, a.z * d); }
        // Divides a vector by a number.
        public static SimpleVector3 operator/(SimpleVector3 a, float d) { return new SimpleVector3(a.x / d, a.y / d, a.z / d); }

        // Returns true if the vectors are equal.
        public static bool operator==(SimpleVector3 lhs, SimpleVector3 rhs)
        {
            // Returns false in the presence of NaN values.
            return SqrMagnitude(lhs - rhs) < kEpsilon * kEpsilon;
        }

        // Returns true if vectors are different.
        public static bool operator!=(SimpleVector3 lhs, SimpleVector3 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        public Vector3 ToVector3()
        {
            return new Vector3{ x = this.x, y = this.y, z=this.z};
        }
    }
}