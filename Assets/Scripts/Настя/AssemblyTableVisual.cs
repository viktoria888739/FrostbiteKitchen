using UnityEngine;
using UnityEngine.UI;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

namespace FrostbiteKitchen.KitchenStation
{
    public class AssemblyTableVisual : MonoBehaviour
    {
        [Header("Визуальные элементы стола")]
        [SerializeField] private Image tableSlotImage;
        [SerializeField] private Sprite emptyPlateSprite;
        [SerializeField] private Sprite spoiledDishSprite;

        [Header("Каталог рецептов для проверки")]
        [Tooltip("Перетащи сюда твой ScriptableObject RecipeCatalog, чтобы стол знал все рецепты игры")]
        [SerializeField] private RecipeCatalog recipeCatalog;

        private void Start()
        {
            UpdateVisual(0, null);
        }

        public void UpdateVisual(int ingredientsCount, Sprite firstIngredientIcon)
        {
            if (tableSlotImage == null) return;

            if (ingredientsCount == 0)
            {
                if (emptyPlateSprite != null)
                {
                    tableSlotImage.gameObject.SetActive(true);
                    tableSlotImage.sprite = emptyPlateSprite;
                }
                else
                {
                    tableSlotImage.gameObject.SetActive(false);
                }
            }
            else if (ingredientsCount == 1)
            {
                tableSlotImage.gameObject.SetActive(true);
                tableSlotImage.sprite = firstIngredientIcon;
            }
            else
            {
                tableSlotImage.gameObject.SetActive(true);
                Sprite matchingRecipeSprite = FindMatchingRecipeSprite();

                if (matchingRecipeSprite != null)
                {
                    tableSlotImage.sprite = matchingRecipeSprite;
                }
                else
                {
                    if (spoiledDishSprite != null)
                        tableSlotImage.sprite = spoiledDishSprite;
                    else
                        tableSlotImage.gameObject.SetActive(false);
                }
            }
        }

        private Sprite FindMatchingRecipeSprite()
        {
            if (DishAssembler.Instance == null) return null;

            // Проверяем все рецепты из каталога, который мы привязали к столу
            if (recipeCatalog != null && recipeCatalog.AllRecipes != null)
            {
                foreach (RecipeData recipe in recipeCatalog.AllRecipes)
                {
                    if (DishAssembler.Instance.ValidateRecipe(recipe))
                    {
                        return recipe.icon; // Если это бутерброд с джемом, покажется бутерброд
                    }
                }
            }
            // Резервный вариант: если забыли привязать каталог, проверяем хотя бы активный заказ
            else if (OrderManager.Instance != null)
            {
                RecipeData activeRecipe = OrderManager.Instance.GetActiveRecipe();
                if (activeRecipe != null && DishAssembler.Instance.ValidateRecipe(activeRecipe))
                {
                    return activeRecipe.icon;
                }
            }

            return null;
        }
    }
}