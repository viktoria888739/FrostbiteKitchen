using UnityEngine;

public class ThreatSpawner2 : MonoBehaviour
{
    [Header("Информация стороны")]
    public string sideName = "Сторона";

    [Header("Визуалы")]
    [SerializeField] private GameObject monsterVisual;
    [SerializeField] private GameObject warningEffect;

    public void ActivateThreat()
    {
        if (monsterVisual != null) monsterVisual.SetActive(true);
        if (warningEffect != null) warningEffect.SetActive(true);
    }

    public void DeactivateThreat()
    {
        if (monsterVisual != null) monsterVisual.SetActive(false);
        if (warningEffect != null) warningEffect.SetActive(false);
    }
}