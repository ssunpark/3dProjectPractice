using UnityEngine;

public enum ViewType
{
    FpsView,
    TpsView,
    QTView
}

public class PlayerManager : MonoBehaviour
{
    // TPS, FPS 시점 전환
    public ViewType CurrentViewType = ViewType.FpsView;

    // 필요 속성
    public GameObject _fpsPlayer;
    public GameObject _tpsPlayer;

    private void Start()
    {
        _fpsPlayer.SetActive(true);
        _tpsPlayer.SetActive(false);
    }
    private void Update()
    {
        // FPS 모드로 변환
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CurrentViewType = ViewType.FpsView;
            _tpsPlayer.SetActive(false);
            _fpsPlayer.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CurrentViewType = ViewType.TpsView;
            _fpsPlayer.SetActive(false);
            _tpsPlayer.SetActive(true);
        }
    }

}
