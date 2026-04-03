using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class BuildSettingsSetup
{
    static BuildSettingsSetup()
    {
        var scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true),
        };

        // Only set if not already configured
        if (EditorBuildSettings.scenes.Length < 2)
        {
            EditorBuildSettings.scenes = scenes;
            Debug.Log("BomberAce: Build settings updated with MainMenu and SampleScene.");
        }
    }
}
