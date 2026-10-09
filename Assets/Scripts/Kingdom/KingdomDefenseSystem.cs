using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Utils;

namespace MiniKingdom.Kingdom
{
    /// <summary>
    /// Weekly defense event logic.
    /// </summary>
    public class KingdomDefenseSystem : Singleton<KingdomDefenseSystem>
    {
        public void TriggerWeeklyDefense()
        {
            int defenseScore = CalculateDefenseScore();
            int enemyPower = CalculateEnemyPower();

            if (defenseScore >= enemyPower)
            {
                // 성공
                Debug.Log("Defense Successful!");
                ResourceManager.Instance.AddResource(Data.ResourceType.Gold, 1000); // 보너스
            }
            else
            {
                // 실패 (경미한 페널티)
                Debug.Log("Defense Partially Failed!");
                ResourceManager.Instance.ConsumeResource(Data.ResourceType.Wood, 100);
            }
        }

        private int CalculateDefenseScore()
        {
            int score = 0;
            // score += 성벽 레벨 * 10
            // score += 주둔지 레벨 * 15
            // score += 주민 수 * 5
            return score + 50; // Mock
        }

        private int CalculateEnemyPower()
        {
            // 영지 레벨에 비례하여 적의 강함 증가
            int kingdomLevel = BuildingManager.Instance.GetTotalKingdomLevel();
            return kingdomLevel * 10;
        }
    }
}
