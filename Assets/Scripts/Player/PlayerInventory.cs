using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;

namespace MiniKingdom.Player
{
    public enum EquipmentSlot { Weapon, Armor, Helmet, Gloves, Boots, Ring, Necklace }

    /// <summary>
    /// Manages player equipped items and resources.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();
        private Dictionary<EquipmentSlot, EquipmentData> _equippedItems = new Dictionary<EquipmentSlot, EquipmentData>();
        
        private PlayerStats _stats;

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
            LoadInventory();
        }

        public void AddResource(ResourceType type, int amount)
        {
            if (_resources.ContainsKey(type)) _resources[type] += amount;
            else _resources[type] = amount;
            
            SaveManager.SaveInventory(_resources, _equippedItems);
        }

        public void RemoveResource(ResourceType type, int amount)
        {
            if (HasResource(type, amount))
            {
                _resources[type] -= amount;
                SaveManager.SaveInventory(_resources, _equippedItems);
            }
        }

        public bool HasResource(ResourceType type, int amount)
        {
            return _resources.ContainsKey(type) && _resources[type] >= amount;
        }

        public void Equip(EquipmentData item, EquipmentSlot slot)
        {
            if (_equippedItems.ContainsKey(slot))
            {
                Unequip(slot);
            }

            _equippedItems[slot] = item;
            ApplyItemStats(item, true);
            CalculateSetBonuses();
            SaveManager.SaveInventory(_resources, _equippedItems);
        }

        public void Unequip(EquipmentSlot slot)
        {
            if (_equippedItems.TryGetValue(slot, out var item))
            {
                ApplyItemStats(item, false);
                _equippedItems.Remove(slot);
                CalculateSetBonuses();
                SaveManager.SaveInventory(_resources, _equippedItems);
            }
        }

        private void ApplyItemStats(EquipmentData item, bool add)
        {
            if (_stats == null || item == null) return;
            
            foreach (var mod in item.Modifiers)
            {
                if (add) _stats.AddModifier(mod);
                else _stats.RemoveModifier(mod);
            }
        }

        private void CalculateSetBonuses()
        {
            // 세트 효과 로직: 같은 SetId를 가진 장비 개수 카운트
            Dictionary<string, int> setCounts = new Dictionary<string, int>();
            foreach (var item in _equippedItems.Values)
            {
                if (!string.IsNullOrEmpty(item.SetId))
                {
                    if (!setCounts.ContainsKey(item.SetId)) setCounts[item.SetId] = 0;
                    setCounts[item.SetId]++;
                }
            }

            // TODO: Apply set bonuses via Stats based on counts (needs SetBonusData)
        }

        private void LoadInventory()
        {
            var data = SaveManager.LoadInventory();
            if (data != null && data.Resources != null)
            {
                foreach (var r in data.Resources)
                {
                    _resources[r.Type] = r.Amount;
                }
            }
        }
    }
}
