using UnityEngine;

namespace MiniKingdom.Combat
{
    /// <summary>
    /// Smooth 2D camera shake manager for visceral combat feedback.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        private Vector3 _originalPos;
        private float _shakeTimer;
        private float _shakeMagnitude;

        private void Awake()
        {
            Instance = this;
            _originalPos = transform.localPosition;
        }

        private void LateUpdate()
        {
            if (_shakeTimer > 0)
            {
                transform.localPosition = _originalPos + (Vector3)(Random.insideUnitCircle * _shakeMagnitude);
                _shakeTimer -= Time.unscaledDeltaTime;
                if (_shakeTimer <= 0)
                {
                    transform.localPosition = _originalPos;
                }
            }
        }

        public void DoShake(float duration = 0.2f, float magnitude = 0.15f)
        {
            _shakeTimer = duration;
            _shakeMagnitude = magnitude;
        }

        public static void Shake(float duration = 0.2f, float magnitude = 0.15f)
        {
            if (Instance != null)
            {
                Instance.DoShake(duration, magnitude);
            }
            else if (Camera.main != null)
            {
                var comp = Camera.main.GetComponent<CameraShake>();
                if (comp == null) comp = Camera.main.gameObject.AddComponent<CameraShake>();
                comp.DoShake(duration, magnitude);
            }
        }
    }
}
