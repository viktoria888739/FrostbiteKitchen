using UnityEngine;
using UnityEngine.UI;

namespace FrostbiteKitchen.KitchenStation
{
    public class StoveSurfaceVisual : MonoBehaviour
    {
        private const string SurfaceChildName = "StoveSurface";
        private const string IngredientChildName = "IngredientIcon";
        private const string ProgressChildName = "ProgressBar";

        [SerializeField] private Image ingredientImage;
        [SerializeField] private StoveUIBridge progressBar;

        public static StoveSurfaceVisual EnsureOnStove(Stove stove)
        {
            if (stove == null)
                return null;

            StoveSurfaceVisual existing = stove.GetComponentInChildren<StoveSurfaceVisual>(true);
            if (existing != null)
            {
                existing.Setup(stove);
                return existing;
            }

            RectTransform stoveRect = stove.GetComponent<RectTransform>();
            if (stoveRect == null)
                return null;

            GameObject surfaceObject = new GameObject(SurfaceChildName, typeof(RectTransform), typeof(StoveSurfaceVisual));
            surfaceObject.transform.SetParent(stove.transform, false);

            RectTransform surfaceRect = surfaceObject.GetComponent<RectTransform>();
            surfaceRect.anchorMin = Vector2.zero;
            surfaceRect.anchorMax = Vector2.one;
            surfaceRect.offsetMin = Vector2.zero;
            surfaceRect.offsetMax = Vector2.zero;

            StoveSurfaceVisual visual = surfaceObject.GetComponent<StoveSurfaceVisual>();
            visual.CreateIngredientIcon();
            visual.Setup(stove);
            return visual;
        }

        public void Setup(Stove stove)
        {
            if (stove == null)
                return;

            transform.SetAsLastSibling();

            if (ingredientImage == null)
                CreateIngredientIcon();

            AttachProgressBar(stove);
            WireProgressBar(stove);
        }

        public void WireProgressBridges(Stove stove)
        {
            Setup(stove);
        }

        private void CreateIngredientIcon()
        {
            Transform existing = transform.Find(IngredientChildName);
            if (existing != null)
            {
                ingredientImage = existing.GetComponent<Image>();
                return;
            }

            GameObject iconObject = new GameObject(IngredientChildName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObject.transform.SetParent(transform, false);

            RectTransform iconRect = iconObject.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = new Vector2(0f, 10f);
            iconRect.sizeDelta = new Vector2(72f, 72f);

            ingredientImage = iconObject.GetComponent<Image>();
            ingredientImage.raycastTarget = false;
            ingredientImage.preserveAspect = true;
            ingredientImage.enabled = false;
        }

        private void AttachProgressBar(Stove stove)
        {
            if (progressBar == null)
                progressBar = GetComponentInChildren<StoveUIBridge>(true);

            if (progressBar == null)
                progressBar = stove.GetComponentInChildren<StoveUIBridge>(true);

            if (progressBar == null)
                return;

            progressBar.transform.SetParent(transform, false);

            RectTransform barRect = progressBar.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0.5f, 0.5f);
            barRect.anchorMax = new Vector2(0.5f, 0.5f);
            barRect.pivot = new Vector2(0.5f, 0.5f);
            barRect.anchoredPosition = new Vector2(0f, 58f);
            barRect.sizeDelta = new Vector2(163f, 28f);
            barRect.localScale = Vector3.one;

            progressBar.BringToFront();
            progressBar.gameObject.SetActive(false);
        }

        private void WireProgressBar(Stove stove)
        {
            if (progressBar == null)
                return;

            progressBar.WireToStove(stove);
        }

        public void ShowIngredient(Sprite icon)
        {
            if (ingredientImage == null)
                CreateIngredientIcon();

            if (ingredientImage == null)
                return;

            ingredientImage.sprite = icon;
            ingredientImage.enabled = icon != null;
            ingredientImage.color = Color.white;
            ingredientImage.transform.SetAsLastSibling();

            if (progressBar != null)
                progressBar.BringToFront();
        }

        public void HideIngredient()
        {
            if (ingredientImage == null)
                return;

            ingredientImage.sprite = null;
            ingredientImage.enabled = false;
        }

        public void SetProgressBarVisible(bool visible)
        {
            if (progressBar == null)
                return;

            if (visible)
            {
                progressBar.BringToFront();
                progressBar.gameObject.SetActive(true);
            }
            else
            {
                progressBar.gameObject.SetActive(false);
            }
        }
    }
}
