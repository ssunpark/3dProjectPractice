using UnityEngine;

public class ObstacleMoving : MonoBehaviour
{
    public Transform pointA; // ���� ��ġ
    public Transform pointB; // �� ��ġ
    public float speed = 1.0f; // �̵� �ӵ�

    void Update()
    {
        // �ð��� ���� PingPong ���� ��� (0~1 ����)
        float time = Mathf.PingPong(Time.time * speed, 1f);

        // A�� B ���̸� Lerp�� ����
        transform.position = Vector3.Lerp(pointA.position, pointB.position, time);
    }
}