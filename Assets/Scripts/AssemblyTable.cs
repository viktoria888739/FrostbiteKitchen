using UnityEngine;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

namespace FrostbiteKitchen.KitchenStation
{
    public class AssemblyTable : MonoBehaviour, IInteractable
    {
        // Добавлено: Синглтон для вызова очистки из зоны выдачи
        public static AssemblyTable Instance { get; private set; }

        [Header("Сборочный стол")]
        [Tooltip("Максимальное количество ингредиентов на тарелке")]
        [SerializeField] private int maxIngredients = 6;

        [Header("Визуализация сборки")]
        [Tooltip("Ссылка на скрипт управления визуалом иконок на столе")]
        [SerializeField] private AssemblyTableVisual tableVisual;

        // Внутренние переменные для отслеживания состояния иконок
        private Sprite firstIngredientSprite = null;
        private int currentIngredientsCount = 0;

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
        }

        public void Interact()
        {
            var inventory = PlayerInventory.Instance;
            var assembler = DishAssembler.Instance;

            if (inventory == null || assembler == null)
            {
                Debug.LogWarning("[СБОРКА] Не найдены Inventory или DishAssembler!");
                return;
            }

            if (inventory.CurrentHeldItem == null)
            {
                Debug.Log("<color=#33FF57>[СБОРКА]</color> Руки пустые.");
                return;
            }

            if (assembler.GetCurrentIngredientCount() >= maxIngredients)
            {
                Debug.Log("<color=yellow>[СБОРКА]</color> Тарелка уже полная!");
                return;
            }

            IngredientData heldItem = inventory.CurrentHeldItem;

            // Добавлено: Запоминаем иконку самого первого ингредиента
            if (currentIngredientsCount == 0)
            {
                firstIngredientSprite = heldItem.icon;
            }

            assembler.AddIngredient(heldItem);
            inventory.TryUseOneItem();
            Debug.Log($"<color=#33FF57>[СБОРКА]</color> Добавлен ингредиент: {heldItem.displayName}");

            // Добавлено: Увеличиваем счетчик и обновляем картинку
            currentIngredientsCount++;
            if (tableVisual != null)
            {
                tableVisual.UpdateVisual(currentIngredientsCount, firstIngredientSprite);
            }
        }

        // Добавлено: Метод сброса стола при успешной или провальной сдаче заказа
        public void ResetTable()
        {
            currentIngredientsCount = 0;
            firstIngredientSprite = null;
            if (tableVisual != null)
            {
                tableVisual.UpdateVisual(0, null);
            }
        }
    }
}