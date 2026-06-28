using UnityEngine;
using UnityEngine.UI;

namespace FrostbiteKitchen.KitchenStation
{
    internal static class ImageRaycastHelper
    {
        public static void EnsureRaycastTarget(GameObject target)
        {
            if (target == null)
                return;

            Image image = target.GetComponent<Image>();
            if (image != null)
                image.raycastTarget = true;
        }
    }
}
