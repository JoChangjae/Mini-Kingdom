using UnityEngine;
using MiniKingdom.Data;
using MiniKingdom.Core;

namespace MiniKingdom.Enemy
{
    /// <summary>
    /// Boss behavior with phase transitions.
    /// </summary>
    public class BossController : EnemyController
    {
        private int _currentPhase = 1;
        private float _maxHp;

        protected override void Start()
        {
            base.Start();
            // 임시로 체력을 가져오는 로직 (실제로는 base에서 처리된 _hp를 프로퍼티로 빼야함)
            _maxHp = 1000f; // Mock
        }

        protected override void Update()
        {
            base.Update();
            CheckPhaseTransition();
        }

        private void CheckPhaseTransition()
        {
            // 현재 체력 비율에 따라 2페이즈, 3페이즈 전환
            float hpRatio = GetCurrentHpRatio(); 

            if (_currentPhase == 1 && hpRatio <= 0.5f)
            {
                EnterPhase(2);
            }
        }

        private void EnterPhase(int phase)
        {
            _currentPhase = phase;
            Debug.Log($"Boss entering Phase {phase}!");
            
            // Time pause for cinematic
            Time.timeScale = 0f;
            
            // TODO: Play roar animation, change BGM, spawn minions
            
            Invoke(nameof(ResumeFromCinematic), 2f); // 리얼타임 기준 Invoke 불가능하므로 코루틴이나 UniTask 권장
            // 여기선 간단히 표현
            Time.timeScale = 1f;
        }

        private void ResumeFromCinematic()
        {
            Time.timeScale = 1f;
        }

        private float GetCurrentHpRatio()
        {
            // EnemyController의 _hp 접근을 위해 public getter가 필요함 (여기선 생략)
            return 0.4f; 
        }

        protected override void Die()
        {
            base.Die();
            EventBus.Publish(new GameEvents.BossKilledEvent());
        }
    }
}
