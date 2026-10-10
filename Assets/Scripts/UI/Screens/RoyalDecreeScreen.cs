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

            var decreeSys = FindAnyObjectByType<Core.RoyalDecreeSystem>();
            if (decreeSys != null)
            {
                var opts = decreeSys.GetDailyDecreeOptions();
                if (opts != null && opts.Count >= 3) _currentOptions = opts.ToArray();
            }

            if (_currentOptions == null || _currentOptions.Length == 0)
            {
                _currentOptions = new[] { Core.DecreeType.WarriorDay, Core.DecreeType.MerchantDay, Core.DecreeType.WoodDay };
            }

            bool alreadyChosen = decreeSys != null && decreeSys.ActiveDecree != Core.DecreeType.None;

            if (decreeCards != null)
            {
                for (int i = 0; i < decreeCards.Length; i++)
                {
                    int index = i;
                    if (decreeCards[i] != null)
                    {
                        decreeCards[i].gameObject.SetActive(true);
                        decreeCards[i].onClick.RemoveAllListeners();
                        if (!alreadyChosen)
                        {
                            decreeCards[i].onClick.AddListener(() => OnCardSelected(index));
                        }
                    }

                    if (_currentOptions != null && i < _currentOptions.Length)
                    {
                        var opt = _currentOptions[i];
                        bool isThisActive = (decreeSys != null && decreeSys.ActiveDecree == opt);

                        if (decreeTitles != null && i < decreeTitles.Length && decreeTitles[i] != null)
                        {
                            decreeTitles[i].text = isThisActive ? $"[선포됨]\n{GetDecreeTitle(opt)}" : GetDecreeTitle(opt);
                        }
                        if (decreeDescs != null && i < decreeDescs.Length && decreeDescs[i] != null)
                        {
                            decreeDescs[i].text = GetDecreeDesc(opt);
                        }

                        if (decreeCards != null && decreeCards[i] != null)
                        {
                            var img = decreeCards[i].GetComponent<Image>();
                            if (img != null)
                            {
                                img.color = isThisActive 
                                    ? new Color(0.2f, 0.5f, 0.3f, 0.95f) 
                                    : new Color(0.12f, 0.15f, 0.22f, 0.9f);
                            }
                        }
                    }
                }
            }

            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveAllListeners();
                var btnTxt = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
                if (alreadyChosen)
                {
                    confirmButton.interactable = false;
                    if (btnTxt != null) btnTxt.text = "오늘의 칙령 선포 완료";
                }
                else
                {
                    confirmButton.interactable = false;
                    if (btnTxt != null) btnTxt.text = "칙령 선포하기";
                    confirmButton.onClick.AddListener(OnConfirm);
                }
            }
        }

        private string GetDecreeTitle(Core.DecreeType decree)
        {
            return decree switch
            {
                Core.DecreeType.WarriorDay => "⚔️ 전사의 날",
                Core.DecreeType.MerchantDay => "💰 상인의 날",
                Core.DecreeType.WoodDay => "🌲 목재의 날",
                Core.DecreeType.ScholarDay => "📖 학자의 날",
                Core.DecreeType.BuilderDay => "🔨 건축의 날",
                Core.DecreeType.HealerDay => "❤️ 치유의 날",
                Core.DecreeType.ExplorerDay => "🗺️ 탐험의 날",
                Core.DecreeType.FortuneDay => "🍀 행운의 날",
                _ => "👑 왕실의 칙령"
            };
        }

        private string GetDecreeDesc(Core.DecreeType decree)
        {
            return decree switch
            {
                Core.DecreeType.WarriorDay => "오늘 던전에서 플레이어 기본 공격력이 20% 증가합니다.",
                Core.DecreeType.MerchantDay => "오늘 던전 골드 획득량이 1.5배로 증가합니다.",
                Core.DecreeType.WoodDay => "오늘 던전 목재 획득량이 2배로 증가합니다.",
                Core.DecreeType.ScholarDay => "던전 내 스킬 획득 시 경험치 획득량이 30% 증가합니다.",
                Core.DecreeType.BuilderDay => "왕국 건물 업그레이드 비용이 20% 할인됩니다.",
                Core.DecreeType.HealerDay => "던전 시작 시 최대 체력이 25% 증가하고 자연 회복이 활성화됩니다.",
                Core.DecreeType.ExplorerDay => "던전 방 이동 시 이동속도 및 회피율이 15% 상승합니다.",
                Core.DecreeType.FortuneDay => "치명타 확률 +15% 및 희귀 아이템 드랍 확률이 상승합니다.",
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
