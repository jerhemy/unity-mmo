using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace UnityMMOServer
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PlayerEntity
    {
        public uint PlayerId;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Name;
        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string LastName;
        
        public byte Race;
        public byte Class;
        
        public byte Level;

        public float heading;
        public double x;
        public double y;
        public double z;
    }
    
    public class Packet
    {
        public PlayerEntity _player;
        public Entity[] _entities;  
    }
    
    class Program
    {
        private static PlayerEntity _player;
        private static List<Entity> _entities = new List<Entity>();
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var packet = new Packet {_player = new PlayerEntity {Name = "Player 1"}};
            
            List<Entity> e = new List<Entity>();

            for (var i = 0; i < 1000; i++)
            {
                e.Add(new Entity{Name = $"Test {i}"});
            }

            packet._entities = e.ToArray();

            var b = Serializer.Serialize(p, e);
            
            Serializer.Deserialize(b);
            
        }


        

        
        private static byte[] getBytes<T>(T data)
        {
            IFormatter formatter = new BinaryFormatter();  
            //Stream stream = new FileStream("MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);  
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, data);  
            stream.Close();

            return stream.ToArray();
            
            int length = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(length);
            byte[] myBuffer = new byte[length];

            Marshal.StructureToPtr(data, ptr, true);
            Marshal.Copy(ptr, myBuffer, 0, length);
            Marshal.FreeHGlobal(ptr);

            return myBuffer;
        }
        
        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        static double ConvertKilobytesToMegabytes(long kilobytes)
        {
            return kilobytes / 1024f;
        }
    }
}
