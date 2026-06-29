using UnityEngine;

public static class ViewRotationBlocker
{
    private static int blockCount;

    public static bool IsRotationBlocked => blockCount > 0;

    public static void PushBlock()
    {
        blockCount++;
    }

    public static void PopBlock()
    {
        if (blockCount > 0)
            blockCount--;
    }

    public static void Reset()
    {
        blockCount = 0;
    }
}
