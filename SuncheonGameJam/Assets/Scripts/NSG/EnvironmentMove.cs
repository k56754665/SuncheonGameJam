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
        animator.SetTrigger("move");
    }
}
