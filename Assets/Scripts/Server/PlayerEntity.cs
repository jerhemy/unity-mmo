// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace

using System;


namespace UnityMMOServer
{
    [Serializable]
    public struct PlayerEntity1
    {
        public uint PlayerId;

        public string Name;
        
        public string LastName;
        
        public byte Race;
        public byte Class;
        
        public byte Level;

        public float heading;
        public float x;
        public float y;
        public float z;
    }

    public struct SpellBuff
    {
        public ushort SpellId;
        public ushort RemainingTicks;
    }
}