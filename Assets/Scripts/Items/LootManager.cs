using UnityEngine;
using MiniKingdom.Data;

namespace MiniKingdom.Items
{
    /// <summary>
    /// Handles enemy drops and loot logic.
    /// </summary>
    public class LootManager : MonoBehaviour
    {
        [SerializeField] private LootTable lootTable;

        public void DropLoot(Vector3 position)
        {
            if (lootTable == null) return;

            foreach (var item in lootTable.Items)
            {
                if (Random.value <= item.DropChance)
                {
                    SpawnLootVisual(position, item);
                }
            }
        }

        private void SpawnLootVisual(Vector3 pos, LootEntry item)
        {
            // 인게임 자원 아이콘을 떨어뜨리고 플레이어에게 날아가는 연출
            // ObjectPool 이용 권장
            // 획득 시 PlayerInventory.AddResource 호출
        }
    }
}
