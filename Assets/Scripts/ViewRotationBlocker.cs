using UnityEngine;

public static class ViewRotationBlocker
{
    public static bool IsRotationBlocked { get; private set; }

    public static void SetBlock(bool block)
    {
        IsRotationBlocked = block;
    }
}