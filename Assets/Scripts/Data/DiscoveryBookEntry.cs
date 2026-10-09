using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    public enum EntryCategory
    {
        Monster,
        Equipment,
        Building,
        NPC,
        Skill,
        Resource
    }

    [Serializable]
    public class DiscoveryMilestone
    {
        public int count;
        public StatModifier[] bonus;
    }

    [CreateAssetMenu(fileName = "NewDiscoveryEntry", menuName = "MiniKingdom/Discovery/New Entry")]
    public class DiscoveryBookEntry : ScriptableObject
    {
        public string entryId;
        public string Id => entryId;
        public string entryName;
        public EntryCategory category;

        [TextArea]
        public string description;
        [TextArea]
        public string lore;

        public Sprite image;
        
        [NonSerialized]
        public bool isDiscovered; // Runtime state

        public DiscoveryMilestone[] milestoneBonuses;
    }
}
