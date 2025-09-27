using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Map : MonoBehaviour
{
    public bool IsOpen { get; private set; }
    
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private RectTransform background;
    
    private CanvasGroup _canvasGroup;
    private CharacterControl _characterControl;

    private float _hidePos;
    private float _showPos;
    
    private void Start()
    {
        okButton.onClick.AddListener(LoadMapScene);
        cancelButton.onClick.AddListener(CloseMap);

        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _characterControl = FindAnyObjectByType<CharacterControl>();
    }

    public void ToggleMap()
    {
        if (_canvasGroup.blocksRaycasts)
        {
            CloseMap();
        }
        else
        {
            OpenMap();
        }
    }

    private void OpenMap()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _characterControl.CanControl = false;
        IsOpen = true;
    }

    private void CloseMap()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _characterControl.CanControl = true;
        IsOpen = false;
    }

    private void LoadMapScene()
    {
        SceneManager.LoadScene("SelectMap");
    }
}
