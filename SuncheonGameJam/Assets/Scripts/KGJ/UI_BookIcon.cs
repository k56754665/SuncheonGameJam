using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_BookIcon : MonoBehaviour
{
    private Button _button;
    private UI_Book _uiBook;
    
    private void Start()
    {
        _uiBook = FindAnyObjectByType<UI_Book>();
        _button = GetComponentInChildren<Button>();

        _button.onClick.AddListener(ToggleBook);
    }

    private void ToggleBook()
    {
        _uiBook.ToggleBook();
    }
}
