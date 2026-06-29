using System.Collections.Generic;
using System.IO;
using FrostbiteKitchen.KitchenAnimation;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public static class KitchenAnimationBuilder
{
    private const string AnimationsRoot = "Assets/Animations/Kitchen";

    [MenuItem("Frostbite Kitchen/Build Kitchen Animators From Profiles")]
    public static void BuildAllFromProfiles()
    {
        EnsureFolder(AnimationsRoot);

        string[] profileGuids = AssetDatabase.FindAssets("t:KitchenStationAnimProfile");
        foreach (string guid in profileGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var profile = AssetDatabase.LoadAssetAtPath<KitchenStationAnimProfile>(path);
            if (profile == null)
                continue;

            BuildControllerForProfile(profile);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[KitchenAnimationBuilder] Animator controllers rebuilt.");
    }

    [MenuItem("Frostbite Kitchen/Assign Built Animators To Prefabs")]
    public static void AssignControllersToPrefabs()
    {
        AssignControllerToPrefab(
            "Assets/Prefabs/Stove_Prefab.prefab",
            $"{AnimationsRoot}/StoveKitchen.controller");

        AssignControllerToPrefab(
            "Assets/Prefabs/CuttingBoard_Prefab.prefab",
            $"{AnimationsRoot}/CuttingBoardKitchen.controller");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[KitchenAnimationBuilder] Prefab animators assigned.");
    }

    public static void BuildControllerForProfile(KitchenStationAnimProfile profile)
    {
        if (profile == null)
            return;

        string controllerName = profile.StationType == KitchenStationType.Stove
            ? "StoveKitchen"
            : "CuttingBoardKitchen";

        string controllerPath = $"{AnimationsRoot}/{controllerName}.controller";
        EnsureFolder(AnimationsRoot);

        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        ConfigureParameters(controller);
        BuildStates(controller, profile, controllerName);
        EditorUtility.SetDirty(controller);
    }

    private static void ConfigureParameters(AnimatorController controller)
    {
        controller.AddParameter(KitchenAnimParams.PrepStage, AnimatorControllerParameterType.Int);
        controller.AddParameter(KitchenAnimParams.IsUnderThreat, AnimatorControllerParameterType.Bool);
    }

    private static void BuildStates(AnimatorController controller, KitchenStationAnimProfile profile, string clipPrefix)
    {
        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine root = layer.stateMachine;

        var states = new Dictionary<KitchenPrepStage, AnimatorState>
        {
            { KitchenPrepStage.Idle, CreateState(root, "Idle", CreateStaticClip(profile, KitchenPrepStage.Idle, $"{clipPrefix}_Idle")) },
            { KitchenPrepStage.Cutting, CreateState(root, "Cutting", CreateLoopClip(profile, KitchenPrepStage.Cutting, $"{clipPrefix}_Cutting")) },
            { KitchenPrepStage.Frying, CreateState(root, "Frying", CreateLoopClip(profile, KitchenPrepStage.Frying, $"{clipPrefix}_Frying")) },
            { KitchenPrepStage.Done, CreateState(root, "Done", CreateStaticClip(profile, KitchenPrepStage.Done, $"{clipPrefix}_Done")) },
            { KitchenPrepStage.Burned, CreateState(root, "Burned", CreateStaticClip(profile, KitchenPrepStage.Burned, $"{clipPrefix}_Burned")) }
        };

        root.defaultState = states[KitchenPrepStage.Idle];

        foreach (KeyValuePair<KitchenPrepStage, AnimatorState> entry in states)
        {
            if (entry.Key == KitchenPrepStage.Idle)
                continue;

            AnimatorStateTransition transition = root.AddAnyStateTransition(entry.Value);
            transition.AddCondition(AnimatorConditionMode.Equals, (int)entry.Key, KitchenAnimParams.PrepStage);
            transition.duration = 0f;
            transition.hasExitTime = false;
            transition.canTransitionToSelf = true;
        }

        AnimatorStateTransition idleTransition = root.AddAnyStateTransition(states[KitchenPrepStage.Idle]);
        idleTransition.AddCondition(AnimatorConditionMode.Equals, (int)KitchenPrepStage.Idle, KitchenAnimParams.PrepStage);
        idleTransition.duration = 0f;
        idleTransition.hasExitTime = false;
        idleTransition.canTransitionToSelf = true;
    }

    private static AnimatorState CreateState(AnimatorStateMachine root, string name, AnimationClip clip)
    {
        var state = root.AddState(name);
        state.motion = clip;
        return state;
    }

    private static AnimationClip CreateStaticClip(KitchenStationAnimProfile profile, KitchenPrepStage stage, string clipName)
    {
        Sprite sprite = profile.GetStaticSprite(stage);
        return CreateSpriteClip(clipName, sprite != null ? new[] { sprite } : null, false, profile.FramesPerSecond);
    }

    private static AnimationClip CreateLoopClip(KitchenStationAnimProfile profile, KitchenPrepStage stage, string clipName)
    {
        Sprite[] frames = profile.GetFrames(stage);
        if (frames == null || frames.Length <= 1)
            return CreateStaticClip(profile, stage, clipName);

        return CreateSpriteClip(clipName, frames, true, profile.FramesPerSecond);
    }

    private static AnimationClip CreateSpriteClip(string clipName, Sprite[] sprites, bool loop, float fps)
    {
        string clipPath = $"{AnimationsRoot}/{clipName}.anim";
        EnsureFolder(AnimationsRoot);

        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (clip == null)
        {
            clip = new AnimationClip { name = clipName };
            AssetDatabase.CreateAsset(clip, clipPath);
        }

        clip.frameRate = Mathf.Max(1f, fps);

        if (sprites == null || sprites.Length == 0 || sprites[0] == null)
        {
            AnimationUtility.SetObjectReferenceCurve(clip, ImageSpriteBinding(), null);
            EditorUtility.SetDirty(clip);
            return clip;
        }

        var keyframes = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i / clip.frameRate,
                value = sprites[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, ImageSpriteBinding(), keyframes);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        EditorUtility.SetDirty(clip);
        return clip;
    }

    private static EditorCurveBinding ImageSpriteBinding()
    {
        return EditorCurveBinding.PPtrCurve(string.Empty, typeof(Image), "m_Sprite");
    }

    private static void AssignControllerToPrefab(string prefabPath, string controllerPath)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
        if (prefab == null || controller == null)
            return;

        GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);
        Animator animator = instance.GetComponent<Animator>();
        if (animator == null)
            animator = instance.AddComponent<Animator>();

        animator.runtimeAnimatorController = controller;

        KitchenStationAnimator stationAnimator = instance.GetComponent<KitchenStationAnimator>();
        if (stationAnimator != null)
        {
            SerializedObject serialized = new SerializedObject(stationAnimator);
            serialized.FindProperty("animator").objectReferenceValue = animator;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        PrefabUtility.UnloadPrefabContents(instance);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
        string folderName = Path.GetFileName(path);
        if (string.IsNullOrEmpty(parent))
            return;

        if (!AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        AssetDatabase.CreateFolder(parent, folderName);
    }
}
