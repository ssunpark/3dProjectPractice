using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;

    private void Update()
    {
        // ����(interpoling), smoothing ����� �� ����
        transform.position = Target.position;
    }
}
