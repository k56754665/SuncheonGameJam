using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BookIcon : MonoBehaviour
{
    private Button _button;
    private UI_Book _uiBook;
    private TMP_Text _text;
    
    private void Start()
    {
        _uiBook = FindAnyObjectByType<UI_Book>();
        _button = GetComponentInChildren<Button>();
        _text = GetComponentInChildren<TMP_Text>();

        _button.onClick.AddListener(ToggleBook);
    }

    private void ToggleBook()
    {
        bool isOpen = _uiBook.ToggleBook();
        _text.text = isOpen ? "×" : "도감";
    }
}
