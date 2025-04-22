using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // ��ǥ: wasd�� ������ ĳ���͸� ī�޶� ���⿡ �°� �̵���Ű�� �ʹ�.
    [Header("�ӵ�")]
    public float MoveSpeed = 7f;
    public float RunSpeed = 12f; // �÷��̾� �� �� �ӵ�
    public float RollSpeed = 20f;

    [Header("����")]
    public float JumpPower = 5f;
    private const float GRAVITY = -9.8f; // �߷�
    private float _yVelocity = 0f; // �߷°��ӵ�
    public int _jumpCount = 0;

    [Header("��Ÿ��")]
    public float WallSlideSpeed;
    public Vector2 WallJumpForce = new Vector2(5f, 8f);
    public LayerMask WallLayerMask;
    public float WallCheckDistance = 0.7f;

    public bool _isRunning = false;
    public bool _isRolling = false;
    public bool _isWallSliding = false;
    private bool _forceFallFromWall = false;

    private CharacterController _chracterController;
    private Vector3 _wallNormal;

    private void Awake()
    {
        _chracterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        IfGrounded();
        WallSlide();
        PlayerMoving();

    }

    private void IfGrounded()
    {
        // ĳ���Ͱ� �� ���� �ִٸ� 
        if (_chracterController.isGrounded) // ���� �ǹ̷�, if(_chracterController.collisionFlags == CollisionFlags.Below & CollisionFlags.Sides)
        {
            _isRunning = false;
            _isRolling = false;
            _jumpCount = 0;
            _isWallSliding = false;
            _forceFallFromWall = false;
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
        if (Input.GetButtonDown("Jump") && _jumpCount < 2 && !_isWallSliding)
        {
            _yVelocity = JumpPower;
            _jumpCount++;
            dir.y = _yVelocity;
        }


        // Shift Ű ������ -> �÷��̾� �޸��� ����
        if (!_isWallSliding && (Input.GetKey(KeyCode.RightShift) || (Input.GetKey(KeyCode.LeftShift))) && _jumpCount == 0)
        {
            _chracterController.Move(dir * RunSpeed * Time.deltaTime);
            _isRunning = true;
        }
        else
        {
            _isRunning = false;
            if (!_isWallSliding)
            {
                _chracterController.Move(dir * MoveSpeed * Time.deltaTime);
            }
        }

        // EŰ ������ -> �÷��̾� ������ ����
        if (!_isWallSliding && Input.GetKey(KeyCode.Q) && _jumpCount == 0)
        {
            _chracterController.Move(dir * RollSpeed * Time.deltaTime);
            _isRolling = true;
        }
    }

    private void WallSlide()
    {
        if (_forceFallFromWall)
        {
            _isWallSliding = false;
            return;
        }
        // 1) ��� �̵� ���� ����
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 flatDir = new Vector3(h, 0f, v).normalized;
        flatDir = Camera.main.transform.TransformDirection(flatDir);

        // 2) �� ���� üũ
        bool touchingWall = (_chracterController.collisionFlags & CollisionFlags.Sides) != 0;
        RaycastHit hit;
        bool rayHit = Physics.Raycast(
            origin: transform.position + Vector3.up * 1f,
            direction: flatDir,
            out hit,
            WallCheckDistance,
            WallLayerMask
        );

        // 3) ���� + �ϰ� �� + �� ���� �� �����̵�
        if (!_chracterController.isGrounded
            && (touchingWall || rayHit)
            && _yVelocity < 0f)
        {
            _isWallSliding = true;
            _wallNormal = rayHit ? hit.normal : -flatDir;
            _yVelocity = Mathf.Max(_yVelocity, -WallSlideSpeed);

            // 4) �����̵� �� ���� �� �� ����
            if (Input.GetButtonDown("Jump"))
            {
                _yVelocity = WallJumpForce.y;
                Vector3 pushOff = _wallNormal * WallJumpForce.x;
                _chracterController.Move(pushOff * Time.deltaTime);

                _isWallSliding = false;
                _jumpCount = 1;
            }
        }
        else
        {
            _isWallSliding = false;
        }
    }

    public void ForceFallFromWall()
    {
        // 1) �� �����̵� ���� ����
        _forceFallFromWall = true;
        _isWallSliding = false;
        // 2) ��� ���� �����ϵ��� ���� �ӵ� ����
        _yVelocity = 0f;
    }
}
