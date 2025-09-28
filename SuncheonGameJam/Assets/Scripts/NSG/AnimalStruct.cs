using System;
using UnityEngine;
public enum MonsterLevelType
{
    C,
    B,
    A,
    S,
    SS,
    SSS
}
public enum EnvironmentType
{
    Reed,
    Mud,
    Sea
}
[CreateAssetMenu(fileName = "AnimalStruct", menuName = "AnimalObject/AnimalStruct")]
public class AnimalStruct : ScriptableObject
{
    public string id;
    public string animalName;
    public Sprite animalImage;
    public MonsterLevelType monsterLevel;
    public EnvironmentType environment;
    public float difficulty;
    [TextArea]
    public string animalDesription;
    public float baseBounty;
    
    /// <summary>
    /// rarityIndex(0~5)에 따른 현상금을 반환
    /// </summary>
    public int GetBounty(int rarityIndex)
    {
        float y = baseBounty * Mathf.Pow(rarityIndex + 1, 2);

        int bounty = Mathf.RoundToInt(y / 10000f) * 10000;

        if (rarityIndex >= 4)
            bounty *= 5;

        return bounty;
    }
}
