using UnityEngine;

public enum ViewType
{
    FpsView,
    TpsView,
    QTView
}

public class PlayerManager : MonoBehaviour
{
    // TPS, FPS ���� ��ȯ
    public ViewType CurrentViewType = ViewType.FpsView;

    // �ʿ� �Ӽ�
    public GameObject _fpsPlayer;
    public GameObject _tpsPlayer;

    private void Start()
    {
        _fpsPlayer.SetActive(true);
        _tpsPlayer.SetActive(false);
    }
    private void Update()
    {
        // FPS ���� ��ȯ
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
