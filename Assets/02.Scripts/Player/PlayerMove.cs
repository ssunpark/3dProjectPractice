using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // ��ǥ: wasd�� ������ ĳ���͸� ī�޶� ���⿡ �°� �̵���Ű�� �ʹ�.
    // �ʿ� �Ӽ�:
    // - �̵� �ӵ�
    [Header("�ӵ�")]
    public float MoveSpeed = 7f;
    public float RunSpeed = 12f; // �÷��̾� �� �� �ӵ�
    public float RollSpeed = 20f;

    [Header("����")]
    public float JumpPower = 5f;
    private const float GRAVITY = -9.8f; // �߷�

    private float _yVelocity = 0f; // �߷°��ӵ�
    public bool _isRunning = false;
    public bool _isRolling = false;
    public int _jumpCount = 0;

    private CharacterController _chracterController;

    private void Awake()
    {
        _chracterController = GetComponent<CharacterController>();
    }

    // ���� ����:
    // 1. Ű���� �Է��� �޴´�.
    // 2. �Է����κ��� ������ �����Ѵ�.
    // 3. ���⿡ ���� �÷��̾ �̵��Ѵ�.

    private void Update()
    {
        InitializeState();
        PlayerMoving();
    }

    private void InitializeState()
    {
        // ĳ���Ͱ� �� ���� �ִٸ� 
        if (_chracterController.isGrounded) // ���� �ǹ̷�, if(_chracterController.collisionFlags == CollisionFlags.Below & CollisionFlags.Sides)
        {
            _isRunning = false;
            _isRolling = false;
            _jumpCount = 0;
        }
    }

    private void PlayerMoving()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;
        dir = Camera.main.transform.TransformDirection(dir);
        // TransformDirection: ���� ������ ���͸� ���� ������ ���ͷ� �ٲ��ִ� �Լ�

        // �߷� ����
        _yVelocity += GRAVITY * Time.deltaTime;
        dir.y = _yVelocity;

        // �÷��̾� ���� ����
        if (Input.GetButtonDown("Jump") && _jumpCount < 2)
        {
            _yVelocity = JumpPower;
            _jumpCount++;
            //dir.y = _yVelocity;
        }


        // Shift Ű ������ -> �÷��̾� �޸��� ����
        if ((Input.GetKey(KeyCode.RightShift) || (Input.GetKey(KeyCode.LeftShift))) && _jumpCount == 0)
        {
            _chracterController.Move(dir * RunSpeed * Time.deltaTime);
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
            _chracterController.Move(dir * MoveSpeed * Time.deltaTime);
        }

        // EŰ ������ -> �÷��̾� ������ ����
        if (Input.GetKey(KeyCode.Q) && _jumpCount == 0)
        {
            _chracterController.Move(dir * RollSpeed * Time.deltaTime);
            _isRolling = true;
        }

        // ��Ÿ�� ����

    }
}
