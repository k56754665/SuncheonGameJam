using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BookCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;
    [SerializeField] private Image frame;
    [SerializeField] private TMP_Text rankText;

    private AnimalStruct _data;
    public Action onClick;
    public void SetBookData(AnimalStruct animal)
    {
        bool isUnlocked = BookManager.Instance.IsUnlocked(animal.id);
        MonsterLevelType highest = BookManager.Instance.GetHighestLevel(animal.id);

        image.sprite = animal.animalImage;
        image.color = isUnlocked ? Color.white : Color.black;
        rankText.text = $"{highest}";
        // 테두리도 변경 필요
    }

    public AnimalStruct GetData() => _data;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}