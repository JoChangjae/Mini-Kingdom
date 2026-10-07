using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniKingdom.UI
{
    public class PopupRequest
    {
        public enum Type { Confirmation, Info, Reward, Toast }
        public Type PopupType;
        public string Title;
        public string Message;
        public Action OnConfirm;
        public Action OnCancel;
        public List<RewardData> Rewards;
    }

    public class RewardData
    {
        public Sprite Icon;
        public int Amount;
    }

    /// <summary>
    /// Manages various types of popups (confirmation, info, rewards).
    /// </summary>
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance { get; private set; }

        [Header("Popup References")]
        [SerializeField] private GameObject confirmationPopupPanel;
        [SerializeField] private TextMeshProUGUI confirmTitleText;
        [SerializeField] private TextMeshProUGUI confirmMessageText;
        [SerializeField] private Button confirmYesButton;
        [SerializeField] private Button confirmNoButton;

        [Header("Toast")]
        [SerializeField] private GameObject toastPanel;
        [SerializeField] private TextMeshProUGUI toastMessageText;

        private Queue<PopupRequest> _popupQueue = new Queue<PopupRequest>();
        private bool _isPopupActive = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            confirmationPopupPanel.SetActive(false);
            toastPanel.SetActive(false);
        }

        public void ShowConfirmation(string title, string message, Action onConfirm, Action onCancel = null)
        {
            EnqueuePopup(new PopupRequest
            {
                PopupType = PopupRequest.Type.Confirmation,
                Title = title,
                Message = message,
                OnConfirm = onConfirm,
                OnCancel = onCancel
            });
        }

        public void ShowInfo(string title, string message, Action onClose = null)
        {
            EnqueuePopup(new PopupRequest
            {
                PopupType = PopupRequest.Type.Info,
                Title = title,
                Message = message,
                OnConfirm = onClose
            });
        }

        public void ShowReward(string title, List<RewardData> rewards, Action onClose = null)
        {
            EnqueuePopup(new PopupRequest
            {
                PopupType = PopupRequest.Type.Reward,
                Title = title,
                Rewards = rewards,
                OnConfirm = onClose
            });
        }

        public void ShowToast(string message)
        {
            // Toasts can bypass the queue for immediate feedback
            toastMessageText.text = message;
            toastPanel.SetActive(true);
            UIAnimations.FadeIn(toastPanel.GetComponent<CanvasGroup>(), 0.2f);
            Invoke(nameof(HideToast), 2f);
        }

        private void HideToast()
        {
            UIAnimations.FadeOut(toastPanel.GetComponent<CanvasGroup>(), 0.5f, () => toastPanel.SetActive(false));
        }

        private void EnqueuePopup(PopupRequest request)
        {
            _popupQueue.Enqueue(request);
            if (!_isPopupActive)
            {
                ProcessNextPopup();
            }
        }

        private void ProcessNextPopup()
        {
            if (_popupQueue.Count == 0)
            {
                _isPopupActive = false;
                return;
            }

            _isPopupActive = true;
            var request = _popupQueue.Dequeue();

            switch (request.PopupType)
            {
                case PopupRequest.Type.Confirmation:
                case PopupRequest.Type.Info:
                    ShowConfirmationInternal(request);
                    break;
                case PopupRequest.Type.Reward:
                    // Show reward logic here
                    ProcessNextPopup(); // placeholder
                    break;
            }
        }

        private void ShowConfirmationInternal(PopupRequest request)
        {
            confirmTitleText.text = request.Title;
            confirmMessageText.text = request.Message;
            
            confirmYesButton.onClick.RemoveAllListeners();
            confirmYesButton.onClick.AddListener(() => {
                request.OnConfirm?.Invoke();
                CloseConfirmationPopup();
            });

            confirmNoButton.onClick.RemoveAllListeners();
            confirmNoButton.onClick.AddListener(() => {
                request.OnCancel?.Invoke();
                CloseConfirmationPopup();
            });

            // Hide No button if it's just an info popup
            confirmNoButton.gameObject.SetActive(request.PopupType == PopupRequest.Type.Confirmation);

            confirmationPopupPanel.SetActive(true);
            UIAnimations.ScaleBounce(confirmationPopupPanel.transform);
        }

        private void CloseConfirmationPopup()
        {
            confirmationPopupPanel.SetActive(false);
            ProcessNextPopup();
        }
    }
}
