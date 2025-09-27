using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartBtn : MonoBehaviour
{
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LoadSelectMap);
    }

    private void LoadSelectMap()
    {
        SceneManager.LoadScene("SelectMap");
    }
}
