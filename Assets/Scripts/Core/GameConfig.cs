using UnityEngine;

namespace MiniKingdom.Core
{
    /// <summary>
    /// Global game configuration data.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "MiniKingdom/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Daily Systems")]
        public int DailyBonusRunCount = 5;
        public float DailyBonusMultiplier = 1.5f;
        public int KingdomVisitPerDay = 3;

        [Header("Run Mechanics")]
        [Tooltip("던전 런 실패 시 유지되는 재화의 비율 (0.0 ~ 1.0)")]
        [Range(0f, 1f)]
        public float RunFailResourceRetainRate = 0.7f;
        public int MaxPlayerLevelPerRun = 15;

        [Header("Combat Mechanics")]
        [Tooltip("퍼펙트 회피 발동 시 슬로우 모션 지속 시간 (초)")]
        public float PerfectDodgeSlowMoDuration = 0.5f;
        [Tooltip("퍼펙트 회피 후 적용되는 대미지 배수")]
        public float PerfectDodgeDamageMultiplier = 2.0f;

        [Header("Economy")]
        public int BaseGoldDrop = 10;
        public int BaseWoodDrop = 5;
        public int BaseStoneDrop = 2;
    }
}
