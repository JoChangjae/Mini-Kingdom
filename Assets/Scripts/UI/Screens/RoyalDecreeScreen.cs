using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniKingdom.UI
{
    public class RoyalDecreeScreen : ScreenBase
    {
        [SerializeField] private Button[] decreeCards;
        [SerializeField] private Button confirmButton;
        
        private int _selectedIndex = -1;

        protected override void OnScreenShow()
        {
            _selectedIndex = -1;
            confirmButton.interactable = false;

            for (int i = 0; i < decreeCards.Length; i++)
            {
                int index = i;
                decreeCards[i].onClick.AddListener(() => OnCardSelected(index));
                // 칙령 데이터 초기화
            }

            confirmButton.onClick.AddListener(OnConfirm);
        }

        protected override void OnScreenHide()
        {
            foreach (var card in decreeCards)
            {
                card.onClick.RemoveAllListeners();
            }
            confirmButton.onClick.RemoveAllListeners();
        }

        protected override void OnScreenUpdate() {}

        private void OnCardSelected(int index)
        {
            _selectedIndex = index;
            confirmButton.interactable = true;

            for (int i = 0; i < decreeCards.Length; i++)
            {
                var border = decreeCards[i].transform.Find("HighlightBorder");
                if (border != null)
                {
                    border.gameObject.SetActive(i == index);
                }
            }
            
            UIAnimations.PunchScale(decreeCards[index].transform, 1.05f, 0.15f);
        }

        private void OnConfirm()
        {
            if (_selectedIndex >= 0)
            {
                // Save selected decree
                PopupManager.Instance.ShowToast("칙령이 선포되었습니다!");
                UIManager.Instance.Pop();
            }
        }
    }
}
