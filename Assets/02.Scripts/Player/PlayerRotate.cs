using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 500f;
    private float _rotationX = 0;

    private void Start()
    {
        _rotationX = 0f;
        transform.eulerAngles = new Vector3(0, _rotationX, 0);
    }
    private void Update()
    {
        if (!GameManager.Instance.CanMove())
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X");
        _rotationX += mouseX * RotationSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, _rotationX, 0);
        //transform.Rotate(Vector3.up * mouseX * RotationSpeed * Time.deltaTime);
    }
}
