using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class CloudSpawner : MonoBehaviour
{
    [Header("구름 배치 설정")]
    public GameObject cloudPrefab; // 흘러가는 쉐이더가 적용된 구름 오브젝트 프리팹
    public Terrain targetTerrain;
    public int cloudCount = 20;     // 배치할 구름의 총 개수
    public float minSpawnDistance = 50f; // 구름 간 최소 이격 거리 (겹침 방지)
    public int maxAttemptsPerCloud = 100; // 위치를 찾기 위한 최대 시도 횟수

    [Header("하늘 영역 경계 (월드 좌표)")]
    public float minX; // 구름 배치 영역의 최소 X
    public float maxX;  // 구름 배치 영역의 최대 X
    public float minZ; // 구름 배치 영역의 최소 Z
    public float maxZ;  // 구름 배치 영역의 최대 Z

    [Header("높이 (Y축) 설정")]
    public float minHeight = 100f; // 구름이 배치될 최소 높이 (Y)
    public float maxHeight = 300f; // 구름이 배치될 최대 높이 (Y)

    private List<Vector3> spawnedPositions = new List<Vector3>(); // 이미 배치된 구름의 위치 리스트

    void Start()
    {
        if (cloudPrefab == null)
        {
            Debug.LogError("Cloud Prefab이 설정되지 않았습니다.");
            return;
        }
        minX = -targetTerrain.terrainData.bounds.max.x/2;
        minZ = -targetTerrain.terrainData.bounds.max.z/2;
        maxX = targetTerrain.terrainData.bounds.max.x/2;
        maxZ = targetTerrain.terrainData.bounds.max.z/2;
        Debug.Log(minX);
        SpawnCloudsRandomly();
    }

    /// <summary>
    /// 설정된 하늘 영역에 구름을 랜덤 위치 및 높이로 배치합니다.
    /// </summary>
    public void SpawnCloudsRandomly()
    {
        int placedCount = 0;
        int totalAttempts = 0;
        
        // 배치된 오브젝트들을 담을 부모 오브젝트 생성 (씬 정리용)
        Transform container = new GameObject("RandomlySpawnedClouds").transform;
        container.SetParent(transform);

        while (placedCount < cloudCount && totalAttempts < cloudCount * maxAttemptsPerCloud)
        {
            totalAttempts++;

            // 1. 랜덤 X, Z, Y 좌표 생성
            float randX = Random.Range(minX, maxX);
            float randZ = Random.Range(minZ, maxZ);
            float randY = Random.Range(minHeight, maxHeight); // 💡 랜덤 높이 적용
            
            Vector3 worldPosCandidate = new Vector3(randX, randY, randZ);

            // 2. 겹침 확인
            if (IsPositionValid(worldPosCandidate))
            {
                // 3. 위치가 유효하면 오브젝트 생성 및 위치 리스트에 추가
                GameObject newCloud = Instantiate(cloudPrefab, worldPosCandidate, Quaternion.identity, container);
                CloudMove cloudMove = newCloud.transform.GetComponent<CloudMove>();
                cloudMove.spawner = this;
                cloudMove.startPosition = new Vector2(minX-2, minZ);
                cloudMove.endPosition = new Vector2(maxX+2, maxZ);
                spawnedPositions.Add(worldPosCandidate);
                placedCount++;
            }
            
            if (totalAttempts >= cloudCount * maxAttemptsPerCloud)
            {
                Debug.LogWarning($"최대 시도 횟수({totalAttempts}회)를 초과하여 {cloudCount}개 중 {placedCount}개만 배치되었습니다.");
                break;
            }
        }

        Debug.Log($"총 {placedCount}개의 구름을 성공적으로 배치했습니다.");
    }

    /// <summary>
    /// 새로운 위치가 기존에 배치된 구름들과 충분히 떨어져 있는지 확인합니다.
    /// </summary>
    bool IsPositionValid(Vector3 newPos)
    {
        foreach (Vector3 existingPos in spawnedPositions)
        {
            // 배치된 구름과의 거리 확인
            // 💡 Y축(높이)도 포함하여 3D 공간에서의 겹침을 검사합니다.
            if (Vector3.Distance(existingPos, newPos) < minSpawnDistance)
            {
                return false; 
            }
        }
        return true;
    }
}