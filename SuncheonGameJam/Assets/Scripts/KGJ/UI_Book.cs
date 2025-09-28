using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Book : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image image;
    
    private GameObject _bookCellPrefab;
    private ScrollRect _scrollRect;
    private Canvas _canvas;
    private Coroutine _waitCoroutine;

    private Vector2 _hiddenPos;
    private Vector2 _shownPos;
    private Tween _currentTween;
    
    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _bookCellPrefab = Resources.Load<GameObject>("UI/UI_BookCell");
        
        // 기준 위치 기록
        _shownPos = root.anchoredPosition;
        _hiddenPos = _shownPos + new Vector2(0, -1000); 
        root.anchoredPosition = _hiddenPos; // 시작은 숨김
    }

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = true;
        MakeBookCells();
        
        _canvas.enabled = false;
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
        bool isUnlocked = BookManager.Instance.IsUnlocked(animal.id);
        title.text = isUnlocked ? animal.animalName : "???";
        description.text = isUnlocked ? animal.animalDesription : "새로운 생물을 채집해 도감을 채워보세요!";
        image.sprite = animal.animalImage;
        image.enabled = isUnlocked;
    }

    /// <summary>
    /// 메서드 작동 후 book이 열렸는지 여부를 반환합니다.
    /// </summary>
    /// <returns>메서드 작동 후 book이 열렸으면 true, 닫혔으면 false</returns>
    public bool ToggleBook()
    {
        if (_canvas.enabled)
        {
            CloseBook();
            return false;
        }
        else
        {
            OpenBook();
            return true;
        }
    }

    private void OpenBook()
    {
        UIManager.Instance.TryOpen(UIManager.UIType.Book);
        _canvas.enabled = true;
        MakeBookCells();
        _scrollRect.verticalNormalizedPosition = 1;
        SetDetail(BookManager.Instance.GetAllEntries().First());
        
        _currentTween?.Kill();
        root.anchoredPosition = _hiddenPos;
        _currentTween = root.DOAnchorPos(_shownPos, 0.5f)
            .SetEase(Ease.OutCubic);
    }

    private void CloseBook()
    {
        _currentTween?.Kill();
        _currentTween = root.DOAnchorPos(_hiddenPos, 0.5f)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                _canvas.enabled = false;
                UIManager.Instance.Close(UIManager.UIType.Book);
            });
    }
}