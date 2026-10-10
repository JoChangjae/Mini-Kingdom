using UnityEngine;
using TMPro;

namespace MiniKingdom.UI
{
    /// <summary>
    /// Floating combat text for damage numbers, critical hits, and status notifications.
    /// </summary>
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private float floatSpeed = 40f;
        [SerializeField] private float lifeTime = 0.85f;

        private float _timer;
        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            if (textMesh == null) textMesh = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Setup(string text, Color color, Vector3 worldPos)
        {
            if (textMesh == null) textMesh = GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = text;
                textMesh.color = color;
                textMesh.raycastTarget = false;
            }

            if (_rect == null) _rect = GetComponent<RectTransform>();

            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Vector3 screenPos = Camera.main != null ? Camera.main.WorldToScreenPoint(worldPos) : worldPos;
                _rect.position = screenPos + new Vector3(Random.Range(-20f, 20f), Random.Range(10f, 30f), 0);
            }
            else
            {
                transform.position = worldPos + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0.2f, 0.5f), 0);
            }

            _timer = lifeTime;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            transform.position += Vector3.up * (floatSpeed * Time.deltaTime);
            _timer -= Time.deltaTime;

            if (textMesh != null)
            {
                Color c = textMesh.color;
                c.a = Mathf.Clamp01(_timer / lifeTime);
                textMesh.color = c;
            }

            if (_timer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
