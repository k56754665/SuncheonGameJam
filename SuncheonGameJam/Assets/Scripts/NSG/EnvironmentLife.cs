using UnityEngine;

public class EnvironmentLife : MonoBehaviour
{
    public int health = 3;
    public GameObject protalImage1;
    public GameObject protalImage2;
    public ParticleSystem particle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 3;
        protalImage1.SetActive(true);
        protalImage2.SetActive(false);
    }
    public void Damage()
    {
        if (health <= 0)
        {
            Debug.Log("이미 작동됨");
            return;
        }
        health -= 1;
        if (health == 1)
        {
            Debug.Log("포탈 파괴효과 시작 후 미니 게임 시작");
            protalImage1.SetActive(false);
            protalImage2.SetActive(true);
        }
        if (health <= 0)
        {
            Debug.Log("포탈 파괴효과 시작 후 미니 게임 시작");
        }
        particle.Play();
    }

}
