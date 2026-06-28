using UnityEngine;

namespace FrostbiteKitchen.KitchenAnimation
{
    [CreateAssetMenu(fileName = "KitchenStationAnimProfile", menuName = "Frostbite Kitchen/Animation/Station Profile")]
    public class KitchenStationAnimProfile : ScriptableObject
    {
        [SerializeField] private KitchenStationType stationType = KitchenStationType.Stove;
        [SerializeField] private float framesPerSecond = 8f;
        [SerializeField] private Sprite idle;
        [SerializeField] private Sprite[] cuttingFrames;
        [SerializeField] private Sprite[] fryingFrames;
        [SerializeField] private Sprite done;
        [SerializeField] private Sprite burned;

        public KitchenStationType StationType => stationType;
        public float FramesPerSecond => framesPerSecond;
        public Sprite Idle => idle;
        public Sprite Done => done;
        public Sprite Burned => burned;

        public Sprite[] GetFrames(KitchenPrepStage stage)
        {
            switch (stage)
            {
                case KitchenPrepStage.Cutting:
                    return cuttingFrames;
                case KitchenPrepStage.Frying:
                    return fryingFrames;
                default:
                    return null;
            }
        }

        public Sprite GetStaticSprite(KitchenPrepStage stage)
        {
            switch (stage)
            {
                case KitchenPrepStage.Idle:
                    return idle;
                case KitchenPrepStage.Cutting:
                    return GetFirstFrame(cuttingFrames) ?? idle;
                case KitchenPrepStage.Frying:
                    return GetFirstFrame(fryingFrames) ?? idle;
                case KitchenPrepStage.Done:
                    return done != null ? done : idle;
                case KitchenPrepStage.Burned:
                    return burned != null ? burned : done;
                default:
                    return idle;
            }
        }

        private static Sprite GetFirstFrame(Sprite[] frames)
        {
            if (frames == null || frames.Length == 0)
                return null;

            return frames[0];
        }
    }
}
