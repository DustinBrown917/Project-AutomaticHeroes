using System.Collections.Generic;

namespace Game
{


    public class MonsterProviderPool
    {
        private readonly Dictionary<string, IMonsterProvider> m_monsterProviders = new();

        public bool TryGet(string id, out IMonsterProvider monsterProvider) => m_monsterProviders.TryGetValue(id, out monsterProvider);

        public bool TryGetFallbackProvider(out IMonsterProvider monsterProvider)
        {
            foreach (IMonsterProvider provider in m_monsterProviders.Values) {
                if (provider != null) {
                    monsterProvider = provider;
                    return true;
                }
            }

            monsterProvider = null;
            return false;
        }


        public bool TryGetWithFallback(string id, out IMonsterProvider monsterProvider)
        {
            if (m_monsterProviders.TryGetValue(id, out monsterProvider))
                return true;

            return TryGetFallbackProvider(out monsterProvider);
        }


        public void SetProvider(string id, IMonsterProvider monsterProvider)
        {
            m_monsterProviders[id] = monsterProvider;
        }

        public bool RemoveProvider(string id)
        {
            return m_monsterProviders.Remove(id);
        }
    }
}
