using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BookCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private GameObject mask;
    [SerializeField] private TMP_Text questionMark;

    private AnimalStruct _data;
    public Action onClick;
    
    public void SetBookData(AnimalStruct animal)
    {
        bool isUnlocked = BookManager.Instance.IsUnlocked(animal.id);
        MonsterLevelType highest = BookManager.Instance.GetHighestLevel(animal.id);

        if (!isUnlocked)
        {
            mask.SetActive(false);
            rankText.text = "";
            questionMark.text = "?";
        }
        else
        {
            mask.SetActive(true);
            image.sprite = animal.animalImage;
            rankText.text = $"{highest}";
            questionMark.text = String.Empty;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}