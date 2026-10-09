using System;
using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;
using MiniKingdom.Utils;
using MiniKingdom.Combat;

namespace MiniKingdom.Player
{
    /// <summary>
    /// Controls the player character including auto-movement, auto-attack, dodge, and perfect dodge mechanics.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement & Combat")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float dodgeSpeed = 15f;
        [SerializeField] private float dodgeDuration = 0.3f;
        [SerializeField] private float dodgeCooldown = 1.5f;
        [SerializeField] private float attackRange = 2f;
        
        [Header("Perfect Dodge")]
        [SerializeField] private float perfectDodgeWindow = 0.2f;
        [SerializeField] private float perfectDodgeSlowDuration = 0.5f;
        [SerializeField] private float perfectDodgeTimeScale = 0.3f;

        private Rigidbody2D _rb;
        private PlayerStats _stats;
        private Transform _targetEnemy;
        
        // State
        private bool _isDodging;
        private bool _isInvincible;
        private float _lastDodgeTime;
        private float _dodgeTimer;
        private Vector2 _dodgeDirection;
        private float _nextAttackTime;
        private bool _perfectDodgeActive;

        // Swipe detection
        private Vector2 _touchStartPos;
        private const float SwipeThreshold = 50f;

        public event Action OnPerfectDodge;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _stats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            if (Time.timeScale == 0) return; // Paused

            HandleInput();
            HandleCombat();
        }

        private void FixedUpdate()
        {
            if (_isDodging)
            {
                HandleDodgeMovement();
            }
            else
            {
                HandleAutoMovement();
            }
        }

        private void HandleInput()
        {
            if (_isDodging || Time.time < _lastDodgeTime + dodgeCooldown) return;

            // Simple swipe detection for mobile
            if (Input.GetMouseButtonDown(0))
            {
                _touchStartPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 swipeDelta = (Vector2)Input.mousePosition - _touchStartPos;
                if (swipeDelta.magnitude > SwipeThreshold)
                {
                    InitiateDodge(swipeDelta.normalized);
                }
            }

            // Keyboard fallback for testing
            Vector2 kbdInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (kbdInput != Vector2.zero && Input.GetKeyDown(KeyCode.Space))
            {
                InitiateDodge(kbdInput.normalized);
            }
        }

        private void InitiateDodge(Vector2 direction)
        {
            _isDodging = true;
            _isInvincible = true;
            _dodgeDirection = direction;
            _dodgeTimer = dodgeDuration;
            _lastDodgeTime = Time.time;
            
            // 퍼펙트 회피 체크 로직은 적의 공격 타이밍과 맞물려야 함 (EnemyController에서 호출)
        }

        private void HandleDodgeMovement()
        {
            _dodgeTimer -= Time.fixedDeltaTime;
            if (_dodgeTimer <= 0)
            {
                _isDodging = false;
                _isInvincible = false;
                _rb.velocity = Vector2.zero;
            }
            else
            {
                _rb.velocity = _dodgeDirection * dodgeSpeed;
            }
        }

        private void HandleAutoMovement()
        {
            FindNearestEnemy();

            if (_targetEnemy != null)
            {
                float dist = Vector2.Distance(transform.position, _targetEnemy.position);
                if (dist > attackRange)
                {
                    Vector2 dir = (_targetEnemy.position - transform.position).normalized;
                    float currentMoveSpeed = _stats != null ? _stats.CalculateFinalStat(StatType.MOV) : moveSpeed;
                    _rb.velocity = dir * currentMoveSpeed;
                }
                else
                {
                    _rb.velocity = Vector2.zero;
                }
            }
            else
            {
                _rb.velocity = Vector2.zero;
            }
        }

        private void HandleCombat()
        {
            if (_targetEnemy == null) return;

            float dist = Vector2.Distance(transform.position, _targetEnemy.position);
            if (dist <= attackRange && Time.time >= _nextAttackTime)
            {
                Attack();
            }
        }

        private void Attack()
        {
            float attackSpeed = _stats != null ? _stats.CalculateFinalStat(StatType.SPD) : 1f;
            _nextAttackTime = Time.time + (1f / attackSpeed);
            
            // Perform attack via CombatSystem
            float damage = _stats != null ? _stats.CalculateFinalStat(StatType.ATK) : 10f;
            if (_perfectDodgeActive)
            {
                damage *= 2f; // Double damage after perfect dodge
                _perfectDodgeActive = false;
            }

            // Mock attack
            CombatSystem.Instance.ProcessAttack(this.gameObject, _targetEnemy.gameObject, damage, DamageType.Physical);
        }

        private void FindNearestEnemy()
        {
            // 간단한 O(N) 탐색, 나중에 Spatial Partitioning 최적화 가능
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float minDist = float.MaxValue;
            Transform closest = null;

            foreach (var enemy in enemies)
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy.transform;
                }
            }

            _targetEnemy = closest;
        }

        /// <summary>
        /// Called by enemy attacks to check for perfect dodge
        /// </summary>
        public bool CheckPerfectDodge(float attackHitTime)
        {
            if (!_isDodging) return false;

            float timeSinceDodge = Time.time - _lastDodgeTime;
            if (timeSinceDodge <= perfectDodgeWindow)
            {
                TriggerPerfectDodge();
                return true;
            }
            return false;
        }

        private void TriggerPerfectDodge()
        {
            _perfectDodgeActive = true;
            OnPerfectDodge?.Invoke();
            
            // Time manipulation
            Time.timeScale = perfectDodgeTimeScale;
            Invoke(nameof(ResetTimeScale), perfectDodgeSlowDuration * perfectDodgeTimeScale); // Scaled duration

            // 시각 효과 (Golden Screen Flash) 처리 이벤트 발행
            EventBus.Publish(new PerfectDodgeEvent());
        }

        private void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }

        public bool IsInvincible() => _isInvincible;
    }
}
