using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BookIcon : MonoBehaviour
{
    [SerializeField] private Sprite openedImage;
    [SerializeField] private Sprite closedImage;
    private Image _image;
    private Button _button;
    private UI_Book _uiBook;
    private TMP_Text _text;
    private CharacterControl _characterControl;
    
    private void Start()
    {
        _characterControl = FindAnyObjectByType<CharacterControl>();
        _uiBook = FindAnyObjectByType<UI_Book>();
        _button = GetComponentInChildren<Button>();
        _image = GetComponentInChildren<Image>();
        _text = GetComponentInChildren<TMP_Text>();

        _button.onClick.AddListener(ToggleBook);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (UIManager.Instance.CurrentUI == UIManager.UIType.MiniGame ||
                UIManager.Instance.CurrentUI == UIManager.UIType.Map)
                return;
            
            ToggleBook();
        }
    }

    private void ToggleBook()
    {
        bool isOpen = _uiBook.ToggleBook();
        _characterControl.CanControl = !isOpen;
        _image.sprite = isOpen ? openedImage : closedImage;
        _text.text = isOpen ? "" : "도감";
    }
}
