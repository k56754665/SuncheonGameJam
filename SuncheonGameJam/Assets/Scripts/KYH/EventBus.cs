using System;
using UnityEngine.SceneManagement;


//GetInvocationList().Contains(handler) 를 사용하여 중복 구독을 방지할 수 있지만, 안쓰고 있어요.
public static class EventBus
{
    private static bool _isInitialized = false;
    
    
    private static void Init()
    {
        if (_isInitialized) return;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        _isInitialized = true;
    }
    
    //SceneLoaded 관련 이벤트
    private static Action _onSceneLoaded;
    public static void SubscribeSceneLoaded(Action handler) => _onSceneLoaded += handler;
    public static void UnsubscribeSceneLoaded(Action handler) => _onSceneLoaded -= handler;
    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();
        _onSceneLoaded?.Invoke();
    }
    
    private static Action _onStartMiniGame;
    public static void SubscribeStartMiniGame(Action handler) => _onStartMiniGame += handler;
    public static void UnsubscribeStartMiniGame(Action handler) => _onStartMiniGame -= handler;
    public static void PublishStartMiniGame() => _onStartMiniGame?.Invoke();
    
    private static Action<bool> _onEndMiniGame;
    public static void SubscribeEndMiniGame(Action<bool> handler) => _onEndMiniGame += handler;
    public static void UnsubscribeEndMiniGame(Action<bool> handler) => _onEndMiniGame -= handler;
    public static void PublishEndMiniGame(bool isSuccess) => _onEndMiniGame?.Invoke(isSuccess);

    

    
}