using UnityEngine;

namespace MiniKingdom.Data
{
    /// <summary>
    /// ScriptableObject defining a skill fusion recipe.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkillFusion", menuName = "MiniKingdom/Data/SkillFusionData")]
    public class SkillFusionData : ScriptableObject
    {
        public string requiredSkill1_Id;
        public string requiredSkill2_Id;
        
        /// <summary>
        /// Reference to the resulting SkillData ID.
        /// </summary>
        public string resultingSkill_Id;
    }
}
