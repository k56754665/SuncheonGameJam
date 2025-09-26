using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerSystem
{
    public void LoadScene(string sceneName)
    {
        // string sSceneName = sceneName.ToString();
        SceneManager.LoadScene(sceneName);
    }
}
