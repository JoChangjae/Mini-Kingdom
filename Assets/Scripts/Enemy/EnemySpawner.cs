using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Data;
using MiniKingdom.Core;
using MiniKingdom.Dungeon;

namespace MiniKingdom.Enemy
{
    /// <summary>
    /// Spawns normal enemy waves or epic boss encounters for dungeon rooms.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Enemy Prefabs")]
        [SerializeField] private GameObject defaultEnemyPrefab;
        [SerializeField] private GameObject wolfEnemyPrefab;
        [SerializeField] private GameObject bossPrefab;

        private int _aliveEnemies = 0;
        private int _currentWave = 0;
        private DungeonData _dungeonData;
        private Coroutine _spawnRoutine;

        public void StartSpawning(DungeonData data, int roomDifficulty, MiniKingdom.Dungeon.RoomType roomType = MiniKingdom.Dungeon.RoomType.Combat)
        {
            _dungeonData = data;
            _currentWave = 0;
            _aliveEnemies = 0;

            if (_spawnRoutine != null) StopCoroutine(_spawnRoutine);

            if (roomType == MiniKingdom.Dungeon.RoomType.Boss)
            {
                _spawnRoutine = StartCoroutine(SpawnBossRoutine());
            }
            else
            {
                _spawnRoutine = StartCoroutine(SpawnWaveRoutine(roomDifficulty));
            }
        }

        private IEnumerator SpawnBossRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            Vector2 spawnPos = (Vector2)transform.position + new Vector2(0f, 2.5f);
            GameObject prefab = bossPrefab;
            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>("Prefabs/Entities/Boss_Troll");
#if UNITY_EDITOR
                if (prefab == null)
                {
                    prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entities/Boss_Troll.prefab");
                }
#endif
            }

            if (prefab != null)
            {
                var bossObj = Instantiate(prefab, spawnPos, Quaternion.identity);
                bossObj.tag = "Enemy";
                _aliveEnemies = 1;
                Debug.Log("[EnemySpawner] 👹 숲 트롤 보스 스폰 완료!");
            }
            else
            {
                Debug.LogWarning("[EnemySpawner] Boss_Troll 프리팹을 찾을 수 없어 방을 즉시 클리어합니다.");
                EventBus.Publish(new RoomClearedEvent());
            }
        }

        private IEnumerator SpawnWaveRoutine(int difficulty)
        {
            int totalWaves = 2;

            while (_currentWave < totalWaves)
            {
                int count = 2 + difficulty + _currentWave;
                for (int i = 0; i < count; i++)
                {
                    bool spawnWolf = (_currentWave > 0 && i % 2 == 1);
                    SpawnSingleEnemy(spawnWolf);
                    yield return new WaitForSeconds(0.4f);
                }

                _currentWave++;

                // Wait for all alive enemies to be defeated
                yield return new WaitUntil(() => _aliveEnemies <= 0);
                yield return new WaitForSeconds(1.2f); // Wave delay
            }

            // Room Clear!
            Debug.Log("[EnemySpawner] ⚔️ 모든 적 처치 완료! RoomClearedEvent 발행");
            EventBus.Publish(new RoomClearedEvent());
        }

        private void SpawnSingleEnemy(bool isWolf)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle.normalized * Random.Range(3f, 6.5f);

            GameObject prefab = isWolf ? wolfEnemyPrefab : defaultEnemyPrefab;
            if (prefab == null)
            {
                string path = isWolf ? "Prefabs/Entities/Enemy_Wolf" : "Prefabs/Entities/Enemy_Slime";
                prefab = Resources.Load<GameObject>(path);
#if UNITY_EDITOR
                if (prefab == null)
                {
                    string assetPath = isWolf ? "Assets/Prefabs/Entities/Enemy_Wolf.prefab" : "Assets/Prefabs/Entities/Enemy_Slime.prefab";
                    prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                }
#endif
            }

            if (prefab != null)
            {
                var enemyObj = Instantiate(prefab, spawnPos, Quaternion.identity);
                enemyObj.tag = "Enemy";
                _aliveEnemies++;
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
            EventBus.Subscribe<BossKilledEvent>(OnBossKilled);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
            EventBus.Unsubscribe<BossKilledEvent>(OnBossKilled);
        }

        private void OnEnemyKilled(EnemyKilledEvent e)
        {
            _aliveEnemies--;
            if (_aliveEnemies < 0) _aliveEnemies = 0;
        }

        private void OnBossKilled(BossKilledEvent e)
        {
            _aliveEnemies = 0;
            EventBus.Publish(new RoomClearedEvent());
        }
    }
}
