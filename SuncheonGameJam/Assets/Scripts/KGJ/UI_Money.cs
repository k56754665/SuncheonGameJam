using TMPro;
using UnityEngine;

public class UI_Money : MonoBehaviour
{
    [SerializeField] private Transform moneyRoot;
    [SerializeField] private TMP_Text moneyText;
    private GameObject _giftMoneyPrefab;
    private MoneyManager _moneyManager;
    
    private void Awake()
    {
        _giftMoneyPrefab = Resources.Load<GameObject>("UI/UI_GiftMoney");
    }

    private void Start()
    {
        _moneyManager = MoneyManager.Instance;
        _moneyManager.OnMoneyAdded += UpdateMoneyText;
        _moneyManager.OnMoneyAdded += MakeMoney;
        _moneyManager.OnMoneyRemoved += UpdateMoneyText;
    }

    private void UpdateMoneyText(float amount)
    {
        moneyText.text = $"순천사랑상품권 : {MoneyManager.Instance.CurrentMoney}";
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

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(_giftMoneyPrefab, transform);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero; // 중앙에서 시작

            var giftMoney = go.GetComponent<UI_GiftMoney>();

            // 이펙트 실행
            giftMoney.PlayEffect(moneyRoot.position);
        }
    }

    private void OnDestroy()
    {
        _moneyManager.OnMoneyAdded -= UpdateMoneyText;
        _moneyManager.OnMoneyAdded -= MakeMoney;
        _moneyManager.OnMoneyRemoved -= UpdateMoneyText;
    }
}
