using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Combat;
using MiniKingdom.Data;
using MiniKingdom.Core;
using MiniKingdom.UI;

namespace MiniKingdom.Player
{
    /// <summary>
    /// Manages player's active skill rotations, automated casting, and visual skill effects.
    /// </summary>
    public class PlayerSkillController : MonoBehaviour
    {
        private Dictionary<string, float> _skillCooldowns = new Dictionary<string, float>();
        private Dictionary<string, SkillData> _activeSkills = new Dictionary<string, SkillData>();
        private PlayerStats _stats;

        // Barrier state
        private bool _isBarrierActive = false;
        private float _barrierTimer = 0f;

        // Tornado state
        private bool _isTornadoActive = false;
        private float _tornadoTimer = 0f;
        private float _tornadoTickTimer = 0f;

        private void Start()
        {
            _stats = GetComponent<PlayerStats>();
            EventBus.Subscribe<UpgradeChosenEvent>(OnSkillUpgraded);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<UpgradeChosenEvent>(OnSkillUpgraded);
        }

        private void OnSkillUpgraded(UpgradeChosenEvent e)
        {
            RefreshSkillsFromSystem();
        }

        public void RefreshSkillsFromSystem()
        {
            if (LevelUpSystem.Instance == null) return;

            var allSkills = LevelUpSystem.Instance.GetPlayerSkills();
            foreach (var kvp in allSkills)
            {
                string skillId = kvp.Key;
                SkillData skillData = kvp.Value;

                if (!_activeSkills.ContainsKey(skillId))
                {
                    _activeSkills[skillId] = skillData;
                    _skillCooldowns[skillId] = 1f; // Initial small delay
                }
            }
        }

        private void Update()
        {
            if (Time.timeScale == 0) return;

            HandleBarrier();
            HandleTornado();

            // Handle skill cooldowns and automated casting
            var keys = new List<string>(_activeSkills.Keys);
            foreach (var key in keys)
            {
                if (!_skillCooldowns.ContainsKey(key)) _skillCooldowns[key] = 0f;

                if (_skillCooldowns[key] > 0)
                {
                    _skillCooldowns[key] -= Time.deltaTime;
                }
                else
                {
                    // Check if enemies exist before triggering offensive skills
                    if (HasNearbyEnemies(6f))
                    {
                        CastSkill(_activeSkills[key]);
                        _skillCooldowns[key] = Mathf.Max(2f, _activeSkills[key].cooldown);
                    }
                }
            }
        }

        private bool HasNearbyEnemies(float range)
        {
            var enemies = FindObjectsByType<Enemy.EnemyController>(FindObjectsSortMode.None);
            foreach (var e in enemies)
            {
                if (e != null && !e.IsDead && Vector2.Distance(transform.position, e.transform.position) <= range)
                    return true;
            }
            return false;
        }

        private void CastSkill(SkillData skill)
        {
            if (skill == null) return;

            float baseAtk = _stats != null ? _stats.CalculateFinalStat(StatType.ATK) : 12f;

            switch (skill.skillId)
            {
                case "skill_flame_sword":
                    CastFlameSword(baseAtk, skill.damageMultiplier);
                    break;
                case "skill_whirlwind":
                    CastWhirlwind(baseAtk, skill.damageMultiplier);
                    break;
                case "skill_king_wrath":
                    CastKingWrath(baseAtk, skill.damageMultiplier);
                    break;
                case "skill_royal_barrier":
                    CastRoyalBarrier();
                    break;
                case "skill_fusion_flame_tornado":
                    CastFlameTornado(baseAtk, skill.damageMultiplier);
                    break;
                default:
                    CastWhirlwind(baseAtk, 1.5f);
                    break;
            }
        }

        private void CastFlameSword(float baseAtk, float multiplier)
        {
            var target = FindNearestEnemy(5f);
            if (target != null)
            {
                float damage = baseAtk * multiplier;
                CombatSystem.Instance?.ProcessAttack(gameObject, target.gameObject, damage, DamageType.Magic);
                SpawnSkillVfx(target.position, new Color(1f, 0.4f, 0.1f), 1.5f);
                CameraShake.Shake(0.15f, 0.12f);
            }
        }

        private void CastWhirlwind(float baseAtk, float multiplier)
        {
            float radius = 3.2f;
            float damage = baseAtk * multiplier;
            var enemies = FindObjectsByType<Enemy.EnemyController>(FindObjectsSortMode.None);

            foreach (var e in enemies)
            {
                if (e != null && !e.IsDead && Vector2.Distance(transform.position, e.transform.position) <= radius)
                {
                    CombatSystem.Instance?.ProcessAttack(gameObject, e.gameObject, damage, DamageType.Physical);
                }
            }

            SpawnSkillVfx(transform.position, new Color(0.3f, 0.9f, 1f), 3.2f);
            CameraShake.Shake(0.2f, 0.15f);
        }

        private void CastKingWrath(float baseAtk, float multiplier)
        {
            float damage = baseAtk * multiplier;
            var enemies = FindObjectsByType<Enemy.EnemyController>(FindObjectsSortMode.None);

            foreach (var e in enemies)
            {
                if (e != null && !e.IsDead && Vector2.Distance(transform.position, e.transform.position) <= 4.5f)
                {
                    CombatSystem.Instance?.ProcessAttack(gameObject, e.gameObject, damage, DamageType.Physical);
                }
            }

            SpawnSkillVfx(transform.position, new Color(1f, 0.85f, 0.2f), 4.5f);
            CameraShake.Shake(0.35f, 0.25f);
        }

        private void CastRoyalBarrier()
        {
            _isBarrierActive = true;
            _barrierTimer = 3.5f;
            SpawnSkillVfx(transform.position, new Color(1f, 0.9f, 0.3f, 0.5f), 1.8f);
            PopupManager.Instance?.ShowToast("🛡️ 왕실 방벽 활성화! (피해 80% 감소)");
        }

        private void CastFlameTornado(float baseAtk, float multiplier)
        {
            _isTornadoActive = true;
            _tornadoTimer = 4.0f;
            _tornadoTickTimer = 0f;
            SpawnSkillVfx(transform.position, new Color(1f, 0.3f, 0.05f), 4.0f);
            CameraShake.Shake(0.4f, 0.3f);
            PopupManager.Instance?.ShowToast("🔥 융합 스킬 [화염 토네이도] 발동!");
        }

        private void HandleBarrier()
        {
            if (_isBarrierActive)
            {
                _barrierTimer -= Time.deltaTime;
                if (_barrierTimer <= 0)
                {
                    _isBarrierActive = false;
                }
            }
        }

        private void HandleTornado()
        {
            if (_isTornadoActive)
            {
                _tornadoTimer -= Time.deltaTime;
                _tornadoTickTimer -= Time.deltaTime;

                if (_tornadoTickTimer <= 0)
                {
                    _tornadoTickTimer = 0.5f; // Tick every 0.5s
                    float baseAtk = _stats != null ? _stats.CalculateFinalStat(StatType.ATK) : 12f;
                    float tickDmg = baseAtk * 1.5f;

                    var enemies = FindObjectsByType<Enemy.EnemyController>(FindObjectsSortMode.None);
                    foreach (var e in enemies)
                    {
                        if (e != null && !e.IsDead && Vector2.Distance(transform.position, e.transform.position) <= 3.8f)
                        {
                            CombatSystem.Instance?.ProcessAttack(gameObject, e.gameObject, tickDmg, DamageType.Magic);
                        }
                    }
                    SpawnSkillVfx(transform.position, new Color(1f, 0.45f, 0.1f), 3.8f);
                }

                if (_tornadoTimer <= 0)
                {
                    _isTornadoActive = false;
                }
            }
        }

        private void SpawnSkillVfx(Vector3 pos, Color color, float scale)
        {
            GameObject vfx = new GameObject("SkillVFX");
            vfx.transform.position = pos;
            vfx.transform.localScale = Vector3.one * scale;
            var sr = vfx.AddComponent<SpriteRenderer>();

            var slashSprite = Resources.Load<Sprite>("Sprites/spr_slash");
#if UNITY_EDITOR
            if (slashSprite == null)
            {
                slashSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/spr_slash.png");
            }
#endif
            if (slashSprite != null) sr.sprite = slashSprite;
            sr.color = color;
            sr.sortingOrder = 18;

            Destroy(vfx, 0.25f);
        }

        private Transform FindNearestEnemy(float maxRange)
        {
            var enemies = FindObjectsByType<Enemy.EnemyController>(FindObjectsSortMode.None);
            float minDist = maxRange;
            Transform nearest = null;

            foreach (var e in enemies)
            {
                if (e == null || e.IsDead) continue;
                float d = Vector2.Distance(transform.position, e.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = e.transform;
                }
            }
            return nearest;
        }

        public bool IsBarrierActive => _isBarrierActive;
    }
}
