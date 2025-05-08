using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform currentTarget;
    public float positionSmooth = 10f;
    public float rotationSmooth = 10f;

    private void Update()
    {
        if (!GameManager.Instance.CanMove())
        {
            return;
        }
    }
    private void LateUpdate()
    {
        if (currentTarget == null) return;

        // �ε巴�� ���󰡱�
        transform.position = Vector3.Lerp(transform.position, currentTarget.position, Time.deltaTime * positionSmooth);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentTarget.rotation, Time.deltaTime * rotationSmooth);
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}
