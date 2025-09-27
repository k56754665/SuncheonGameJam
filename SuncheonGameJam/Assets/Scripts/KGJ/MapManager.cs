using System;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    private string[] _areaNames = new string[7] { "장산소공원", "갯벌관찰장", "대대선장", "안풍습지", "탐조쉼터", "생태체험장", "람사르길" };
    [SerializeField]
    private int[] _areaCost = new int[7];
    private bool[] _unlockedArea = new bool[7] { true, false, false, false, false, false, false };

    public event Action<int> OnAreaUnlockedEvent;
    public event Action<int> OnShowAreaShopEvent;

    public string GetAreaName(int id)
    {
        return _areaNames[id];
    }

    public int GetAreaCost(int id)
    {
        return _areaCost[id];
    }
    
    public bool IsAreaUnlocked(int area)
    {
        return _unlockedArea[area];
    }

    public void ShowAreaShop(int area)
    {
        OnShowAreaShopEvent?.Invoke(area);
    }
    
    /// <summary>
    /// 돈을 지불하고 지역을 해금 시도
    /// </summary>
    /// <param name="area">해금할 지역 번호</param>
    /// <returns>해금 성공 여부</returns>
    public bool TryUnlockArea(int area)
    {
        // 이미 해금된 지역이라면 실패
        if (_unlockedArea[area])
        {
            return false;
        }

        int cost = _areaCost[area];

        // 돈이 충분한지 확인
        if (MoneyManager.Instance.CurrentMoney >= cost)
        {
            // 돈 차감
            MoneyManager.Instance.TrySpendMoney(cost);

            // 해금 처리
            _unlockedArea[area] = true;
            
            OnAreaUnlockedEvent?.Invoke(area);

            return true;
        }
        else
        {
            return false;
        }
    }
}