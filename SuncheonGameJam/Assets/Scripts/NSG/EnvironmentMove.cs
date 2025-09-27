using UnityEngine;

public class EnvironmentMove : MonoBehaviour
{
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void MoveStart()
    {
        try
        {
        animator.SetTrigger("move");
        }catch(System.Exception e)
        {
            Debug.Log("애니메이터 없음");
        }
    }
}
