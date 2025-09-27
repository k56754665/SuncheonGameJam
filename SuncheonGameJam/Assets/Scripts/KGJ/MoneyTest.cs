using UnityEngine;

public class MoneyTest : MonoBehaviour
{
    public void MoneyUp()
    {
        MoneyManager.Instance.AddMoney(10000);
    }

    public void MoneyDown()
    {
        MoneyManager.Instance.TrySpendMoney(10000);
    }
}
