using TMPro;
using UnityEngine;

public class UI_Money : MonoBehaviour
{
    [SerializeField] private Transform moneyRoot;
    [SerializeField] private TMP_Text moneyText;
    
    private GameObject _giftMoneyPrefab;
    private MoneyManager _moneyManager;
    private MoneyPopupText _moneyPopupText;
    
    private void Awake()
    {
        _giftMoneyPrefab = Resources.Load<GameObject>("UI/UI_GiftMoney");
    }

    private void Start()
    {
        _moneyPopupText = GetComponentInChildren<MoneyPopupText>();
        _moneyManager = MoneyManager.Instance;
        _moneyManager.OnMoneyAdded += MakeMoney;
        _moneyManager.OnMoneyAdded += ShowMoneyPopup;
        _moneyManager.OnMoneyRemoved += UpdateMoneyText;
        UpdateMoneyText(_moneyManager.CurrentMoney);
    }

    private void ShowMoneyPopup(float amount)
    {
        _moneyPopupText.ShowPopup(amount);
    }
    
    private void UpdateMoneyText(float amount)
    {
        moneyText.text = $"{MoneyManager.Instance.CurrentMoney / 10000} 만원";
    }

    private int GetGiftCount(float amount)
    {
        if (amount < 100000) return 3;      // 10만 미만
        if (amount < 1000000) return 5;     // 백만 미만
        if (amount < 5000000) return 7;     // 오백만 미만
        return 9;                          // 그 이상
    }

    private void MakeMoney(float amount)
    {
        int count = GetGiftCount(amount);
        bool executed = false; // 한 번만 실행되도록 플래그

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(_giftMoneyPrefab, transform);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;

            var giftMoney = go.GetComponent<UI_GiftMoney>();

            giftMoney.OnEffectComplete += () =>
            {
                if (executed) return; // 이미 실행했다면 무시
                executed = true;

                //ShowMoneyPopup(amount);
                UpdateMoneyText(amount);
            };

            giftMoney.PlayEffect(moneyRoot.position);
        }
    }

    
    private void OnDestroy()
    {
        _moneyManager.OnMoneyAdded -= MakeMoney;
        _moneyManager.OnMoneyAdded -= ShowMoneyPopup;
        _moneyManager.OnMoneyRemoved -= UpdateMoneyText;
    }
}
