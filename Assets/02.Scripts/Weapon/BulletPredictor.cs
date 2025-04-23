using UnityEngine;

public class BulletPredictor : MonoBehaviour
{
    [Header("�Ѿ� ������")]
    public Transform firePoint;               // �Ѿ� �߻� ��ġ
    public float maxDistance = 50f;           // ���� �󸶳� �ָ����� �׷�����
    public LayerMask hitMask;                 // ���� �� �ִ� ���̾�

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
