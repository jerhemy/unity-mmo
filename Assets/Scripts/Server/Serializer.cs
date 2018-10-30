using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace UnityMMOServer
{
    public class Serializer
    {
        public static byte[] Serialize<T>(T data) where T : struct
        {
            using (MemoryStream ms = new MemoryStream())
            {
                #if DEBUG
                var watch = System.Diagnostics.Stopwatch.StartNew();
                #endif
                
                IFormatter formatter = new BinaryFormatter();
                
                formatter.Serialize(ms, data);

                #if DEBUG
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"Time to Serialize: {elapsedMs} ms");
                Console.WriteLine($"Stream Size {ms.Length / 1024} KB");
                #endif

                return ms.ToArray();
            }
        }
        
        public static void Deserialize(byte[] data)
        {
            
            if (data.Length > 0)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(data);

                PlayerEntity _player = (PlayerEntity) formatter.Deserialize(stream);
                int entityCount =  (int) formatter.Deserialize(stream) / (Marshal.SizeOf(typeof(Entity)));

                var e = (Entity[]) formatter.Deserialize(stream);

                stream.Close();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;

                Console.WriteLine($"Time to Deserialize: {elapsedMs} ms");
            }
        }
    }
}