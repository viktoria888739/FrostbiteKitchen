using UnityEngine;
using UnityEngine.UI;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.Threats
{
    public class BlindsToggle : MonoBehaviour, IInteractable
    {
        [Header("Спрайты жалюзи")]
        [SerializeField] private Image blindsImage;
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Sprite closedSprite;

        [Header("Размер UI (ширина берётся из сцены)")]
        [SerializeField] private float openHeight = 60f;
        [SerializeField] private float closedHeight = 450f;

        private RectTransform rectTransform;
        private float fixedWidth;
        private bool isClosed;
        private int lastInteractFrame = -1;

        public bool IsClosed => isClosed;

        private void Awake()
        {
            if (blindsImage == null)
                blindsImage = GetComponent<Image>();

            rectTransform = blindsImage != null ? blindsImage.rectTransform : GetComponent<RectTransform>();
            if (rectTransform != null)
                fixedWidth = rectTransform.sizeDelta.x;
        }

        private void Start()
        {
            ApplyVisual(closed: false);
        }

        public void Interact()
        {
            if (Time.frameCount == lastInteractFrame)
                return;

            lastInteractFrame = Time.frameCount;
            SetBlindsState(!isClosed);
        }

        public void SetBlindsState(bool closed)
        {
            if (isClosed == closed)
                return;

            if (closed)
            {
                isClosed = true;
                ApplyVisual(true);
                ViewRotationBlocker.PushBlock();
                GameAudioManager.Instance?.PlayBlindsClose();

                if (ThreatManager.Instance != null && ThreatManager.Instance.IsActiveThreatOn(KitchenSide.Front))
                    ThreatManager.Instance.PlayerDefendedThreat(KitchenSide.Front);

                Debug.Log("<color=red>[ЖАЛЮЗИ] Окно закрыто</color>");
                return;
            }

            isClosed = false;
            ApplyVisual(false);
            ViewRotationBlocker.PopBlock();
            GameAudioManager.Instance?.PlayBlindsOpen();
            Debug.Log("<color=green>[ЖАЛЮЗИ] Окно открыто</color>");
        }

        public void ForceOpen()
        {
            if (!isClosed)
                return;

            isClosed = false;
            ApplyVisual(false);
            ViewRotationBlocker.PopBlock();
        }

        private void ApplyVisual(bool closed)
        {
            if (blindsImage != null)
            {
                Sprite targetSprite = closed ? closedSprite : openSprite;
                if (targetSprite != null)
                    blindsImage.sprite = targetSprite;
            }

            if (rectTransform != null)
            {
                float targetHeight = closed ? closedHeight : openHeight;
                rectTransform.sizeDelta = new Vector2(fixedWidth, targetHeight);
            }
        }
    }
}
