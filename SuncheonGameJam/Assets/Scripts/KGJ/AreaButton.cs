using UnityEngine;
using UnityEngine.UI;

public class AreaButton : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private EnvironmentType _environmentType;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    private Button _button;
    [SerializeField, Range(1, 3)]
    private int _stage = 1;
    private SceneManagerSystem _scene = new();
    private MapManager _mapManager;
    
    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
        _mapManager = MapManager.Instance;
        _mapManager.OnAreaUnlockedEvent += HandleUnlocked;
        UpdateButtonSprite();
    }
    
    private void UpdateButtonSprite()
    {
        if (_mapManager.IsAreaUnlocked(id))
        {
            _button.image.sprite = unlockedSprite;
        }
        else
        {
            _button.image.sprite = lockedSprite;
        }
    }

    private void HandleUnlocked(int unlockedId)
    {
        if (id == unlockedId)
        {
            _button.image.sprite = unlockedSprite;
        }
    }

    private void HandleClick()
    {
        if (_mapManager.IsAreaUnlocked(id))
        {
            LoadAreaScene();
        }
        else
        {
            _mapManager.ShowAreaShop(id);
        }
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

    private void OnDestroy()
    {
        _mapManager.OnAreaUnlockedEvent -= HandleUnlocked;
    }
}
