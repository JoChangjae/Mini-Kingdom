using UnityEngine;
using UnityEngine.UI;

namespace MiniKingdom.UI
{
    public class SettingsScreen : ScreenBase
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private Dropdown graphicsDropdown;
        [SerializeField] private Button closeBtn;
        [SerializeField] private Button supportBtn;

        protected override void OnScreenShow()
        {
            bgmSlider.value = PlayerPrefs.GetFloat("BGM_Volume", 1f);
            sfxSlider.value = PlayerPrefs.GetFloat("SFX_Volume", 1f);
            vibrationToggle.isOn = PlayerPrefs.GetInt("Vibration", 1) == 1;

            bgmSlider.onValueChanged.AddListener(OnBgmChanged);
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
            
            closeBtn.onClick.AddListener(() => UIManager.Instance.Pop());
            supportBtn.onClick.AddListener(() => Application.OpenURL("https://example.com/support"));
        }

        protected override void OnScreenHide()
        {
            bgmSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.RemoveAllListeners();
            vibrationToggle.onValueChanged.RemoveAllListeners();
            closeBtn.onClick.RemoveAllListeners();
            supportBtn.onClick.RemoveAllListeners();
            
            PlayerPrefs.Save();
        }

        protected override void OnScreenUpdate() {}

        private void OnBgmChanged(float val)
        {
            PlayerPrefs.SetFloat("BGM_Volume", val);
            // AudioManager.Instance.SetBgmVolume(val);
        }

        private void OnSfxChanged(float val)
        {
            PlayerPrefs.SetFloat("SFX_Volume", val);
            // AudioManager.Instance.SetSfxVolume(val);
        }

        private void OnVibrationChanged(bool val)
        {
            PlayerPrefs.SetInt("Vibration", val ? 1 : 0);
        }
    }
}
