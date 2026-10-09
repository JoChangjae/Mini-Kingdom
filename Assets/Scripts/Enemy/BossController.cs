using UnityEngine;
using MiniKingdom.Data;
using MiniKingdom.Core;
using MiniKingdom.UI;

namespace MiniKingdom.Enemy
{
    /// <summary>
    /// Boss behavior with phase transitions.
    /// </summary>
    public class BossController : EnemyController
    {
        private int _currentPhase = 1;

        protected override void Start()
        {
            base.Start();
            _maxHp = data != null ? data.MaxHP : 1000f;
            _hp = _maxHp;
        }

        protected override void Update()
        {
            base.Update();
            CheckPhaseTransition();
        }

        private void CheckPhaseTransition()
        {
            float hpRatio = GetCurrentHpRatio(); 

            if (_currentPhase == 1 && hpRatio <= 0.5f)
            {
                EnterPhase(2);
            }
            else if (_currentPhase == 2 && hpRatio <= 0.2f)
            {
                EnterPhase(3);
            }
        }

        private void EnterPhase(int phase)
        {
            _currentPhase = phase;
            Debug.Log($"[BossController] 보스 페이즈 {phase} 진입!");

            // 페이즈 전환에 따른 분노 효과 (크기 증가 및 공격력 상승)
            transform.localScale = Vector3.one * (1f + phase * 0.2f);
            
            // 데미지 텍스트 팝업 등으로 페이즈 전환 알림
            PopupManager.Instance?.ShowToast($"⚠️ 보스 분노 모드 돌입! (페이즈 {phase})");
        }

        private float GetCurrentHpRatio()
        {
            return Mathf.Clamp01(_hp / Mathf.Max(1f, _maxHp)); 
        }

        protected override void Die()
        {
            base.Die();
            string bossName = data != null ? data.enemyName : "Forest Boss";
            EventBus.Publish(new BossKilledEvent(bossName));
            Debug.Log($"[BossController] 보스 {bossName} 처치 완료!");
        }
    }
}
