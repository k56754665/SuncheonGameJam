using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameResult : MonoBehaviour
{
    [SerializeField]
    private AnimalStruct _currentAnimal;
    [SerializeField]
    private TextMeshProUGUI _gradeText;
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _descText;
    [SerializeField]
    private Image _image;
    
    public void SetSuccess(AnimalStruct animal)
    {
        _currentAnimal = animal;
        if (_gradeText) _gradeText.text = animal != null ? animal.monsterLevel.ToString() : "Unknown";
        if (_nameText) _nameText.text = animal != null ? animal.animalName : "Unknown";
        if (_descText) _descText.text = animal != null ? animal.animalDesription : "No Description";
        if (_image) _image.sprite = animal != null ? animal.animalImage : null;
        
    }
    
}