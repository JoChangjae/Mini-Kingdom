using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniKingdom.Data
{
    [Serializable]
    public class LootEntry
    {
        public ResourceType resourceType;
        public int minAmount;
        public int maxAmount;
        [Range(0f, 1f)]
        public float dropChance;

        public float DropChance => dropChance;
        public ResourceType ResourceType => resourceType;
        public int MinAmount => minAmount;
        public int MaxAmount => maxAmount;
    }

    [Serializable]
    public class LootResult
    {
        public ResourceType resourceType;
        public int amount;
    }

    [Serializable]
    public class LootTable
    {
        public LootEntry[] entries;
        public LootEntry[] Items => entries ?? Array.Empty<LootEntry>();

        public List<LootResult> Roll(float bonusMultiplier = 1f)
        {
            List<LootResult> results = new List<LootResult>();
            if (entries == null) return results;

            foreach (var entry in entries)
            {
                float chance = entry.dropChance * bonusMultiplier;
                if (UnityEngine.Random.value <= chance)
                {
                    int amount = UnityEngine.Random.Range(entry.minAmount, entry.maxAmount + 1);
                    results.Add(new LootResult { resourceType = entry.resourceType, amount = amount });
                }
            }
            return results;
        }
    }
}
