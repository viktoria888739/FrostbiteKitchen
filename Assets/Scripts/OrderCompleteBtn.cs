using UnityEngine;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.KitchenStation; // Добавлено для доступа к AssemblyTable

namespace FrostbiteKitchen.UI
{
    public class OrderCompleteBtn : MonoBehaviour, IInteractable
    {
        [Header("Зона выдачи")]
        [Tooltip("Ссылка на сборщик тарелок")]
        [SerializeField] private DishAssembler dishAssembler;

        private void Awake()
        {
            if (dishAssembler == null)
            {
                dishAssembler = DishAssembler.Instance;
            }
        }

        public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            if (dishAssembler == null || OrderManager.Instance == null)
            {
                Debug.LogError("[ЗОНА ВЫДАЧИ] Не найдены необходимые компоненты!");
                return;
            }

            RecipeData activeRecipe = OrderManager.Instance.GetActiveRecipe();
            if (activeRecipe == null)
            {
                Debug.LogWarning("<color=yellow>[ЗОНА ВЫДАЧИ]</color> Нет активного заказа.");
                return;
            }

            if (dishAssembler.ValidateRecipe(activeRecipe))
            {
                Debug.Log($"<color=green>[ЗОНА ВЫДАЧИ]</color> Заказ '{activeRecipe.recipeName}' успешно сдан!");

                OrderManager.Instance.CompleteActiveOrder();
                dishAssembler.ClearPlate();

                // Добавлено: Очищаем визуал стола сборки при успешной сдаче
                if (AssemblyTable.Instance != null)
                {
                    AssemblyTable.Instance.ResetTable();
                }
            }
            else
            {
                Debug.Log("<color=red>[ЗОНА ВЫДАЧИ]</color> Блюдо не соответствует заказу! Заказ провален.");

                OrderManager.Instance.FailCurrentOrder();
                dishAssembler.ClearPlate();

                // Добавлено: Очищаем визуал стола сборки, так как тарелка была опустошена
                if (AssemblyTable.Instance != null)
                {
                    AssemblyTable.Instance.ResetTable();
                }
            }
        }
    }
}