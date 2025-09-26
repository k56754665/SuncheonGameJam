using System;
using System.Collections.Generic;
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
    public int[] Bounties = new int[6];
    public float difficulty;
    [TextArea]
    public string animalDesription;
    
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            id = Guid.NewGuid().ToString("N"); // 고유 GUID
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
}
