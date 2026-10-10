using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;
using MiniKingdom.Utils;

namespace MiniKingdom.Kingdom
{
    /// <summary>
    /// Singleton managing all kingdom resources with automatic persistence and starting balances.
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();

        private void Start()
        {
            LoadResources();
        }

        private void LoadResources()
        {
            var saved = SaveManager.LoadInventory();
            if (saved != null && saved.Resources != null && saved.Resources.Count > 0)
            {
                foreach (var entry in saved.Resources)
                {
                    _resources[entry.Type] = entry.Amount;
                }
            }

            // New game initial resource grants
            if (GetResource(ResourceType.Gold) == 0 && GetResource(ResourceType.Wood) == 0 && GetResource(ResourceType.Stone) == 0)
            {
                _resources[ResourceType.Gold] = 1000;
                _resources[ResourceType.Wood] = 200;
                _resources[ResourceType.Stone] = 100;
                SaveResources();
            }
        }

        private void SaveResources()
        {
            SaveManager.SaveInventory(_resources, null);
        }

        public void AddResource(ResourceType type, int amount)
        {
            if (_resources.ContainsKey(type)) _resources[type] += amount;
            else _resources[type] = amount;
            
            SaveResources();
            EventBus.Publish(new ResourceChangedEvent(type, _resources[type]));
        }

        public bool ConsumeResource(ResourceType type, int amount)
        {
            if (HasResource(type, amount))
            {
                _resources[type] -= amount;
                SaveResources();
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

        public void ProcessProduction()
        {
            // Passive production logic if needed
        }
    }
}
