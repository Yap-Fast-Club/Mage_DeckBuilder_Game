#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
 

public class UILoader : MonoBehaviour
{
    private string _loadedUiSceneName;

    void Start()
    {
        LoadUI();
    }


    //[Button(ButtonStyle.Box, Expanded = true)]
    public void LoadUI(string sceneName = "GUI")
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
            return;


        if (Application.isPlaying)
            SceneManager.LoadSceneAsync($"Scenes/{sceneName}", LoadSceneMode.Additive);
#if UNITY_EDITOR
        else
            EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity", OpenSceneMode.Additive);
#endif

        _loadedUiSceneName = sceneName;
    }

    //[Button]
    public void UnloadUI()
    {
        if (!SceneManager.GetSceneByName(_loadedUiSceneName).isLoaded)
            return;
        if (Application.isPlaying)
            SceneManager.UnloadSceneAsync(_loadedUiSceneName);
#if UNITY_EDITOR
        else
            EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByName(_loadedUiSceneName), true);
#endif

        _loadedUiSceneName = string.Empty;
    }



}
