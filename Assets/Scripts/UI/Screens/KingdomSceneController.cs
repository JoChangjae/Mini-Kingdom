using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniKingdom.UI
{
    /// <summary>
    /// Controller for the Kingdom scene to handle primary scene-level interactions.
    /// </summary>
    public class KingdomSceneController : MonoBehaviour
    {
        private bool _isDeparting = false;

        public void OnClickDepartDungeon()
        {
            if (_isDeparting) return;
            _isDeparting = true;

            Debug.Log("[KingdomSceneController] ⚔️ 던전 출정하기 버튼 클릭 -> Dungeon 씬으로 전환");
            if (Application.CanStreamedLevelBeLoaded("Dungeon"))
            {
                SceneManager.LoadScene("Dungeon");
            }
            else
            {
                Debug.LogWarning("[KingdomSceneController] Dungeon 씬을 로드할 수 없습니다. Build Settings를 확인하세요.");
                _isDeparting = false;
            }
        }
    }
}
