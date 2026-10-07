using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    public enum DecreeType
    {
        Economy,
        Military,
        Magic,
        Exploration
    }

    [CreateAssetMenu(fileName = "NewRoyalDecree", menuName = "MiniKingdom/Kingdom/New Royal Decree")]
    public class RoyalDecreeData : ScriptableObject
    {
        public string decreeId;
        [Tooltip("법령 이름")]
        public string decreeName;
        [TextArea]
        public string description;

        public DecreeType decreeType;

        [Header("Visuals")]
        public Sprite icon;
        public Color color = Color.white;

        [TextArea]
        [Header("Effects")]
        public string effectsDescription;
        public float[] buffValues;
    }
}
