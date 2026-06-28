using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FrostbiteKitchen.KitchenAnimation
{
    [DisallowMultipleComponent]
    public class KitchenStationAnimator : MonoBehaviour
    {
        [SerializeField] private KitchenStationAnimProfile profile;
        [SerializeField] private Animator animator;
        [SerializeField] private Image targetImage;

        private Coroutine frameRoutine;
        private KitchenPrepStage currentStage = KitchenPrepStage.Idle;

        public KitchenPrepStage CurrentStage => currentStage;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            if (targetImage == null)
                targetImage = GetComponent<Image>();

            if (targetImage == null)
                targetImage = GetComponentInChildren<Image>(true);
        }

        private void Start()
        {
            ApplyStage(currentStage, true);
        }

        public void SetStage(KitchenPrepStage stage)
        {
            if (currentStage == stage)
                return;

            ApplyStage(stage, false);
        }

        public void ResetToIdle()
        {
            ApplyStage(KitchenPrepStage.Idle, false);
        }

        public void SetUnderThreat(bool underThreat)
        {
            if (animator != null && animator.runtimeAnimatorController != null)
                animator.SetBool(KitchenAnimParams.IsUnderThreatHash, underThreat);
        }

        private void ApplyStage(KitchenPrepStage stage, bool force)
        {
            if (!force && currentStage == stage)
                return;

            currentStage = stage;

            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetInteger(KitchenAnimParams.PrepStageHash, (int)stage);
                return;
            }

            PlayFallback(stage);
        }

        private void PlayFallback(KitchenPrepStage stage)
        {
            if (targetImage == null || profile == null)
                return;

            if (frameRoutine != null)
            {
                StopCoroutine(frameRoutine);
                frameRoutine = null;
            }

            Sprite[] frames = profile.GetFrames(stage);
            if (frames != null && frames.Length > 1)
            {
                frameRoutine = StartCoroutine(PlayFrames(frames));
                return;
            }

            targetImage.sprite = profile.GetStaticSprite(stage);
        }

        private IEnumerator PlayFrames(Sprite[] frames)
        {
            float frameDuration = 1f / Mathf.Max(1f, profile.FramesPerSecond);

            while (true)
            {
                for (int i = 0; i < frames.Length; i++)
                {
                    if (frames[i] != null)
                        targetImage.sprite = frames[i];

                    yield return new WaitForSeconds(frameDuration);
                }
            }
        }
    }
}
