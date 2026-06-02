using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.Core
{
    public class InteractionDetector : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
            if (_mainCamera == null) _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                PerformInteractionCheck();
            }
        }

        private void PerformInteractionCheck()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (EventSystem.current != null)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current) { position = mousePosition };
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                foreach (var result in results)
                {
                    IInteractable interactable = result.gameObject.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact();
                        return;
                    }
                }
            }

            Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit2D.collider != null)
            {
                IInteractable interactable = hit2D.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    return;
                }
            }

            Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit3D))
            {
                IInteractable interactable = hit3D.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}