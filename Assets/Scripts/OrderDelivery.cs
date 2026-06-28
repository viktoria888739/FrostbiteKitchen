using UnityEngine;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.UI
{
    public static class OrderDelivery
    {
        public static void TrySubmit()
        {
            if (OrderManager.Instance == null || PlayerInventory.Instance == null)
                return;

            RecipeData activeRecipe = OrderManager.Instance.GetActiveRecipe();
            if (activeRecipe == null)
                return;

            DishData heldDish = PlayerInventory.Instance.CurrentHeldDish;
            if (heldDish == null)
                return;

            if (DishAssembler.ValidateDish(heldDish, activeRecipe))
            {
                GameAudioManager.Instance?.PlayPlateServe();
                PlayerInventory.Instance.ClearSelectedSlot();
                OrderManager.Instance.CompleteActiveOrder();
                return;
            }

            GameAudioManager.Instance?.PlayDishError();
            PlayerInventory.Instance.ClearSelectedSlot();
            OrderManager.Instance.FailWrongOrder();
        }
    }
}
