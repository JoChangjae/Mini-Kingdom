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
        [SerializeField] private EnemyData data;
        
        public float DEF { get; private set; }
        
        private EnemyState _currentState = EnemyState.Idle;
        private float _hp;
        private Transform _player;

        private float _attackTimer;
        private bool _isWindingUp;

        protected virtual void Start()
        {
            if (data != null)
            {
                _hp = data.MaxHP;
                DEF = data.DEF;
            }
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        protected virtual void Update()
        {
            if (_currentState == EnemyState.Dead || _currentState == EnemyState.Stunned) return;

            switch (_currentState)
            {
                case EnemyState.Idle:
                    if (_player != null && Vector2.Distance(transform.position, _player.position) < data.DetectionRange)
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

            float dist = Vector2.Distance(transform.position, _player.position);
            if (dist <= data.AttackRange)
            {
                _currentState = EnemyState.Attack;
                _attackTimer = data.AttackCooldown;
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position, _player.position, data.MoveSpeed * Time.deltaTime);
        }

        private void HandleAttack()
        {
            if (_player == null || Vector2.Distance(transform.position, _player.position) > data.AttackRange * 1.2f)
            {
                _currentState = EnemyState.Chase;
                _isWindingUp = false;
                return;
            }

            _attackTimer -= Time.deltaTime;
            
            // Wind-up logic for perfect dodge indicator
            if (_attackTimer <= data.WindUpTime && !_isWindingUp)
            {
                _isWindingUp = true;
                ShowWindUpIndicator();
            }

            if (_attackTimer <= 0)
            {
                PerformAttack();
                _attackTimer = data.AttackCooldown;
                _isWindingUp = false;
            }
        }

        private void ShowWindUpIndicator()
        {
            // 빨간색 반짝임이나 느낌표 표시
            // Debug.Log("Enemy winding up attack!");
        }

        private void PerformAttack()
        {
            // 공격 처리 시 플레이어의 퍼펙트 회피 윈도우 체크
            var pCtrl = _player.GetComponent<Player.PlayerController>();
            if (pCtrl != null)
            {
                bool dodged = pCtrl.CheckPerfectDodge(Time.time);
                if (!dodged)
                {
                    // 실제 데미지 적용
                    var pStats = _player.GetComponent<Player.PlayerStats>();
                    if (pStats != null) pStats.TakeDamage(data.ATK, DamageType.Physical);
                }
            }
        }

        public void TakeDamage(float amount)
        {
            _hp -= amount;
            if (_hp <= 0 && _currentState != EnemyState.Dead)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            _currentState = EnemyState.Dead;
            EventBus.Publish(new GameEvents.EnemyKilledEvent(this));
            
            // Drop loot
            GetComponent<MiniKingdom.Items.LootManager>()?.DropLoot(transform.position);

            Destroy(gameObject, 1f); // 약간의 딜레이 후 삭제 (사망 애니메이션)
        }
    }
}
