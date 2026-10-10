using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniKingdom.UI
{
    public class RoyalDecreeScreen : ScreenBase
    {
        [SerializeField] private Button[] decreeCards;
        [SerializeField] private Button confirmButton;
        [SerializeField] private TextMeshProUGUI[] decreeTitles;
        [SerializeField] private TextMeshProUGUI[] decreeDescs;

        private int _selectedIndex = -1;
        private Core.DecreeType[] _currentOptions;

        public void SetupDecrees(Core.DecreeType[] options, Button[] cards, TextMeshProUGUI[] titles, TextMeshProUGUI[] descs, Button confirm)
        {
            _currentOptions = options;
            decreeCards = cards;
            decreeTitles = titles;
            decreeDescs = descs;
            confirmButton = confirm;
        }

        protected override void OnScreenShow()
        {
            _selectedIndex = -1;
            if (confirmButton != null) confirmButton.interactable = false;

            if (_currentOptions == null || _currentOptions.Length == 0)
            {
                var decreeSys = FindAnyObjectByType<Core.RoyalDecreeSystem>();
                if (decreeSys != null)
                {
                    var opts = decreeSys.GetDailyDecreeOptions();
                    if (opts != null && opts.Count > 0) _currentOptions = opts.ToArray();
                }
            }

            if (_currentOptions == null || _currentOptions.Length == 0)
            {
                _currentOptions = new[] { Core.DecreeType.WarriorDay, Core.DecreeType.MerchantDay, Core.DecreeType.WoodDay };
            }

            if (decreeCards != null)
            {
                for (int i = 0; i < decreeCards.Length; i++)
                {
                    int index = i;
                    if (decreeCards[i] != null)
                    {
                        decreeCards[i].onClick.RemoveAllListeners();
                        decreeCards[i].onClick.AddListener(() => OnCardSelected(index));
                    }

                    if (_currentOptions != null && i < _currentOptions.Length)
                    {
                        var opt = _currentOptions[i];
                        if (decreeTitles != null && i < decreeTitles.Length && decreeTitles[i] != null)
                        {
                            decreeTitles[i].text = GetDecreeTitle(opt);
                        }
                        if (decreeDescs != null && i < decreeDescs.Length && decreeDescs[i] != null)
                        {
                            decreeDescs[i].text = GetDecreeDesc(opt);
                        }
                    }
                }
            }

            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(OnConfirm);
            }
        }

        private string GetDecreeTitle(Core.DecreeType decree)
        {
            return decree switch
            {
                Core.DecreeType.WarriorDay => "⚔️ 전사의 날",
                Core.DecreeType.MerchantDay => "💰 상인의 날",
                Core.DecreeType.WoodDay => "🌲 목재의 날",
                _ => "👑 왕실의 칙령"
            };
        }

        private string GetDecreeDesc(Core.DecreeType decree)
        {
            return decree switch
            {
                Core.DecreeType.WarriorDay => "오늘 하루 동안 모든 던전에서 기본 공격력이 20% 증가합니다.",
                Core.DecreeType.MerchantDay => "오늘 하루 동안 던전 골드 획득량이 1.5배로 증가합니다.",
                Core.DecreeType.WoodDay => "오늘 하루 동안 던전 목재 획득량이 2배로 증가합니다.",
                _ => "왕국 전역에 특별한 은총이 내립니다."
            };
        }

        protected override void OnScreenHide()
        {
            if (decreeCards != null)
            {
                foreach (var card in decreeCards)
                {
                    if (card != null) card.onClick.RemoveAllListeners();
                }
            }
            if (confirmButton != null) confirmButton.onClick.RemoveAllListeners();
        }

        protected override void OnScreenUpdate() {}

        private void OnCardSelected(int index)
        {
            _selectedIndex = index;
            if (confirmButton != null) confirmButton.interactable = true;

            if (decreeCards != null)
            {
                for (int i = 0; i < decreeCards.Length; i++)
                {
                    if (decreeCards[i] == null) continue;
                    var img = decreeCards[i].GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = (i == index) ? new Color(0.25f, 0.45f, 0.7f, 0.95f) : new Color(0.12f, 0.15f, 0.22f, 0.9f);
                    }
                }
                if (index < decreeCards.Length && decreeCards[index] != null)
                {
                    UIAnimations.PunchScale(decreeCards[index].transform, 1.05f, 0.15f);
                }
            }
        }

        private void OnConfirm()
        {
            if (_selectedIndex >= 0 && _currentOptions != null && _selectedIndex < _currentOptions.Length)
            {
                var chosen = _currentOptions[_selectedIndex];
                var sys = FindAnyObjectByType<Core.RoyalDecreeSystem>();
                if (sys != null)
                {
                    sys.ChooseDecree(chosen);
                }
                PopupManager.Instance?.ShowToast($"📜 [왕의 칙령 선포]: {GetDecreeTitle(chosen)}");
            }

            if (UIManager.Instance != null) UIManager.Instance.Pop();
            else gameObject.SetActive(false);
        }
    }
}
