using System.Collections.Concurrent;

namespace UnityMMO.Server
{
    public class EntityManager
    {
        // Containers for Zone entities (Doors, Traps, Mobs, Players, etc)
        private ConcurrentDictionary<ulong, object> doors;
        private ConcurrentDictionary<ulong, object> traps;
        private ConcurrentDictionary<ulong, object> mob;
        private ConcurrentDictionary<ulong, object> player;
        
        private static EntityManager _instance = null;

        public EntityManager Instance => _instance ?? (_instance = new EntityManager());

        private EntityManager()
        {
            
        }
        
        
    }
}