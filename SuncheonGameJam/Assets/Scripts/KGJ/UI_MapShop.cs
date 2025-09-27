using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapShop : MonoBehaviour
{
    [SerializeField] private TMP_Text description;
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;
    private MapManager _mapManager;
    private Canvas _canvas;
    private int _id;
    
    private void Start()
    {
        _mapManager = MapManager.Instance;
        _mapManager.OnShowAreaShopEvent += ShowUI;
        _yesButton.onClick.AddListener(ClickYesButton);
        _noButton.onClick.AddListener(HideUI);
        _canvas = GetComponent<Canvas>();
    }

    private void ClickYesButton()
    {
        bool UnlockSuccess = _mapManager.TryUnlockArea(_id);
        if (UnlockSuccess)
        {
            HideUI();
        }
    }
    
    private void ShowUI(int id)
    {
        _id = id;
        string areaName = _mapManager.GetAreaName(id);
        int cost = _mapManager.GetAreaCost(id);
        description.text = $"{cost / 10000}만원으로 <{areaName}>을\n해금하시겠습니까?";
        _canvas.enabled = true;
    }

    private void HideUI()
    {
        _canvas.enabled = false;
    }

    private void OnDestroy()
    {
        _mapManager.OnShowAreaShopEvent -= ShowUI;
    }
}
