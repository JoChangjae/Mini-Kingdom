using System;
using UnityEngine;

namespace MiniKingdom.Data
{
    [Serializable]
    public class SetBonus
    {
        public int requiredCount; // 2, 4, or 6
        [TextArea]
        public string bonusDescription;
        public StatModifier[] stats;
        public string specialAbilityId;
    }

    [CreateAssetMenu(fileName = "NewEquipmentSet", menuName = "MiniKingdom/Equipment/New Equipment Set")]
    public class EquipmentSetData : ScriptableObject
    {
        public int setId;
        [Tooltip("세트 이름")]
        public string setName;

        public SetBonus[] setBonuses;
        public string[] equipmentIds;
    }
}
