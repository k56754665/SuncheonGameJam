using UnityEngine;

public class Test : MonoBehaviour
{
    public AnimalStruct animal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("나의 등급 : " + animal.monsterLevel);
        //Debug.Log("나의 현상금 : " +animal.Bounties[(int)animal.monsterLevel]);
        Debug.Log("나의 환경 : " +animal.environment);
        Debug.Log(animal.animalDesription);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
