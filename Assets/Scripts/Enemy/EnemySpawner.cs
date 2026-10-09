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

        private void SpawnEnemy()
        {
            // 무작위 위치 (플레이어 주변 피해서)
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 5f;
            
            // 데이터에서 적 프리팹 가져오기
            // var prefab = _dungeonData.GetRandomEnemy();
            // var go = Instantiate(prefab, spawnPos, Quaternion.identity);
            
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
