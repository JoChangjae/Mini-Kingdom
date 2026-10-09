using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;
using MiniKingdom.Utils;

namespace MiniKingdom.Kingdom
{
    /// <summary>
    /// Singleton managing all kingdom resources.
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();

        public void AddResource(ResourceType type, int amount)
        {
            if (_resources.ContainsKey(type)) _resources[type] += amount;
            else _resources[type] = amount;
            
            EventBus.Publish(new ResourceChangedEvent(type, _resources[type]));
        }

        public bool ConsumeResource(ResourceType type, int amount)
        {
            if (HasResource(type, amount))
            {
                _resources[type] -= amount;
                EventBus.Publish(new ResourceChangedEvent(type, _resources[type]));
                return true;
            }
            return false;
        }

        public bool Spend(ResourceType type, int amount) => ConsumeResource(type, amount);

        public bool HasResource(ResourceType type, int amount)
        {
            return _resources.ContainsKey(type) && _resources[type] >= amount;
        }

        public int GetResource(ResourceType type)
        {
            return _resources.ContainsKey(type) ? _resources[type] : 0;
        }

        public int GetAmount(ResourceType type) => GetResource(type);

        // 건물의 자원 생산 (시간 기반)
        public void ProcessProduction()
        {
            // 주기적으로 호출되어 건물의 생산량 추가
        }
    }
}
