using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectMapping : MonoBehaviour
{
    [Header("설정")]
    public Terrain targetTerrain;       // 배치할 터레인 오브젝트
    public GameObject prefabToSpawn;     // 배치할 오브젝트 프리팹
    public GameObject prefabToSpawn2;     // 배치할 오브젝트 프리팹
    public int spawnCount = 10;          // 배치할 최종 개수
    public float minSpawnDistance = 2f; // 오브젝트 간 최소 이격 거리 (겹침 방지)
    public int maxAttemptsPerObject = 50; // 위치를 찾기 위한 최대 시도 횟수

    // 배치된 오브젝트들을 담을 컨테이너
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

    /// <summary>
    /// 터레인 지형에 지정된 수의 오브젝트를 겹치지 않게 랜덤 배치합니다.
    /// </summary>
    public void SpawnRandomObjectsOnTerrain()
    {
        int placedCount = 0;
        int totalAttempts = 0;
        TerrainData terrainData = targetTerrain.terrainData;
        Vector3 terrainPosition = targetTerrain.transform.position;

        // 배치해야 할 전체 개수만큼 반복
        while (placedCount < spawnCount && totalAttempts < spawnCount * maxAttemptsPerObject)
        {
            totalAttempts++;

            // 1. 랜덤 X, Z 좌표 생성 (터레인 경계 내)
            float randX = Random.Range(0f, terrainData.size.x);
            float randZ = Random.Range(0f, terrainData.size.z);
            
            // 2. 월드 좌표로 변환
            Vector3 worldPosCandidate = terrainPosition + new Vector3(randX, 0, randZ);

            // 3. 해당 지점의 터레인 높이(Y)를 가져옴
            float terrainHeight = targetTerrain.SampleHeight(worldPosCandidate);
            worldPosCandidate.y = terrainHeight;

            // 4. 겹침 확인 (배치된 오브젝트들과의 거리 체크)
            if (IsPositionValid(worldPosCandidate))
            {
                // 5. 위치가 유효하면 오브젝트 생성
                if( totalAttempts % 3 != 0 )
                {
                    GameObject newObj = Instantiate(prefabToSpawn, worldPosCandidate, Quaternion.identity, spawnContainer);
                }
                else
                {
                    GameObject newObj = Instantiate(prefabToSpawn2, worldPosCandidate, Quaternion.identity, spawnContainer);
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

    /// <summary>
    /// 새로운 위치가 기존에 배치된 오브젝트들과 충분히 떨어져 있는지 확인합니다.
    /// </summary>
    bool IsPositionValid(Vector3 newPos)
    {
        // Collider 체크를 사용하여 더 정확하게 겹침을 검사할 수도 있지만,
        // 여기서는 배치된 오브젝트들과의 거리만 체크하여 효율성을 높입니다.
        
        foreach (Transform child in spawnContainer)
        {
            // 배치된 오브젝트와의 거리 확인
            if (Vector3.Distance(child.position, newPos) < minSpawnDistance)
            {
                // 겹침 허용 거리보다 가까우면 유효하지 않음
                return false; 
            }
        }
        return true;
    }
}