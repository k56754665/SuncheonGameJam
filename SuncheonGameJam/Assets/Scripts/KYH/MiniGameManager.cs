using UnityEngine;
using UnityEngine.Serialization;

public class MiniGameManager : Singleton<MiniGameManager>
{
    [FormerlySerializedAs("fishingUIRoot")]
    [Header("Fishing UI Root")]
    [SerializeField] private GameObject MiniGameCanvas;

    [Header("Fishing Components")]
    [SerializeField] private InertiaHandleUI handleCtrl;
    [SerializeField] private TargetNoiseMoverUI targetCtrl;
    [SerializeField] private FishingUIController fishingCtrl;

    private void OnEnable()
    {
        EventBus.SubscribeStartMiniGame(OnStartMiniGame);
        EventBus.SubscribeEndMiniGame(OnEndMiniGame);
    }

    private void OnDisable()
    {
        EventBus.UnsubscribeStartMiniGame(OnStartMiniGame);
        EventBus.UnsubscribeEndMiniGame(OnEndMiniGame);
    }

    private void Start()
    {
        if (MiniGameCanvas) MiniGameCanvas.SetActive(false);
        if (handleCtrl) handleCtrl.enabled = false;
        if (targetCtrl) targetCtrl.enabled = false;
        if (fishingCtrl) fishingCtrl.enabled = false;
    }

    public void OnStartMiniGame()
    {
        if (!MiniGameCanvas) return;
        MiniGameCanvas.SetActive(true);

        if (handleCtrl) handleCtrl.enabled = true;
        if (targetCtrl) targetCtrl.enabled = true;
        if (fishingCtrl)
        {
            fishingCtrl.enabled = true;
            fishingCtrl.Begin(); // 진행도/타이머 초기화 (FishingUIController에 이미 구현됨)
        }
    }

    public void OnEndMiniGame(bool success)
    {
        // TODO: 성공/실패 보상 처리

        if (MiniGameCanvas) MiniGameCanvas.SetActive(false);

        if (handleCtrl) handleCtrl.enabled = false;
        if (targetCtrl) targetCtrl.enabled = false;
        if (fishingCtrl) fishingCtrl.enabled = false;
    }
}
