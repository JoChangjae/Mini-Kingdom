using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Data;
using MiniKingdom.Core;

namespace MiniKingdom.Enemy
{
    /// <summary>
    /// Spawns enemies for a dungeon room in waves.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        private int _aliveEnemies = 0;
        private int _currentWave = 0;
        private DungeonData _dungeonData;
        
        public void StartSpawning(DungeonData data, int roomDifficulty)
        {
            _dungeonData = data;
            _currentWave = 0;
            StartCoroutine(SpawnWaveRoutine(roomDifficulty));
        }

        private IEnumerator SpawnWaveRoutine(int difficulty)
        {
            // 2 웨이브라고 가정
            int totalWaves = 2;

            while (_currentWave < totalWaves)
            {
                int enemiesToSpawn = 3 + difficulty + _currentWave;
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(0.5f);
                }

                _currentWave++;
                
                // Wait for enemies to be cleared before next wave
                yield return new WaitUntil(() => _aliveEnemies == 0);
                yield return new WaitForSeconds(2f); // Wave delay
            }

            // Room Clear!
            EventBus.Publish(new RoomClearedEvent());
        }

        [SerializeField] private GameObject defaultEnemyPrefab;
        [SerializeField] private GameObject bossPrefab;

        private void SpawnEnemy()
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle.normalized * Random.Range(3f, 6f);

            GameObject prefabToSpawn = defaultEnemyPrefab;
            if (prefabToSpawn == null)
            {
                prefabToSpawn = Resources.Load<GameObject>("Prefabs/Entities/Enemy_Slime");
                if (prefabToSpawn == null)
                {
#if UNITY_EDITOR
                    prefabToSpawn = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entities/Enemy_Slime.prefab");
#endif
                }
            }

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            }
            
            _aliveEnemies++;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKilledEvent e)
        {
            _aliveEnemies--;
            if (_aliveEnemies < 0) _aliveEnemies = 0;
        }
    }
}
