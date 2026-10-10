using UnityEngine;
using MiniKingdom.Data;
using MiniKingdom.Combat;

namespace MiniKingdom.Enemy
{
    /// <summary>
    /// Agile predator enemy that circles the player and performs rapid lunging bite attacks.
    /// </summary>
    public class WolfEnemyController : EnemyController
    {
        [Header("Wolf Specifics")]
        [SerializeField] private float lungeRange = 4.5f;
        [SerializeField] private float lungeSpeed = 12f;
        [SerializeField] private float lungeDuration = 0.25f;
        [SerializeField] private float lungeCooldown = 3f;

        private float _lastLungeTime = -99f;
        private bool _isLunging = false;
        private float _lungeTimer = 0f;
        private Vector2 _lungeDirection;
        private Rigidbody2D _rb;

        protected override void Start()
        {
            base.Start();
            _rb = GetComponent<Rigidbody2D>();
        }

        protected override void Update()
        {
            if (_currentState == EnemyState.Dead || _currentState == EnemyState.Stunned) return;

            if (_isLunging)
            {
                HandleLunge();
                return;
            }

            if (_player != null && Time.time >= _lastLungeTime + lungeCooldown)
            {
                float dist = Vector2.Distance(transform.position, _player.position);
                if (dist <= lungeRange && dist > 1.2f)
                {
                    StartCoroutine(PrepareLungeRoutine());
                    return;
                }
            }

            base.Update();
        }

        private System.Collections.IEnumerator PrepareLungeRoutine()
        {
            _lastLungeTime = Time.time;
            _currentState = EnemyState.Attack;

            // Telegraph: brief pause and red warning flash
            var sr = GetComponentInChildren<SpriteRenderer>();
            Color original = sr != null ? sr.color : Color.white;
            if (sr != null) sr.color = new Color(1f, 0.2f, 0.2f);

            yield return new WaitForSeconds(0.35f);

            if (_currentState == EnemyState.Dead) yield break;

            if (sr != null) sr.color = original;

            if (_player != null)
            {
                _lungeDirection = ((Vector2)_player.position - (Vector2)transform.position).normalized;
                _isLunging = true;
                _lungeTimer = lungeDuration;
            }
            else
            {
                _currentState = EnemyState.Idle;
            }
        }

        private void HandleLunge()
        {
            _lungeTimer -= Time.deltaTime;
            if (_rb != null)
            {
                _rb.linearVelocity = _lungeDirection * lungeSpeed;
            }
            else
            {
                transform.position += (Vector3)(_lungeDirection * (lungeSpeed * Time.deltaTime));
            }

            // Check collision with player
            if (_player != null && Vector2.Distance(transform.position, _player.position) <= 0.9f)
            {
                var pCtrl = _player.GetComponent<Player.PlayerController>();
                if (pCtrl != null && !pCtrl.CheckPerfectDodge(Time.time))
                {
                    var pStats = _player.GetComponent<Player.PlayerStats>();
                    float atk = data != null ? data.ATK * 1.3f : 15f;
                    pStats?.TakeDamage(atk, DamageType.Physical);
                    CameraShake.Shake(0.12f, 0.1f);
                }
                _lungeTimer = 0f;
            }

            if (_lungeTimer <= 0)
            {
                _isLunging = false;
                if (_rb != null) _rb.linearVelocity = Vector2.zero;
                _currentState = EnemyState.Chase;
            }
        }
    }
}
