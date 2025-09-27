using UnityEngine;

public class CloudMove : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 1f;             // 오른쪽 이동 속도
    public Vector2 startPosition;           // 되돌아올 시작 위치
    public Vector2 endPosition;             // 도착 후 리셋될 위치

    [Header("흔들림 설정")]
    public float verticalAmplitude = 0.5f;  // 위아래 흔들림 세기
    public float verticalFrequency = 1f;    // 흔들림 속도

    private float initialY;                 // 시작 Y 위치
    private float randomOffset;             // 각 구름마다 다른 흔들림 시작점
    public CloudSpawner spawner;      // 구름 스포너 참조

    void Start()
    {
        initialY = transform.position.y;
        randomOffset = Random.Range(0f, 100f); // 흔들림 패턴 랜덤화
    }

    void Update()
    {
        if(spawner == null)
        {
            return;
        }
        MoveCloud();
        SwayUpDown();
        CheckResetPosition();
    }

    // 구름 오른쪽으로 이동
    void MoveCloud()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    // 구름 위아래 흔들림
    void SwayUpDown()
    {
        float newY = initialY + Mathf.Sin(Time.time * verticalFrequency + randomOffset) * verticalAmplitude;
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }

    // 특정 위치 도달 시 위치 리셋
    void CheckResetPosition()
    {
        if (transform.position.x >= endPosition.x)
        {
            transform.position = new Vector3(startPosition.x, transform.position.y, transform.position.z);
            initialY = transform.position.y; // 새 위치에 맞춰 흔들림 기준점 갱신
        }
    }
}
