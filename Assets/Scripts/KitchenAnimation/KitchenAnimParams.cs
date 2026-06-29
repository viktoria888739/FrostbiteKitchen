using UnityEngine;

namespace FrostbiteKitchen.KitchenAnimation
{
    public static class KitchenAnimParams
    {
        public const string PrepStage = "PrepStage";
        public const string IsUnderThreat = "IsUnderThreat";

        public static readonly int PrepStageHash = Animator.StringToHash(PrepStage);
        public static readonly int IsUnderThreatHash = Animator.StringToHash(IsUnderThreat);
    }
}
