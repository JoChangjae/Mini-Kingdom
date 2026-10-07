using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniKingdom.UI
{
    public class DiscoveryBookScreen : ScreenBase
    {
        [Header("Tabs")]
        [SerializeField] private Button monstersTab;
        [SerializeField] private Button equipmentTab;
        [SerializeField] private Button buildingsTab;

        [Header("Content")]
        [SerializeField] private Transform itemGridContainer;
        [SerializeField] private GameObject discoveryItemPrefab;
        [SerializeField] private TextMeshProUGUI completionText;

        [Header("Detail Panel")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private TextMeshProUGUI detailNameText;
        [SerializeField] private TextMeshProUGUI detailLoreText;

        protected override void OnScreenShow()
        {
            detailPanel.SetActive(false);

            monstersTab.onClick.AddListener(() => LoadCategory("Monsters"));
            equipmentTab.onClick.AddListener(() => LoadCategory("Equipment"));
            buildingsTab.onClick.AddListener(() => LoadCategory("Buildings"));

            LoadCategory("Monsters"); // Default tab
        }

        protected override void OnScreenHide()
        {
            monstersTab.onClick.RemoveAllListeners();
            equipmentTab.onClick.RemoveAllListeners();
            buildingsTab.onClick.RemoveAllListeners();
        }

        protected override void OnScreenUpdate() {}

        private void LoadCategory(string category)
        {
            foreach (Transform t in itemGridContainer) Destroy(t.gameObject);

            completionText.text = $"{category} 수집률: 45%";

            // Dummy data
            for (int i = 0; i < 20; i++)
            {
                var item = Instantiate(discoveryItemPrefab, itemGridContainer);
                var btn = item.GetComponent<Button>();
                bool discovered = Random.value > 0.5f;
                
                if (discovered)
                {
                    btn.onClick.AddListener(() => ShowDetail($"항목 {i}", "발견된 아이템/몬스터에 대한 설명입니다."));
                }
                else
                {
                    // 실루엣 처리 (Image 색상 어둡게)
                    var img = item.GetComponent<Image>();
                    if (img) img.color = Color.black;
                }
            }
        }

        private void ShowDetail(string itemName, string lore)
        {
            detailNameText.text = itemName;
            detailLoreText.text = lore;
            detailPanel.SetActive(true);
            UIAnimations.ScaleBounce(detailPanel.transform);
        }
    }
}
