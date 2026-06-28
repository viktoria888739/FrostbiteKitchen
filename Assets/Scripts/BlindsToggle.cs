using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.Threats
{
    public class BlindsToggle : MonoBehaviour, IPointerClickHandler, IInteractable
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

        public bool IsClosed => isClosed;

        private void Awake()
        {
            if (blindsImage == null)
            {
                blindsImage = GetComponent<Image>();
            }

            rectTransform = blindsImage != null ? blindsImage.rectTransform : GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                fixedWidth = rectTransform.sizeDelta.x;
            }
        }

        private void Start()
        {
            ApplyVisual(closed: false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            SetBlindsState(!isClosed);
        }

        public void SetBlindsState(bool closed)
        {
            if (isClosed == closed)
            {
                return;
            }

            isClosed = closed;
            ApplyVisual(isClosed);

            if (isClosed)
            {
                Debug.Log("<color=red>[ЖАЛЮЗИ] Окно ЗАКРЫТО — вращение заблокировано</color>");
                ViewRotationBlocker.SetBlock(true);
                GameAudioManager.Instance?.PlayBlindsClose();
                ThreatManager.Instance?.PlayerDefendedThreat(KitchenSide.Front);
            }
            else
            {
                Debug.Log("<color=green>[ЖАЛЮЗИ] Окно ОТКРЫТО — вращение разрешено</color>");
                ViewRotationBlocker.SetBlock(false);
                GameAudioManager.Instance?.PlayBlindsOpen();
            }
        }

        private void ApplyVisual(bool closed)
        {
            isClosed = closed;

            if (blindsImage != null)
            {
                Sprite targetSprite = isClosed ? closedSprite : openSprite;
                if (targetSprite != null)
                {
                    blindsImage.sprite = targetSprite;
                }
            }

            if (rectTransform != null)
            {
                float targetHeight = isClosed ? closedHeight : openHeight;
                rectTransform.sizeDelta = new Vector2(fixedWidth, targetHeight);
            }
        }
    }
}
