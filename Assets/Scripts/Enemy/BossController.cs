using System.Collections;
using UnityEngine;
using MiniKingdom.Data;
using MiniKingdom.Core;
using MiniKingdom.UI;
using MiniKingdom.Combat;

namespace MiniKingdom.Enemy
{
    /// <summary>
    /// Boss behavior with multi-phase mechanics, telegraphed AoE ground slams, and rage mode.
    /// </summary>
    public class BossController : EnemyController
    {
        [Header("Boss Phases & Skills")]
        [SerializeField] private float slamCooldown = 5f;
        [SerializeField] private float slamRadius = 3.5f;
        [SerializeField] private float slamDamage = 25f;

        private int _currentPhase = 1;
        private float _lastSlamTime = -99f;
        private bool _isPerformingSlam = false;
        private DungeonHUDScreen _hudScreen;

        protected override void Start()
        {
            base.Start();
            _maxHp = data != null ? data.MaxHP : 600f;
            _hp = _maxHp;

            _hudScreen = FindFirstObjectByType<DungeonHUDScreen>();
            string bossName = data != null ? data.enemyName : "숲 트롤 (보스)";
            _hudScreen?.ShowBossHP(bossName, _hp, _maxHp);
        }

        protected override void Update()
        {
            if (_currentState == EnemyState.Dead || _currentState == EnemyState.Stunned) return;

            if (_isPerformingSlam) return;

            // Phase 2 & 3: Periodic Ground Slam
            if (_currentPhase >= 2 && Time.time >= _lastSlamTime + slamCooldown)
            {
                if (_player != null && Vector2.Distance(transform.position, _player.position) <= slamRadius * 1.5f)
                {
                    StartCoroutine(PerformGroundSlamRoutine());
                    return;
                }
            }

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
            Debug.Log($"[BossController] 🔥 보스 페이즈 {phase} 진입!");

            CameraShake.Shake(0.35f, 0.2f);

            if (phase == 2)
            {
                transform.localScale = Vector3.one * 1.3f;
                slamCooldown = 4f;
                PopupManager.Instance?.ShowToast("⚠️ 보스 분노! 대지 강타를 시전합니다!");
            }
            else if (phase == 3)
            {
                transform.localScale = Vector3.one * 1.55f;
                slamCooldown = 2.5f;

                var sr = GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.color = new Color(1f, 0.4f, 0.4f);

                PopupManager.Instance?.ShowToast("🔥 보스 광란 모드 돌입! (공격 속도 대폭 증가)");
            }
        }

        private IEnumerator PerformGroundSlamRoutine()
        {
            _isPerformingSlam = true;
            _lastSlamTime = Time.time;
            _currentState = EnemyState.Attack;

            Vector3 slamTargetPos = _player != null ? _player.position : transform.position;

            // 1. Wind-up jump
            var sr = GetComponentInChildren<SpriteRenderer>();
            Color origColor = sr != null ? sr.color : Color.white;
            if (sr != null) sr.color = new Color(1f, 0.6f, 0.1f); // Warning orange

            Vector3 baseScale = transform.localScale;
            transform.localScale = baseScale * 1.25f; // Jump scale up

            yield return new WaitForSeconds(0.7f);

            if (_currentState == EnemyState.Dead) yield break;

            // 2. Slam down impact!
            transform.localScale = baseScale;
            if (sr != null) sr.color = origColor;

            CameraShake.Shake(0.4f, 0.3f);

            // Check damage on player
            if (_player != null && Vector2.Distance(transform.position, _player.position) <= slamRadius)
            {
                var pCtrl = _player.GetComponent<Player.PlayerController>();
                if (pCtrl != null && !pCtrl.CheckPerfectDodge(Time.time))
                {
                    var pStats = _player.GetComponent<Player.PlayerStats>();
                    float actualDmg = slamDamage * (_currentPhase == 3 ? 1.5f : 1.0f);
                    pStats?.TakeDamage(actualDmg, DamageType.Physical);
                }
            }

            yield return new WaitForSeconds(0.4f);

            _isPerformingSlam = false;
            _currentState = EnemyState.Chase;
        }

        public new void TakeDamage(float amount)
        {
            base.TakeDamage(amount);

            if (_hudScreen == null) _hudScreen = FindFirstObjectByType<DungeonHUDScreen>();
            _hudScreen?.UpdateBossHP(_hp, _maxHp);
        }

        private float GetCurrentHpRatio()
        {
            return Mathf.Clamp01(_hp / Mathf.Max(1f, _maxHp));
        }

        protected override void Die()
        {
            base.Die();
            CameraShake.Shake(0.6f, 0.35f);

            _hudScreen?.HideBossHP();

            string bossName = data != null ? data.enemyName : "숲 트롤 (보스)";
            EventBus.Publish(new BossKilledEvent(bossName));
            PopupManager.Instance?.ShowToast($"🎉 보스 [{bossName}] 토벌 성공!");
            Debug.Log($"[BossController] 보스 {bossName} 처치 완료!");
        }
    }
}
