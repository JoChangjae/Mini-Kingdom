using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MiniKingdom.Utils;
using MiniKingdom.Data;

namespace MiniKingdom.Core
{
    [Serializable]
    public class ResourceEntry
    {
        public ResourceType Type;
        public int Amount;
    }

    [Serializable]
    public class InventorySaveData
    {
        public List<ResourceEntry> Resources = new List<ResourceEntry>();
        public List<string> EquippedItemIds = new List<string>();
    }

    [Serializable]
    public class SaveData
    {
        public string KingdomData = "{}";
        public string PlayerData = "{}";
        public string InventoryData = "{}";
        public string ProgressData = "{}";
        public string SettingsData = "{}";
        public string DiscoveryBookData = "{}";
        public long lastLogoutTime;
        public List<string> AcquiredRelicIds = new List<string>();
    }

    /// <summary>
    /// Manages saving and loading game data to the local device.
    /// </summary>
    public class SaveManager : Singleton<SaveManager>
    {
        private const string SAVE_FILE_NAME = "save_main.json";
        private const int MAX_BACKUPS = 3;

        private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        
        public SaveData CurrentSaveData { get; private set; }

        public static DateTime LastLogoutTime
        {
            get
            {
                if (Instance != null && Instance.CurrentSaveData != null && Instance.CurrentSaveData.lastLogoutTime > 0)
                {
                    return DateTimeOffset.FromUnixTimeSeconds(Instance.CurrentSaveData.lastLogoutTime).LocalDateTime;
                }
                return DateTime.MinValue;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (this != Instance) return;
            LoadGame();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            RecordLogout();
            SaveGame();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                RecordLogout();
                SaveGame();
            }
        }

        private void RecordLogout()
        {
            if (CurrentSaveData == null) CurrentSaveData = new SaveData();
            CurrentSaveData.lastLogoutTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Saves the game data to persistent storage.
        /// </summary>
        public void SaveGame()
        {
            if (CurrentSaveData == null) CurrentSaveData = new SaveData();

            try
            {
                CreateBackup();

                string json = JsonUtility.ToJson(CurrentSaveData, true);
                File.WriteAllText(SavePath, json);
                Debug.Log($"[SaveManager] 게임이 저장되었습니다. 경로: {SavePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 저장 중 오류 발생: {e.Message}");
            }
        }

        /// <summary>
        /// Loads the game data from persistent storage.
        /// </summary>
        public void LoadGame()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    string json = File.ReadAllText(SavePath);
                    CurrentSaveData = JsonUtility.FromJson<SaveData>(json);
                    Debug.Log("[SaveManager] 게임 데이터를 성공적으로 불러왔습니다.");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SaveManager] 불러오기 중 오류 발생: {e.Message}");
                    CurrentSaveData = new SaveData();
                }
            }
            else
            {
                CurrentSaveData = new SaveData();
                Debug.Log("[SaveManager] 저장 파일이 없습니다. 새 데이터를 생성합니다.");
            }
        }

        public static void SaveInventory(Dictionary<ResourceType, int> resources, Dictionary<Player.EquipmentSlot, EquipmentData> equippedItems)
        {
            if (Instance == null) return;
            if (Instance.CurrentSaveData == null) Instance.CurrentSaveData = new SaveData();

            var invData = new InventorySaveData();
            if (resources != null)
            {
                foreach (var kvp in resources)
                {
                    invData.Resources.Add(new ResourceEntry { Type = kvp.Key, Amount = kvp.Value });
                }
            }

            Instance.CurrentSaveData.InventoryData = JsonUtility.ToJson(invData);
            Instance.SaveGame();
        }

        public static InventorySaveData LoadInventory()
        {
            if (Instance == null || Instance.CurrentSaveData == null) return new InventorySaveData();
            if (string.IsNullOrEmpty(Instance.CurrentSaveData.InventoryData)) return new InventorySaveData();

            try
            {
                return JsonUtility.FromJson<InventorySaveData>(Instance.CurrentSaveData.InventoryData) ?? new InventorySaveData();
            }
            catch
            {
                return new InventorySaveData();
            }
        }

        private void CreateBackup()
        {
            if (!File.Exists(SavePath)) return;

            for (int i = MAX_BACKUPS - 1; i >= 1; i--)
            {
                string oldBackup = Path.Combine(Application.persistentDataPath, $"save_backup_{i}.json");
                string newBackup = Path.Combine(Application.persistentDataPath, $"save_backup_{i + 1}.json");

                if (File.Exists(oldBackup))
                {
                    if (File.Exists(newBackup)) File.Delete(newBackup);
                    File.Move(oldBackup, newBackup);
                }
            }

            string firstBackup = Path.Combine(Application.persistentDataPath, "save_backup_1.json");
            if (File.Exists(firstBackup)) File.Delete(firstBackup);
            File.Copy(SavePath, firstBackup);
        }
    }
}
