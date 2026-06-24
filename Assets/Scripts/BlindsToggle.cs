using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.Threats
{
    public class BlindsToggle : MonoBehaviour, IPointerClickHandler, IInteractable
    {
        private RectTransform rectTransform;
        private bool isClosed = false;

        [Header("Высота жалюзи")]
        [SerializeField] private float openHeight = 60f;    
        [SerializeField] private float closedHeight = 450f; 

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            SetBlindsHeight(openHeight);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            isClosed = !isClosed; 

            if (isClosed)
            {
                SetBlindsHeight(closedHeight);
                Debug.Log("<color=red>[ЖАЛЮЗИ] Окно ЗАКРЫТО. Защита от монстра активна!</color>");

                if (ThreatManager.Instance != null)
                    ThreatManager.Instance.PlayerDefendedThreat(null);
            }
            else
            {
                SetBlindsHeight(openHeight);
                Debug.Log("<color=green>[ЖАЛЮЗИ] Окно ОТКРЫТО.</color>");
            }
        }

        private void SetBlindsHeight(float height)
        {
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
            }
        }
    }
}