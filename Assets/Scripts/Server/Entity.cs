using System;
using System.Runtime.InteropServices;

namespace UnityMMOServer
{
    [Serializable]
    public class Entity
    {
        public uint PlayerId;
        
        public string Name;
        public string LastName;
        
        public byte Race;
        public byte Class;
        public ushort Health;
        
        public byte Level;
        public uint XP;

        public float heading;
        public float x;
        public float y;
        public float z;
    }
}