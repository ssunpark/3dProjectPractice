using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    // ī�޶� ȸ�� ��ũ��Ʈ
    // ��ǥ: ���콺�� �����ϸ� ī�޶� �� �������� ȸ����Ű�� �ʹ�.
    public float RotationSpeed = 500f;

    // ī�޶� ������ 0���������� �����Ѵٰ� ������ �����.
    private float _rotationX = 0;
    private float _rotationY = 0;

    private void Update()
    {
        // ���� ����
        // 1. ���콺 �Է��� �޴´�.(���콺 Ŀ���� ������ ����)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Debug.Log($"Mouse X: {mouseX}, Mouse Y: {mouseY}");

        // 2. ���콺 �Է����κ��� ȸ����ų ������ �����.
        // TODO: ���콺 ��ǥ��� ȭ�� ��ǥ���� �������� �˰�, ���۵��ϵ��� �Ʒ� ������ �ڵ带 �����غ���.
        _rotationX += mouseX * RotationSpeed * Time.deltaTime;
        _rotationY += mouseY * RotationSpeed * Time.deltaTime;
        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);

        // 3. ȸ�� �������� ȸ����Ų��
        transform.eulerAngles = new Vector3(-_rotationY, _rotationX, 0);
        //Vector3 dir = new Vector3(-mouseY, mouseX, 0);

        //// 3. ī�޶� �� �������� ȸ���Ѵ�.
        //// ���ο� ��ġ = ���� ��ġ + �ӵ�*�ð�
        //// ���ο� ���� = ���� ���� + ȸ�� �ӵ�*�ð�
        //transform.eulerAngles = transform.eulerAngles + dir * RotationSpeed * Time.deltaTime;

        //// ȸ���� ���� ������ �ʿ��ϴ�!(-90 ~ 90)
        //Vector3 rotation = transform.eulerAngles;
        //rotation.x = Mathf.Clamp(rotation.x, -90f, 90f); // ����Ƽ�� ���̳ʽ� ������ ����.
        //transform.eulerAngles = rotation;




    }
}
