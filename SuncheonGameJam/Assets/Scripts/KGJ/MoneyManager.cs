using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoneyManager : Singleton<MoneyManager>
{
    public float CurrentMoney { get; private set; }
    private float _currentMoney;
    private const float GoalMoney = 10000000; // 천만

    /// <summary>
    /// 번 돈을 파라미터로 넘기는 액션
    /// </summary>
    public event Action<float> OnMoneyAdded;

    /// <summary>
    /// 쓴 돈을 파라미터로 넘기는 액션
    /// </summary>
    public event Action<float> OnMoneyRemoved;
    
    public bool IsGoal()
    {
        return CurrentMoney >= GoalMoney;
    }

    /// <summary>
    /// 돈을 얻는다.
    /// </summary>
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        CurrentMoney += amount;

        OnMoneyAdded?.Invoke(amount);
        
        if (IsGoal())
        {
            SceneManager.LoadScene("EndingScene");
        }
    }

    /// <summary>
    /// 돈이 충분하면 차감하고 true 반환, 아니면 false 반환.
    /// </summary>
    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0) return false;

        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            OnMoneyRemoved?.Invoke(amount);
            return true;
        }

        return false;
    }
}