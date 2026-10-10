using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;
using MiniKingdom.UI;
using MiniKingdom.Utils;

namespace MiniKingdom.Combat
{
    /// <summary>
    /// Singleton managing combat formulas, combos, and damage text.
    /// </summary>
    public class CombatSystem : Singleton<CombatSystem>
    {
        [Header("Damage Text")]
        [SerializeField] private FloatingText floatingTextPrefab;
        [SerializeField] private Canvas worldSpaceCanvas;

        private int _comboCount;
        private float _lastHitTime;
        private float _comboTimeout = 3f;
        
        private int _killsThisRun;

        public int ComboCount => _comboCount;
        public int KillsThisRun => _killsThisRun;

        private void Update()
        {
            if (_comboCount > 0 && Time.time > _lastHitTime + _comboTimeout)
            {
                ResetCombo();
            }
        }

        public void ProcessAttack(GameObject attacker, GameObject defender, float baseDamage, DamageType type, float skillMultiplier = 1f)
        {
            if (defender == null) return;

            // 방어력 및 적 속성 확인
            float def = 5f; 
            var enemyCtrl = defender.GetComponent<Enemy.EnemyController>();
            float typeMod = 1.0f;

            if (enemyCtrl != null)
            {
                def = enemyCtrl.DEF;
                typeMod = CalculateElementalModifier(type, enemyCtrl.Data);
            }

            // 크리티컬 (플레이어인 경우)
            float critMult = 1f;
            bool isCrit = false;
            var pStats = attacker != null ? attacker.GetComponent<Player.PlayerStats>() : null;
            if (pStats != null)
            {
                if (Random.value < pStats.CalculateFinalStat(Player.StatType.CRT))
                {
                    critMult = pStats.CalculateFinalStat(Player.StatType.CDMG);
                    isCrit = true;
                }
            }

            // 데미지 공식 계산 (GDD 1.2)
            float defReduction = 100f / (100f + Mathf.Max(0, def));
            float randomMod = Random.Range(0.9f, 1.1f);
            
            float finalDamage = baseDamage * skillMultiplier * defReduction * typeMod * critMult * randomMod;
            finalDamage = Mathf.Max(1f, finalDamage);

            // 적용
            if (enemyCtrl != null)
            {
                enemyCtrl.TakeDamage(finalDamage);
            }
            else
            {
                var pDefStats = defender.GetComponent<Player.PlayerStats>();
                if (pDefStats != null) pDefStats.TakeDamage(finalDamage, type);
            }

            // 콤보 증가 (플레이어 공격시)
            if (pStats != null)
            {
                _comboCount++;
                _lastHitTime = Time.time;

                var hud = FindObjectOfType<DungeonHUDScreen>();
                if (hud != null && _comboCount >= 3)
                {
                    hud.ShowCombo(_comboCount);
                }
            }

            // 데미지 텍스트 띄우기
            SpawnDamageText(defender.transform.position, finalDamage, isCrit, type);
        }

        public void RegisterKill()
        {
            _killsThisRun++;
        }

        public void ResetCombo()
        {
            _comboCount = 0;
        }

        public void ResetRunStats()
        {
            _killsThisRun = 0;
            _comboCount = 0;
        }

        private float CalculateElementalModifier(DamageType attackType, EnemyData enemyData)
        {
            if (enemyData == null) return 1.0f;

            // 약점 속성: 데미지 50% 증가 (+50%)
            if (enemyData.weakness == attackType)
            {
                return 1.5f;
            }

            // 저항 속성: 데미지 50% 감소 (-50%)
            if (enemyData.resistance == attackType)
            {
                return 0.5f;
            }

            return 1.0f;
        }

        private void SpawnDamageText(Vector3 pos, float damage, bool isCrit, DamageType type)
        {
            Color color = Color.white;
            if (isCrit)
            {
                color = new Color(1f, 0.85f, 0.2f); // Golden yellow
            }
            else
            {
                switch (type)
                {
                    case DamageType.Magic:
                        color = new Color(0.7f, 0.4f, 1f); // Purple/Magic
                        break;
                    case DamageType.Nature:
                        color = new Color(0.4f, 0.9f, 0.4f); // Green/Nature
                        break;
                    default:
                        color = Color.white; // Physical
                        break;
                }
            }

            string text = isCrit ? $"CRIT! {Mathf.RoundToInt(damage)}" : $"{Mathf.RoundToInt(damage)}";

            if (floatingTextPrefab == null)
            {
                floatingTextPrefab = Resources.Load<FloatingText>("Prefabs/UI/FloatingText");
#if UNITY_EDITOR
                if (floatingTextPrefab == null)
                {
                    floatingTextPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<FloatingText>("Assets/Prefabs/UI/FloatingText.prefab");
                }
#endif
            }

            if (floatingTextPrefab != null)
            {
                Canvas targetCanvas = worldSpaceCanvas;
                if (targetCanvas == null)
                {
                    targetCanvas = FindFirstObjectByType<Canvas>();
                }
                var ft = Instantiate(floatingTextPrefab, targetCanvas != null ? targetCanvas.transform : null);
                ft.Setup(text, color, pos);
            }
        }
    }
}
