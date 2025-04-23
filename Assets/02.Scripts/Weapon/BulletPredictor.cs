using UnityEngine;

public class BulletPredictor : MonoBehaviour
{
    [Header("총알 예측선")]
    public Transform firePoint;               // 총알 발사 위치
    public float maxDistance = 50f;           // 선이 얼마나 멀리까지 그려질지
    public LayerMask hitMask;                 // 맞을 수 있는 레이어

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        ShowBulletPath();
    }

    private void ShowBulletPath()
    {
        Vector3 from = firePoint.position;
        Vector3 direction = firePoint.forward;

        Ray ray = new Ray(from, direction);
        Vector3 to;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitMask))
        {
            to = hit.point;
        }
        else
        {
            to = from + direction * maxDistance;
        }

        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
    }
}
