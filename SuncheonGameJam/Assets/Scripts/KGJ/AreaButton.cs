using NUnit;
using UnityEngine;
using UnityEngine.UI;

public class AreaButton : MonoBehaviour
{
    [SerializeField] private EnvironmentType _environmentType;
    private Button _button;
    private SceneManagerSystem _scene = new();

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LoadAreaScene);
    }

    private void LoadAreaScene()
    {
        _scene.LoadScene(GetAreaName(_environmentType));
    }
    
    private string GetAreaName(EnvironmentType environmentType)
    {
        string areaName;
        
        switch (environmentType)
        {
            case EnvironmentType.Reed:
                areaName = "ReedMap";
                break;
            case EnvironmentType.Mud:
                areaName = "MudMap";
                break;
            case EnvironmentType.Sea:
                areaName = "SeaMap";
                break;
            default:
                areaName = "ReedMap";
                break;
        }

        return areaName;
    }
}
