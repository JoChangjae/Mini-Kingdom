using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using MiniKingdom.Core;
using MiniKingdom.Player;

namespace MiniKingdom.UI
{
    public class DungeonHUDScreen : ScreenBase
    {
        [Header("Top Bar")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button speedToggleBtn;
        [SerializeField] private TextMeshProUGUI speedText;

        [Header("Boss/Combo")]
        [SerializeField] private GameObject bossHpPanel;
        [SerializeField] private Slider bossHpBar;
        [SerializeField] private TextMeshProUGUI bossNameText;
        [SerializeField] private GameObject comboPanel;
        [SerializeField] private TextMeshProUGUI comboText;

        [Header("Skills")]
        [SerializeField] private SkillButton skill1Btn;
        [SerializeField] private SkillButton skill2Btn;
        [SerializeField] private SkillButton dodgeBtn;

        [Header("Indicators")]
        [SerializeField] private Image perfectDodgeFlash;

        private float _currentSpeed = 1f;
        private PlayerStats _playerStats;

        protected override void OnScreenShow()
        {
            if (bossHpPanel != null) bossHpPanel.SetActive(false);
            if (comboPanel != null) comboPanel.SetActive(false);
            if (perfectDodgeFlash != null) perfectDodgeFlash.color = new Color(1, 1, 1, 0);

            if (pauseButton != null) pauseButton.onClick.AddListener(OnPauseClicked);
            if (speedToggleBtn != null) speedToggleBtn.onClick.AddListener(OnSpeedToggleClicked);

            EventBus.Subscribe<PerfectDodgeEvent>(OnPerfectDodgeEvent);
            EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Subscribe<PlayerHealedEvent>(OnPlayerHealed);

            _playerStats = FindObjectOfType<PlayerStats>();
            if (_playerStats != null)
            {
                UpdateHP(Mathf.RoundToInt(_playerStats.CurrentHP), Mathf.RoundToInt(_playerStats.CalculateFinalStat(StatType.MaxHP)));
            }
        }

        protected override void OnScreenHide()
        {
            if (pauseButton != null) pauseButton.onClick.RemoveAllListeners();
            if (speedToggleBtn != null) speedToggleBtn.onClick.RemoveAllListeners();

            EventBus.Unsubscribe<PerfectDodgeEvent>(OnPerfectDodgeEvent);
            EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Unsubscribe<PlayerHealedEvent>(OnPlayerHealed);
        }

        private void OnPerfectDodgeEvent(PerfectDodgeEvent e)
        {
            TriggerPerfectDodge();
        }

        private void OnPlayerDamaged(PlayerDamagedEvent e)
        {
            if (_playerStats != null)
            {
                UpdateHP(Mathf.RoundToInt(e.RemainingHP), Mathf.RoundToInt(_playerStats.CalculateFinalStat(StatType.MaxHP)));
            }
        }

        private void OnPlayerHealed(PlayerHealedEvent e)
        {
            if (_playerStats != null)
            {
                UpdateHP(Mathf.RoundToInt(e.CurrentHP), Mathf.RoundToInt(_playerStats.CalculateFinalStat(StatType.MaxHP)));
            }
        }

        protected override void OnScreenUpdate()
        {
            if (_playerStats == null)
            {
                _playerStats = FindObjectOfType<PlayerStats>();
            }

            if (_playerStats != null && hpBar != null)
            {
                float maxHp = _playerStats.CalculateFinalStat(StatType.MaxHP);
                UpdateHP(Mathf.RoundToInt(_playerStats.CurrentHP), Mathf.RoundToInt(maxHp));
            }
        }

        public void UpdateHP(int current, int max)
        {
            if (hpBar != null && max > 0) hpBar.value = (float)current / max;
            if (hpText != null) hpText.text = $"{current}/{max}";
        }

        public void ShowBossHP(string bossName, float current, float max)
        {
            if (bossHpPanel != null) bossHpPanel.SetActive(true);
            if (bossNameText != null) bossNameText.text = bossName;
            UpdateBossHP(current, max);
        }

        public void UpdateBossHP(float current, float max)
        {
            if (bossHpBar != null && max > 0)
            {
                bossHpBar.value = Mathf.Clamp01(current / max);
            }
        }

        public void HideBossHP()
        {
            if (bossHpPanel != null) bossHpPanel.SetActive(false);
        }

        public void ShowCombo(int count)
        {
            if (comboPanel != null) comboPanel.SetActive(true);
            if (comboText != null)
            {
                comboText.text = $"{count} Hits!";
                UIAnimations.PunchScale(comboText.transform);
            }
        }

        public void TriggerPerfectDodge()
        {
            if (perfectDodgeFlash != null)
            {
                StartCoroutine(FlashPerfectDodge());
            }
        }

        private IEnumerator FlashPerfectDodge()
        {
            if (perfectDodgeFlash == null) yield break;
            perfectDodgeFlash.color = new Color(1f, 0.9f, 0.2f, 0.6f); // Golden yellow flash
            yield return new WaitForSecondsRealtime(0.15f);
            perfectDodgeFlash.color = new Color(1f, 0.9f, 0.2f, 0f);
        }

        private void OnPauseClicked()
        {
            UIManager.Instance?.Show(ScreenType.Pause);
        }

        private void OnSpeedToggleClicked()
        {
            _currentSpeed = _currentSpeed == 1f ? 2f : 1f;
            if (speedText != null) speedText.text = $"x{_currentSpeed}";
            Time.timeScale = _currentSpeed;
        }
    }
}
