using System;
using UnityEngine;
using MiniKingdom.Data;
using MiniKingdom.Core;

namespace MiniKingdom.Kingdom
{
    /// <summary>
    /// Handles offline idle rewards (Taxes).
    /// Calculates rewards based on time since last logout and Kingdom Level.
    /// </summary>
    public class TaxSystem : MonoBehaviour
    {
        public static TaxSystem Instance { get; private set; }

        private const int MAX_OFFLINE_MINUTES = 720; // 12 hours max
        
        public int PendingGold { get; private set; }
        public int PendingResources { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            CalculateOfflineTaxes();
        }

        private void CalculateOfflineTaxes()
        {
            // 오프라인 방치 보상 계산
            DateTime lastLogoutTime = SaveManager.lastLogoutTime; 
            
            // 만약 lastLogoutTime이 초기값이라면 (예: 첫 접속), 보상 없음
            if (lastLogoutTime == default(DateTime))
            {
                return;
            }

            TimeSpan offlineDuration = DateTime.Now - lastLogoutTime;
            int offlineMinutes = Mathf.FloorToInt((float)offlineDuration.TotalMinutes);

            if (offlineMinutes < 1) return;

            // 최대 12시간으로 제한 (Clamp to max 12 hours)
            offlineMinutes = Mathf.Clamp(offlineMinutes, 0, MAX_OFFLINE_MINUTES);

            // 왕국 레벨 가져오기 (임시로 1로 설정, 실제 구현에 맞게 수정 필요)
            int kingdomLevel = 1; 

            // Formula: Gold = Mins * KingdomLevel * 2
            PendingGold = offlineMinutes * kingdomLevel * 2;
            
            // Formula: Resources = Mins * KingdomLevel * 0.5
            PendingResources = Mathf.FloorToInt(offlineMinutes * kingdomLevel * 0.5f);

            Debug.Log($"[TaxSystem] Offline mins: {offlineMinutes}. Pending Taxes -> Gold: {PendingGold}, Resources: {PendingResources}");
        }

        /// <summary>
        /// Claims the pending taxes, adds them to ResourceManager, and clears pending.
        /// </summary>
        public void ClaimTaxes()
        {
            if (PendingGold <= 0 && PendingResources <= 0) return;

            // ResourceManager를 통해 자원 획득 (메서드명은 실제 구현에 따라 조정)
            // ResourceManager.Instance.AddGold(PendingGold);
            // ResourceManager.Instance.AddResources(PendingResources);

            // 파티클 트리거 등 연출 (Trigger particles)
            PlayClaimParticles();

            PendingGold = 0;
            PendingResources = 0;
        }

        private void PlayClaimParticles()
        {
            // 획득 연출 로직
            Debug.Log("[TaxSystem] Playing tax claim particles...");
        }
    }
}
