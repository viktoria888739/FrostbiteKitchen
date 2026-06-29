using System.Collections.Generic;
using UnityEngine;

public class StoveCookingRunner : MonoBehaviour
{
    public static StoveCookingRunner Instance { get; private set; }

    private readonly List<Stove> cookingStoves = new List<Stove>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static StoveCookingRunner EnsureExists()
    {
        if (Instance != null)
            return Instance;

        StoveCookingRunner existing = Object.FindFirstObjectByType<StoveCookingRunner>();
        if (existing != null)
            return existing;

        GameObject runnerObject = new GameObject(nameof(StoveCookingRunner));
        return runnerObject.AddComponent<StoveCookingRunner>();
    }

    public void Register(Stove stove)
    {
        if (stove == null || cookingStoves.Contains(stove))
            return;

        cookingStoves.Add(stove);
    }

    public void Unregister(Stove stove)
    {
        if (stove == null)
            return;

        cookingStoves.Remove(stove);
    }

    private void Update()
    {
        if (!ShouldAdvanceCooking())
            return;

        float deltaTime = Time.deltaTime;

        for (int i = cookingStoves.Count - 1; i >= 0; i--)
        {
            Stove stove = cookingStoves[i];
            if (stove == null)
            {
                cookingStoves.RemoveAt(i);
                continue;
            }

            stove.TickCooking(deltaTime);
        }
    }

    private static bool ShouldAdvanceCooking()
    {
        if (GameStateMachine.Instance == null)
            return true;

        return GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Pause;
    }
}
