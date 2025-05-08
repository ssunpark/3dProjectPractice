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
    public Transform[] cameraTargets; // 0: TPS, 1: QT
    public CameraController cameraController;

    private void Start()
    {
        SetView(CurrentViewType);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8)) SetView(ViewType.FpsView);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SetView(ViewType.TpsView);
        if (Input.GetKeyDown(KeyCode.Alpha0)) SetView(ViewType.QTView);
    }

    private void SetView(ViewType viewType)
    {
        CurrentViewType = viewType;

        switch (viewType)
        {
            case ViewType.FpsView:
                _fpsPlayer.SetActive(true);
                _tpsPlayer.SetActive(false);
                break;
            case ViewType.TpsView:
                _fpsPlayer.SetActive(false);
                _tpsPlayer.SetActive(true);
                cameraController.SetTarget(cameraTargets[0]);
                break;
            case ViewType.QTView:
                _fpsPlayer.SetActive(false);
                _tpsPlayer.SetActive(true);
                cameraController.SetTarget(cameraTargets[1]);
                break;
        }
    }

}
