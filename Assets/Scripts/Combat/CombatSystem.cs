using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;

namespace MiniKingdom.Combat
{
    public enum DamageType { Physical, Magic, Nature }

    /// <summary>
    /// Singleton managing combat formulas, combos, and damage text.
    /// </summary>
    public class CombatSystem : Singleton<CombatSystem>
    {
        private int _comboCount;
        private float _lastHitTime;
        private float _comboTimeout = 3f;
        
        private int _killsThisRun;

        private void Update()
        {
            if (_comboCount > 0 && Time.time > _lastHitTime + _comboTimeout)
            {
                ResetCombo();
            }
        }

        public void ProcessAttack(GameObject attacker, GameObject defender, float baseDamage, DamageType type, float skillMultiplier = 1f)
        {
            // 방어력 가져오기 (가상의 컴포넌트)
            float def = 5f; 
            var enemyStats = defender.GetComponent<Enemy.EnemyController>();
            if (enemyStats != null) def = enemyStats.DEF;

            // 속성 상성
            float typeMod = GetTypeModifier(type, DamageType.Physical); // 적의 속성을 안다면 대체

            // 크리티컬 (플레이어인 경우)
            float critMult = 1f;
            var pStats = attacker.GetComponent<Player.PlayerStats>();
            if (pStats != null)
            {
                if (Random.value < pStats.CalculateFinalStat(Player.StatType.CRT))
                {
                    critMult = pStats.CalculateFinalStat(Player.StatType.CDMG);
                }
            }

            // 데미지 계산
            float defReduction = 100f / (100f + def);
            float randomMod = Random.Range(0.9f, 1.1f);
            
            float finalDamage = baseDamage * skillMultiplier * defReduction * typeMod * critMult * randomMod;

            // 적용
            if (enemyStats != null) enemyStats.TakeDamage(finalDamage);

            // 콤보 증가 (플레이어 공격시)
            if (pStats != null)
            {
                _comboCount++;
                _lastHitTime = Time.time;
            }

            // 데미지 텍스트 띄우기
            SpawnDamageText(defender.transform.position, finalDamage, critMult > 1f, type);
        }

        public void RegisterKill()
        {
            _killsThisRun++;
        }

        public void ResetCombo()
        {
            _comboCount = 0;
        }

        private float GetTypeModifier(DamageType attackType, DamageType defenseType)
        {
            // 상성 로직: Physical -> Magic (1.3), Magic -> Nature (1.3), Nature -> Physical (1.3)
            if (attackType == DamageType.Physical && defenseType == DamageType.Magic) return 1.3f;
            if (attackType == DamageType.Magic && defenseType == DamageType.Nature) return 1.3f;
            if (attackType == DamageType.Nature && defenseType == DamageType.Physical) return 1.3f;
            
            // 역상성
            if (attackType == DamageType.Physical && defenseType == DamageType.Nature) return 0.7f;
            if (attackType == DamageType.Magic && defenseType == DamageType.Physical) return 0.7f;
            if (attackType == DamageType.Nature && defenseType == DamageType.Magic) return 0.7f;

            return 1.0f; // Neutral
        }

        private void SpawnDamageText(Vector3 pos, float damage, bool isCrit, DamageType type)
        {
            // TODO: ObjectPool을 이용해 데미지 텍스트 띄우기
        }
    }
}
