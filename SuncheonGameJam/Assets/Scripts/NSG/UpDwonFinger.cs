using UnityEngine;

public class UpDownFinger : MonoBehaviour
{
    // 움직임 설정
    [Header("움직임 설정")]
    [Tooltip("오브젝트가 얼마나 높이 떠오를지 (움직임의 최대 폭)")]
    public float amplitude = 0.5f; // 진폭 (Amplitude): 움직이는 거리
    
    [Tooltip("움직이는 속도 (클수록 빠름)")]
    public float frequency = 1f;    // 주파수 (Frequency): 움직이는 속도/빈도

    // 초기 위치 저장을 위한 변수
    private Vector3 startPosition;

    void Start()
    {
        // 씬 시작 시 오브젝트의 현재 위치를 저장합니다.
        startPosition = transform.position;
    }

    void Update()
    {
        // 1. 시간에 따른 사인파 값 계산
        // Mathf.Sin()은 주기적으로 -1.0에서 1.0 사이의 값을 반환합니다.
        // Time.time은 게임 시작 후 누적된 시간을 제공합니다.
        // frequency를 곱하여 속도를 제어합니다.
        float sinValue = Mathf.Sin(Time.time * frequency);

        // 2. 진폭(amplitude)을 적용하여 움직일 거리를 결정
        float offset = sinValue * amplitude;

        // 3. 새로운 위치 계산
        // 저장된 초기 Y 위치에 계산된 offset을 더하여 새로운 Y 위치를 만듭니다.
        Vector3 newPosition = startPosition;
        newPosition.y += offset;

        // 4. 오브젝트 위치 업데이트
        transform.position = newPosition;

        transform.LookAt(Camera.main.transform.position, Vector3.down);
    }
}