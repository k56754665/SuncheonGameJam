using UnityEngine;

public class MouseClick : MonoBehaviour
{
    public Sprite[] sprites;
    public UnityEngine.UI.Image image;
    public float frameRate = 0.2f; // 0.2초마다 스프라이트 변경

    private int currentFrameIndex = 0; // 현재 표시 중인 스프라이트의 인덱스
    private float timer = 0f; // 시간 측정을 위한 타이머
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Update()
    {
        // 1. 타이머 증가
        timer += Time.deltaTime;

        // 2. 설정된 시간(frameRate)이 지났는지 확인
        if (timer >= frameRate)
        {
            // 3. 타이머 초기화
            timer = 0f;

            // 4. 다음 프레임 인덱스 계산
            // 인덱스를 0과 1 사이에서 토글합니다. (currentFrameIndex + 1) % 2
            currentFrameIndex = (currentFrameIndex + 1) % sprites.Length; 
            
            // 5. 스프라이트 변경 적용
            image.sprite = sprites[currentFrameIndex];
        }
    }
}
