using UnityEngine;

public class MinimapControl : MonoBehaviour
{
    public Camera MinimapCamera;
    [SerializeField] private float _zoomStep = 1f;
    [SerializeField] private float _minZoom = 7f;
    [SerializeField] private float _maxZoom = 20f;

    // 초기 줌 값 설정
    private void Start()
    {
        MinimapCamera.orthographicSize = Mathf.Clamp(MinimapCamera.orthographicSize, _minZoom, _maxZoom);
    }

    public void ZoomIn()
    {
        MinimapCamera.orthographicSize = Mathf.Clamp(MinimapCamera.orthographicSize - _zoomStep, _minZoom, _maxZoom);
    }
    public void ZoomOut()
    {
        MinimapCamera.orthographicSize = Mathf.Clamp(MinimapCamera.orthographicSize + _zoomStep, _minZoom, _maxZoom);
    }
}
