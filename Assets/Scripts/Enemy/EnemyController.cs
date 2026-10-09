using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;

namespace MiniKingdom.Enemy
{
    public enum EnemyState { Idle, Chase, Attack, Retreat, Stunned, Dead }

    /// <summary>
    /// Base enemy behavior controller.
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] protected EnemyData data;
        
        public EnemyData Data => data;
        public float DEF { get; private set; }
        public float HP => _hp;
        public float MaxHP => _maxHp;
        
        protected EnemyState _currentState = EnemyState.Idle;
        protected float _hp;
        protected float _maxHp = 100f;
        protected Transform _player;

        private float _attackTimer;
        private bool _isWindingUp;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor = Color.white;

        protected virtual void Start()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;

            if (data != null)
            {
                _hp = data.MaxHP;
                _maxHp = data.MaxHP;
                DEF = data.DEF;
            }
            else
            {
                _hp = _maxHp;
                DEF = 5f;
            }

            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        public void SetData(EnemyData enemyData)
        {
            data = enemyData;
            if (data != null)
            {
                _hp = data.MaxHP;
                _maxHp = data.MaxHP;
                DEF = data.DEF;
            }
        }

        protected virtual void Update()
        {
            if (_currentState == EnemyState.Dead || _currentState == EnemyState.Stunned) return;

            float detectionRange = data != null ? data.DetectionRange : 8f;

            switch (_currentState)
            {
                case EnemyState.Idle:
                    if (_player != null && Vector2.Distance(transform.position, _player.position) < detectionRange)
                        _currentState = EnemyState.Chase;
                    break;
                case EnemyState.Chase:
                    ChasePlayer();
                    break;
                case EnemyState.Attack:
                    HandleAttack();
                    break;
            }
        }

        private void ChasePlayer()
        {
            if (_player == null) return;

            float attackRange = data != null ? data.AttackRange : 1.5f;
            float moveSpeed = data != null ? data.MoveSpeed : 2f;

            float dist = Vector2.Distance(transform.position, _player.position);
            if (dist <= attackRange)
            {
                _currentState = EnemyState.Attack;
                _attackTimer = data != null ? data.AttackCooldown : 1.5f;
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, _player.position, moveSpeed * Time.deltaTime);
        }

        private void HandleAttack()
        {
            float attackRange = data != null ? data.AttackRange : 1.5f;
            float windUpTime = data != null ? data.WindUpTime : 0.4f;
            float attackCooldown = data != null ? data.AttackCooldown : 1.5f;

            if (_player == null || Vector2.Distance(transform.position, _player.position) > attackRange * 1.3f)
            {
                _currentState = EnemyState.Chase;
                _isWindingUp = false;
                ResetWindUpColor();
                return;
            }

            _attackTimer -= Time.deltaTime;
            
            // Wind-up logic for perfect dodge indicator (빨간색 경고 표시)
            if (_attackTimer <= windUpTime && !_isWindingUp)
            {
                _isWindingUp = true;
                ShowWindUpIndicator();
            }

            if (_attackTimer <= 0)
            {
                PerformAttack();
                _attackTimer = attackCooldown;
                _isWindingUp = false;
                ResetWindUpColor();
            }
        }

        private void ShowWindUpIndicator()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = new Color(1f, 0.3f, 0.3f); // Red warning tint
            }
        }

        private void ResetWindUpColor()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
        }

        private void PerformAttack()
        {
            if (_player == null) return;

            // 공격 처리 시 플레이어의 퍼펙트 회피 윈도우 체크
            var pCtrl = _player.GetComponent<Player.PlayerController>();
            if (pCtrl != null)
            {
                bool dodged = pCtrl.CheckPerfectDodge(Time.time);
                if (!dodged)
                {
                    // 실제 데미지 적용
                    float atk = data != null ? data.ATK : 10f;
                    var pStats = _player.GetComponent<Player.PlayerStats>();
                    if (pStats != null) pStats.TakeDamage(atk, DamageType.Physical);
                }
            }
        }

        public void TakeDamage(float amount)
        {
            _hp -= amount;

            // 피격 플래시
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.white;
                Invoke(nameof(ResetWindUpColor), 0.1f);
            }

            if (_hp <= 0 && _currentState != EnemyState.Dead)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            _currentState = EnemyState.Dead;
            Combat.CombatSystem.Instance?.RegisterKill();
            EventBus.Publish(new GameEvents.EnemyKilledEvent(this));
            
            // Drop loot
            GetComponent<MiniKingdom.Items.LootManager>()?.DropLoot(transform.position);

            Destroy(gameObject, 0.5f); // 딜레이 후 삭제
        }
    }
}
