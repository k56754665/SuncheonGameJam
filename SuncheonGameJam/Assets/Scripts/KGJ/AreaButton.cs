using NUnit;
using UnityEngine;
using UnityEngine.UI;

public class AreaButton : MonoBehaviour
{
    [SerializeField] private EnvironmentType _environmentType;
    private Button _button;
    [SerializeField, Range(1, 3)]
    private int _stage = 1;
    private SceneManagerSystem _scene = new();

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LoadAreaScene);
    }

    private void LoadAreaScene()
    {
        MiniGameManager.Instance.AnimalPoolData.SetMap(_environmentType, _stage - 1);
        _scene.LoadScene(GetAreaName(_environmentType,_stage));
    }
    
    private string GetAreaName(EnvironmentType environmentType,int stage)
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
