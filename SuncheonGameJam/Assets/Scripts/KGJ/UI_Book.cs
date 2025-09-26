using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Book : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private UI_BookCell detailPanel; // 오른쪽 상세 정보 패널
    private GameObject _bookCellPrefab;
    private ScrollRect _scrollRect;
    private Canvas _canvas;
    
    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _bookCellPrefab = Resources.Load<GameObject>("UI/UI_BookCell");
    }

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        MakeBookCells();
    }

    private void MakeBookCells()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        List<AnimalStruct> entries = BookManager.Instance.GetAllEntries();

        foreach (AnimalStruct entry in entries)
        {
            GameObject go = Instantiate(_bookCellPrefab, contentRoot);
            UI_BookCell cell = go.GetComponent<UI_BookCell>();
            cell.SetBookData(entry);

            cell.onClick = () => SetDetail(entry);
        }
    }

    private void SetDetail(AnimalStruct animal)
    {
        if (animal == null)
        {
            Debug.Log("animal is null");
        }
        bool isUnlocked = BookManager.Instance.IsUnlocked(animal.id);
        title.text = isUnlocked ? animal.animalName : "???";
        description.text = isUnlocked ? animal.animalDesription : "";
        detailPanel.SetBookData(animal);
    }

    public void ToggleBook()
    {
        if (_canvas.enabled)
        {
            CloseBook();
        }
        else
        {
            OpenBook();
        }
    }

    private void OpenBook()
    {
        MakeBookCells();
        _scrollRect.verticalNormalizedPosition = 1;
        _canvas.enabled = true;
        
        SetDetail(BookManager.Instance.GetAllEntries().First());
    }

    private void CloseBook()
    {
        _canvas.enabled = false;
    }
}