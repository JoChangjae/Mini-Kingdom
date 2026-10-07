using UnityEngine;
using MiniKingdom.Data;
using MiniKingdom.Kingdom;

namespace MiniKingdom.Items
{
    /// <summary>
    /// Handles equipment crafting and enhancement.
    /// </summary>
    public class EquipmentManager : MonoBehaviour
    {
        public bool CraftEquipment(EquipmentData blueprint)
        {
            // 자원 소모 체크 (레시피 데이터 필요)
            // if (ResourceManager.Instance.ConsumeResource(blueprint.ReqResource, blueprint.ReqAmount)) ...
            
            return true;
        }

        public bool EnhanceEquipment(EquipmentData equipment)
        {
            if (equipment.CurrentLevel >= 10) return false;

            // 소모 비용
            int cost = equipment.CurrentLevel * 100;
            if (!ResourceManager.Instance.ConsumeResource(ResourceType.Gold, cost)) return false;

            float successRate = GetEnhancementRate(equipment.CurrentLevel);
            if (Random.value <= successRate)
            {
                equipment.CurrentLevel++;
                Debug.Log($"Enhancement Success! Level: {equipment.CurrentLevel}");
                return true;
            }
            else
            {
                // 파괴되지 않음 (캐주얼 지향)
                Debug.Log("Enhancement Failed! Item remains intact.");
                return false;
            }
        }

        private float GetEnhancementRate(int currentLevel)
        {
            if (currentLevel < 5) return 1.0f; // +1~5: 100%
            if (currentLevel < 8) return 0.7f; // +6~8: 70%
            return 0.5f;                       // +9~10: 50%
        }
    }
}
