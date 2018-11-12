using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityMMO.Models
{
    [StructLayout(LayoutKind.Explicit, Size = 12, Pack = 1)]
    public struct Position
    {
        [FieldOffset(0)] private float x;
        [FieldOffset(4)] private float y;
        [FieldOffset(8)] private float z;
		
        [FieldOffset(12)] private float rotX;
        [FieldOffset(16)] private float rotY;
        [FieldOffset(20)] private float rotZ;
		
        public Position(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
			
            rotX = 0;
            rotY = 0;
            rotZ = 0;
        }
		
        public Position(Vector3 position, Quaternion rotation)
        {
            x = position.x;
            y = position.y;
            z = position.z;

            rotX = rotation.x;
            rotY = rotation.y;
            rotZ = rotation.z;
        }

        public override string ToString()
        {
            return $"X:{x} Y:{y} Z:{z}";
        }
    }
}