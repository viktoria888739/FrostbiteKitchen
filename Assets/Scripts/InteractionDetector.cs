using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.KitchenStation;

namespace FrostbiteKitchen.Core
{
    public class InteractionDetector : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                PerformInteractionCheck();
        }

        private void PerformInteractionCheck()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (EventSystem.current != null)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current) { position = mousePosition };
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                IInteractable interactable = ChooseUiInteractable(results);
                if (interactable != null)
                {
                    interactable.Interact();
                    return;
                }
            }

            Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit2D.collider != null)
            {
                IInteractable interactable = hit2D.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    return;
                }
            }

            Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit3D))
            {
                IInteractable interactable = hit3D.collider.GetComponentInParent<IInteractable>();
                interactable?.Interact();
            }
        }

        private static IInteractable ChooseUiInteractable(List<RaycastResult> results)
        {
            Stove stoveInResults = FindStoveInResults(results);

            if (stoveInResults != null)
            {
                if (stoveInResults.CanPickupCookedIngredient() || stoveInResults.HasIngredientOnStove)
                    return stoveInResults;
            }

            IInteractable topInteractable = null;

            foreach (RaycastResult result in results)
            {
                IInteractable onHit = result.gameObject.GetComponent<IInteractable>();
                if (onHit == null)
                    onHit = result.gameObject.GetComponentInParent<IInteractable>();

                if (onHit != null)
                {
                    topInteractable = onHit;
                    break;
                }
            }

            if (CuttingBoard.CanAcceptHeldIngredient() && CuttingBoard.Instance != null)
            {
                if (topInteractable is CuttingBoard || topInteractable is AssemblyTable)
                    return CuttingBoard.Instance;
            }

            if (Stove.IsHoldingCookableSingleItem())
            {
                if (stoveInResults != null)
                    return stoveInResults;

                return topInteractable;
            }

            if (AssemblyTable.CanAcceptHeldIngredient() && AssemblyTable.Instance != null)
            {
                if (topInteractable is AssemblyTable)
                    return AssemblyTable.Instance;
            }

            return topInteractable;
        }

        private static Stove FindStoveInResults(List<RaycastResult> results)
        {
            foreach (RaycastResult result in results)
            {
                Stove stove = result.gameObject.GetComponent<Stove>();
                if (stove == null)
                    stove = result.gameObject.GetComponentInParent<Stove>();

                if (stove != null)
                    return stove;
            }

            return null;
        }
    }
}
