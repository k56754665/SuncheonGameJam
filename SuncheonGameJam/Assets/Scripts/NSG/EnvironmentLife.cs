using UnityEngine;

public class EnvironmentLife : MonoBehaviour
{
    int health = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 3;
    }
    public void Damage()
    {
        if (health <= 0)
        {
            Debug.Log("이미 작동됨");
            return;
        }
        health -= 1;
        if (health <= 0)
        {
            Debug.Log("포탈 파괴효과 시작 후 미니 게임 시작");
        }
    }

}
