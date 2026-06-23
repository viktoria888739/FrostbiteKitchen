using UnityEngine;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

public class ChoppingBoard : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        IngredientData held = PlayerInventory.Instance.CurrentHeldItem;
        
        if (held != null)
        {
            // Запуск анимации нарезки
            if (animator != null)
                animator.SetTrigger("Chop");

            Debug.Log($"<color=cyan>[ДОСКА] Нарезаем: {held.displayName}</color>");

            // Здесь можно добавить задержку и замену на нарезанную версию
            // StartCoroutine(ChopItem(held));
        }
    }

    // Пример корутины
    // private IEnumerator ChopItem(IngredientData item) { ... }
}