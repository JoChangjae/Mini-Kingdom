using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniKingdom.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public void OnClickStart()
        {
            if (Application.CanStreamedLevelBeLoaded("Kingdom"))
            {
                SceneManager.LoadScene("Kingdom");
            }
            else
            {
                Debug.Log("[MainMenu] Kingdom 씬으로 전환");
            }
        }
    }
}
