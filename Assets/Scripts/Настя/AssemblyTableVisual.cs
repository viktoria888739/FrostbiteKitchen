using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

namespace FrostbiteKitchen.KitchenStation
{
    public class AssemblyTableVisual : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image tableSlotImage;
        [SerializeField] private Sprite spoiledDishSprite;
        [SerializeField] private RecipeCatalog recipeCatalog;

        private List<RecipeData> runtimeRecipes = new List<RecipeData>();

        private void Awake()
        {
            if (tableSlotImage == null)
                tableSlotImage = GetComponent<Image>();

            ImageRaycastHelper.EnsureRaycastTarget(gameObject);

            ResolveRecipeCatalog();
            BuildRuntimeRecipes();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            AssemblyTable.Instance?.Interact();
        }

        private void Start()
        {
            RefreshFromPlate();
        }

        public void RefreshFromPlate()
        {
            if (tableSlotImage == null)
                return;

            IReadOnlyList<IngredientData> plateIngredients = DishAssembler.Instance != null
                ? DishAssembler.Instance.GetCurrentIngredients()
                : null;

            if (plateIngredients == null || plateIngredients.Count == 0)
            {
                ShowEmptyPlate();
                return;
            }

            if (runtimeRecipes.Count == 0)
                BuildRuntimeRecipes();

            Sprite targetSprite = AssemblyPlateVisualResolver.ResolvePlateSprite(
                plateIngredients,
                runtimeRecipes,
                spoiledDishSprite);

            if (targetSprite == null)
            {
                ShowEmptyPlate();
                return;
            }

            tableSlotImage.gameObject.SetActive(true);
            tableSlotImage.raycastTarget = true;
            tableSlotImage.sprite = targetSprite;
            tableSlotImage.color = Color.white;
            tableSlotImage.preserveAspect = true;
        }

        private void ShowEmptyPlate()
        {
            if (tableSlotImage == null)
                return;

            tableSlotImage.sprite = null;
            tableSlotImage.color = new Color(1f, 1f, 1f, 0f);
            tableSlotImage.raycastTarget = false;
        }

        private void ResolveRecipeCatalog()
        {
            if (recipeCatalog != null)
                return;

            recipeCatalog = Resources.Load<RecipeCatalog>("MainRecipeCatalog");
        }

        private void BuildRuntimeRecipes()
        {
            runtimeRecipes.Clear();

            if (recipeCatalog == null || recipeCatalog.AllRecipes == null)
                return;

            foreach (RecipeData recipe in recipeCatalog.AllRecipes)
            {
                if (recipe != null && !runtimeRecipes.Contains(recipe))
                    runtimeRecipes.Add(recipe);
            }
        }
    }
}
