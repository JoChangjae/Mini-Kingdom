using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
        [SerializeField] private GameObject comboPanel;
        [SerializeField] private TextMeshProUGUI comboText;

        [Header("Skills")]
        [SerializeField] private SkillButton skill1Btn;
        [SerializeField] private SkillButton skill2Btn;
        [SerializeField] private SkillButton dodgeBtn;

        [Header("Indicators")]
        [SerializeField] private Image perfectDodgeFlash;

        private float _currentSpeed = 1f;

        protected override void OnScreenShow()
        {
            bossHpPanel.SetActive(false);
            comboPanel.SetActive(false);
            perfectDodgeFlash.color = new Color(1, 1, 1, 0);

            pauseButton.onClick.AddListener(OnPauseClicked);
            speedToggleBtn.onClick.AddListener(OnSpeedToggleClicked);
        }

        protected override void OnScreenHide()
        {
            pauseButton.onClick.RemoveAllListeners();
            speedToggleBtn.onClick.RemoveAllListeners();
        }

        protected override void OnScreenUpdate()
        {
            // Update HP, cooldowns, etc. from CombatSystem
        }

        public void UpdateHP(int current, int max)
        {
            hpBar.value = (float)current / max;
            hpText.text = $"{current}/{max}";
        }

        public void ShowCombo(int count)
        {
            comboPanel.SetActive(true);
            comboText.text = $"{count} Hits!";
            UIAnimations.PunchScale(comboText.transform);
        }

        public void TriggerPerfectDodge()
        {
            StartCoroutine(FlashPerfectDodge());
        }

        private IEnumerator FlashPerfectDodge()
        {
            perfectDodgeFlash.color = new Color(1f, 1f, 0.5f, 0.5f);
            yield return new WaitForSeconds(0.1f);
            perfectDodgeFlash.color = new Color(1f, 1f, 0.5f, 0f);
        }

        private void OnPauseClicked()
        {
            UIManager.Instance.Show(ScreenType.Pause);
        }

        private void OnSpeedToggleClicked()
        {
            _currentSpeed = _currentSpeed == 1f ? 2f : 1f;
            speedText.text = $"x{_currentSpeed}";
            Time.timeScale = _currentSpeed;
        }
    }
}
