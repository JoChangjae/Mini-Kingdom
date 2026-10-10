using UnityEngine;

namespace MiniKingdom.Combat
{
    /// <summary>
    /// Experience gem dropped by defeated enemies that magnetizes to the player.
    /// </summary>
    public class ExpOrb : MonoBehaviour
    {
        [SerializeField] private int expAmount = 15;
        [SerializeField] private float magnetRadius = 4.5f;
        [SerializeField] private float initialSpeed = 2f;

        private Transform _player;
        private bool _isAttracted = false;
        private float _currentSpeed;

        public void Setup(int amount)
        {
            expAmount = amount;
        }

        private void Start()
        {
            _currentSpeed = initialSpeed;
            FindPlayer();

            // Slight random initial impulse
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Random.insideUnitCircle * 2f, ForceMode2D.Impulse);
            }
        }

        private void FindPlayer()
        {
            var pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) _player = pObj.transform;
            else
            {
                var pCtrl = FindFirstObjectByType<Player.PlayerController>();
                if (pCtrl != null) _player = pCtrl.transform;
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                FindPlayer();
                return;
            }

            float dist = Vector2.Distance(transform.position, _player.position);

            if (!_isAttracted && dist <= magnetRadius)
            {
                _isAttracted = true;
            }

            if (_isAttracted)
            {
                _currentSpeed += Time.deltaTime * 18f;
                transform.position = Vector2.MoveTowards(transform.position, _player.position, _currentSpeed * Time.deltaTime);

                if (dist <= 0.5f)
                {
                    Collect();
                }
            }
        }

        private void Collect()
        {
            LevelUpSystem.Instance?.AddExp(expAmount);
            Destroy(gameObject);
        }
    }
}
