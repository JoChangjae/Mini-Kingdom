using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    /// <summary>
    /// Types of resources in the game.
    /// </summary>
    public enum ResourceType
    {
        Gold,
        Wood,
        Stone,
        Ore,
        Gem,
        ManaStone,
        Herb,
        Food,
        Crown // Premium currency
    }

    /// <summary>
    /// Stores display information for a resource type.
    /// </summary>
    [CreateAssetMenu(fileName = "NewResourceDefinition", menuName = "MiniKingdom/Data/Resource Definition")]
    public class ResourceDefinition : ScriptableObject
    {
        public ResourceType type;
        public string displayName; // Korean name
        public Sprite icon;
        public Color color = Color.white;
    }

    /// <summary>
    /// Represents a cost or amount of a specific resource.
    /// </summary>
    [Serializable]
    public struct ResourceCost
    {
        public ResourceType resourceType;
        public int amount;
    }
}
