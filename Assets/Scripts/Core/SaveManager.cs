using System;
using System.IO;
using UnityEngine;

namespace MiniKingdom.Core
{
    [Serializable]
    public class SaveData
    {
        public string KingdomData = "{}";
        public string PlayerData = "{}";
        public string InventoryData = "{}";
        public string ProgressData = "{}";
        public string SettingsData = "{}";
        public string DiscoveryBookData = "{}";
    }

    /// <summary>
    /// Manages saving and loading game data to the local device.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private const string SAVE_FILE_NAME = "save_main.json";
        private const int MAX_BACKUPS = 3;

        private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        
        public SaveData CurrentSaveData { get; private set; }

        private void Awake()
        {
            LoadGame();
        }

        /// <summary>
        /// Saves the game data to persistent storage.
        /// </summary>
        public void SaveGame()
        {
            if (CurrentSaveData == null) CurrentSaveData = new SaveData();

            try
            {
                // 백업 생성 로직 (이전 3개 유지)
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
                // 새 저장 데이터 생성
                CurrentSaveData = new SaveData();
                Debug.Log("[SaveManager] 저장 파일이 없습니다. 새 데이터를 생성합니다.");
            }
        }

        private void CreateBackup()
        {
            if (!File.Exists(SavePath)) return;

            // 백업 파일 밀어내기
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
