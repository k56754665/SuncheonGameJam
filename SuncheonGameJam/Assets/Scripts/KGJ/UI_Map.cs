using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Map : MonoBehaviour
{
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
        if (UIManager.Instance.TryOpen(UIManager.UIType.Map))
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _characterControl.CanControl = false;
        }
    }

    private void CloseMap()
    {
        if (UIManager.Instance.CurrentUI != UIManager.UIType.Map)
            return;
        
        UIManager.Instance.Close(UIManager.UIType.Map);
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _characterControl.CanControl = true;
    }

    private void LoadMapScene()
    {
        SceneManager.LoadScene("SelectMap");
    }
}
