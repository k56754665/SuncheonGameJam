using System;
using UnityEngine;

public class UI_MapIcon : MonoBehaviour
{
    private UI_Map _uiMap;
    
    private void Start()
    {
        _uiMap = FindAnyObjectByType<UI_Map>();
        
    }

    private void Update()
    {
        if (_uiMap != null && FindAnyObjectByType<UI_Book>().IsOpen)
            return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            _uiMap.ToggleMap();
        }
    }
}
