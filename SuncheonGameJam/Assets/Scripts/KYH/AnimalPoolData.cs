using System;
using UnityEngine;
using System.Collections.Generic;

public class AnimalPoolData : MonoBehaviour
{
    public List<AnimalStruct> animals;
    public List<AnimalStruct> reedAnimals;
    public List<AnimalStruct> mudAnimals;
    public List<AnimalStruct> seaAnimals;
    public List<AnimalStruct> currentPool;
    public float[][] rareRates = new float[][]
    {
        // stage 1: C,B,A 만 (S,SS,SSS는 0)
        new float[] { 0.50f, 0.30f, 0.19f, 0.00f, 0.00f, 0.01f },

        // stage 2: B,A,S
        new float[] { 0.00f, 0.50f, 0.29f, 0.19f, 0.00f, 0.02f },

        // stage 3: A,S,SS
        new float[] { 0.00f, 0.00f, 0.49f, 0.29f, 0.19f, 0.03f },
    };
    public float[] currentRareRate;
    
    
    private void Start()
    {
        animals = BookManager.Instance.GetAllEntries();
        reedAnimals = animals.FindAll(a => a.environment == EnvironmentType.Reed);
        mudAnimals = animals.FindAll(a => a.environment == EnvironmentType.Mud);
        seaAnimals = animals.FindAll(a => a.environment == EnvironmentType.Sea);
    }

    public void SetMap(EnvironmentType environment, int stage)
    {
        switch (environment)
        {
            case EnvironmentType.Reed:
                currentPool = reedAnimals;
                break;
            case EnvironmentType.Mud:
                currentPool = mudAnimals;
                break;
            case EnvironmentType.Sea:
                currentPool = seaAnimals;
                break;
            default:
                currentPool = reedAnimals;
                break;
        }
        currentRareRate = rareRates[Mathf.Clamp(stage, 0, rareRates.Length - 1)];
    }
    
    public AnimalStruct PickAnimal()
    {
        // 풀에서 하나 선택
        if (currentPool == null || currentPool.Count == 0) return default;
        AnimalStruct picked = currentPool[UnityEngine.Random.Range(0, currentPool.Count)];

        // 레벨 가중치로 등급 하나 선택 (C,B,A,S,SS,SSS = 0..5)
        if (currentRareRate == null || currentRareRate.Length < 6) return picked;

        float sum = 0f;
        for (int i = 0; i < 6; i++) sum += Mathf.Max(0f, currentRareRate[i]);
        if (sum <= 0f) return picked;

        float r = UnityEngine.Random.value * sum;
        float acc = 0f;
        int chosenIdx = 0;
        for (int i = 0; i < 6; i++)
        {
            float w = Mathf.Max(0f, currentRareRate[i]);
            if (w <= 0f) continue;
            acc += w;
            if (r < acc) { chosenIdx = i; break; }
        }

        picked.monsterLevel= (MonsterLevelType)chosenIdx;
        return picked;
    }
    
    
}
