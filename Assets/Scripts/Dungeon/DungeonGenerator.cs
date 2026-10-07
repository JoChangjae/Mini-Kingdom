using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Data;

namespace MiniKingdom.Dungeon
{
    public enum RoomType { Entrance, Combat, Shop, Rest, Event, Treasure, Boss }

    public class RoomConfig
    {
        public RoomType Type;
        public int Difficulty;
        public bool IsBranch;
        public RoomConfig NextA;
        public RoomConfig NextB;
    }

    /// <summary>
    /// Generates dungeon layout procedurally.
    /// </summary>
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] private DungeonData dungeonData;
        [SerializeField] private int roomCount = 12;

        public RoomConfig Generate()
        {
            RoomConfig entrance = new RoomConfig { Type = RoomType.Entrance, Difficulty = 0 };
            RoomConfig current = entrance;

            int midPoint = roomCount / 2;

            for (int i = 1; i < roomCount; i++)
            {
                RoomConfig nextRoom = new RoomConfig();
                nextRoom.Difficulty = i;

                if (i == roomCount - 1)
                {
                    nextRoom.Type = RoomType.Boss;
                }
                else if (i == midPoint)
                {
                    // Guaranteed rest or shop
                    nextRoom.Type = Random.value > 0.5f ? RoomType.Rest : RoomType.Shop;
                }
                else
                {
                    // Weighted random based on data
                    nextRoom.Type = GetRandomRoomType();

                    // 30% chance for branch path
                    if (Random.value < 0.3f)
                    {
                        nextRoom.IsBranch = true;
                        nextRoom.NextB = new RoomConfig { Type = GetRandomRoomType(), Difficulty = i };
                    }
                }

                current.NextA = nextRoom;
                current = nextRoom;
            }

            return entrance;
        }

        private RoomType GetRandomRoomType()
        {
            // 간단한 가중치 랜덤
            float rnd = Random.value;
            if (rnd < 0.6f) return RoomType.Combat;
            if (rnd < 0.8f) return RoomType.Event;
            return RoomType.Treasure;
        }
    }
}
