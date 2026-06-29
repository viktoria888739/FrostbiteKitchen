using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class TutorialSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/Tutorial.unity";

    [MenuItem("Frostbite Kitchen/Build Tutorial Scene")]
    public static void BuildTutorialScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateMainCamera();
        CreateEventSystem();
        CreateTutorialController();

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log($"Tutorial scene saved to {ScenePath}");
    }

    private static void CreateMainCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.19215687f, 0.3019608f, 0.4745098f, 0f);
        cameraObject.AddComponent<AudioListener>();
    }

    private static void CreateEventSystem()
    {
        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private static void CreateTutorialController()
    {
        GameObject tutorialObject = new GameObject("_Tutorial");
        TutorialSceneController controller = tutorialObject.AddComponent<TutorialSceneController>();

        SerializedObject serializedController = new SerializedObject(controller);
        serializedController.FindProperty("frontBackground").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Backgrounds/GameBg/front-bg.png");
        serializedController.FindProperty("dialogueBoxSprite").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/ui-dialogue-window.png");
        serializedController.FindProperty("customerPortrait").objectReferenceValue =
            Resources.Load<Sprite>("Customers/person 1");
        serializedController.FindProperty("dialogueFont").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/TextMesh Pro/Resources/Fonts & Materials/stolzl_book SDF.asset");
        serializedController.FindProperty("gameplaySceneIndex").intValue = TutorialSceneController.GameplaySceneIndex;
        serializedController.ApplyModifiedPropertiesWithoutUndo();
    }
}
