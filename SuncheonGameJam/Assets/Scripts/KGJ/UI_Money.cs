using UnityEngine;

public class UI_Money : MonoBehaviour
{
    [SerializeField] private Transform moneyRoot;
    private GameObject _giftMoneyPrefab;

    private void Awake()
    {
        _giftMoneyPrefab = Resources.Load<GameObject>("UI/UI_GiftMoney");
    }

    private int GetGiftCount(float amount)
    {
        if (amount < 100000) return 3;      // 10만 미만
        if (amount < 1000000) return 5;     // 백만 미만
        if (amount < 5000000) return 7;     // 오백만 미만
        return 9;                          // 그 이상
    }

    public void MakeMoney(float amount)
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

    // 디버그용 버튼
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));

        if (GUILayout.Button("Make 100,000")) MakeMoney(100000);
        if (GUILayout.Button("Make 1,000,000")) MakeMoney(1000000);
        if (GUILayout.Button("Make 10,000,000")) MakeMoney(10000000);

        GUILayout.EndArea();
    }
}
