using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectMapping : MonoBehaviour
{
    [Header("설정")]
    public Terrain targetTerrain;       // 배치할 터레인 오브젝트
    public GameObject prefabToSpawn;    // 배치할 오브젝트 프리팹
    public GameObject prefabToSpawn2;   // 배치할 오브젝트 프리팹
    public GameObject portal;           // 순간이동 오브젝트
    public int spawnCount = 10;         // 배치할 최종 개수
    public float minSpawnDistance = 2f; // 오브젝트 간 최소 이격 거리
    public int maxAttemptsPerObject = 50;

    private Transform spawnContainer;

    void Start()
    {
        if (targetTerrain == null)
        {
            Debug.LogError("Target Terrain이 설정되지 않았습니다.");
            return;
        }

        spawnContainer = new GameObject("SpawnedObjects").transform;
        spawnContainer.SetParent(transform);

        SpawnRandomObjectsOnTerrain();
    }

    public void SpawnRandomObjectsOnTerrain()
    {
        int placedCount = 0;
        int totalAttempts = 0;
        TerrainData terrainData = targetTerrain.terrainData;
        Vector3 terrainPosition = targetTerrain.transform.position;

        while (placedCount < spawnCount && totalAttempts < spawnCount * maxAttemptsPerObject)
        {
            totalAttempts++;

            // 1. 랜덤 XZ 좌표 (터레인 내부)
            float randX = Random.Range(0f, terrainData.size.x);
            float randZ = Random.Range(0f, terrainData.size.z);
            Vector3 worldPosCandidate = terrainPosition + new Vector3(randX, 0, randZ);

            // 2. 높이 적용
            float terrainHeight = targetTerrain.SampleHeight(worldPosCandidate);
            worldPosCandidate.y = terrainHeight;

            // 3. 겹침 체크
            if (IsPositionValid(worldPosCandidate))
            {
                GameObject newObj;

                if (totalAttempts % 3 != 0)
                {
                    newObj = Instantiate(prefabToSpawn, worldPosCandidate, Quaternion.identity, spawnContainer);
                }
                else if (totalAttempts % 10 == 0)
                {
                    // ⭐ 포탈 배치 + 터레인 기울기 회전 적용
                    newObj = Instantiate(portal, worldPosCandidate, Quaternion.identity, spawnContainer);

                    // 터레인 좌표를 0~1 사이로 정규화해서 노멀 추출
                    float normalizedX = (worldPosCandidate.x - terrainPosition.x) / terrainData.size.x;
                    float normalizedZ = (worldPosCandidate.z - terrainPosition.z) / terrainData.size.z;
                    Vector3 terrainNormal = terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);

                    // 노멀 방향에 맞춰 회전
                    Quaternion alignRotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);
                    newObj.transform.rotation = alignRotation;
                }
                else
                {
                    newObj = Instantiate(prefabToSpawn2, worldPosCandidate, Quaternion.identity, spawnContainer);
                }

                placedCount++;
            }

            if (totalAttempts >= spawnCount * maxAttemptsPerObject)
            {
                Debug.LogWarning($"최대 시도 횟수({totalAttempts}회)를 초과하여 {spawnCount}개 중 {placedCount}개만 배치되었습니다.");
                break;
            }
        }

        Debug.Log($"총 {placedCount}개의 오브젝트를 성공적으로 배치했습니다.");
    }

    bool IsPositionValid(Vector3 newPos)
    {
        foreach (Transform child in spawnContainer)
        {
            if (Vector3.Distance(child.position, newPos) < minSpawnDistance)
                return false;
        }
        return true;
    }
}
