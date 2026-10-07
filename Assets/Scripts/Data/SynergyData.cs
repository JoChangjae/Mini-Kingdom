using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    [CreateAssetMenu(fileName = "NewSynergy", menuName = "MiniKingdom/Upgrades/New Synergy")]
    public class SynergyData : ScriptableObject
    {
        public string synergyId;
        [Tooltip("시너지 이름")]
        public string synergyName;
        [TextArea]
        public string description;

        public UpgradeCategory requiredCategory;
        public int requiredCount = 3;

        [Header("Effects")]
        public StatModifier[] bonusEffects;
        
        [Tooltip("디메리트 효과 (선택)")]
        public StatModifier[] downside;

        [Header("Visuals")]
        public Sprite icon;
        public Color color = Color.white;
    }
}
