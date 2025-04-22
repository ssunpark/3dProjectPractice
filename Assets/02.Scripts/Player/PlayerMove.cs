using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 목표: wasd를 누르면 캐릭터를 카메라 방향에 맞게 이동시키고 싶다.
    [Header("속도")]
    public float MoveSpeed = 7f;
    public float RunSpeed = 12f; // 플레이어 뛸 때 속도
    public float RollSpeed = 20f;

    [Header("점프")]
    public float JumpPower = 5f;
    private const float GRAVITY = -9.8f; // 중력
    private float _yVelocity = 0f; // 중력가속도
    public int _jumpCount = 0;

    [Header("벽타기")]
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
        // 캐릭터가 땅 위에 있다면 
        if (_chracterController.isGrounded) // 같은 의미로, if(_chracterController.collisionFlags == CollisionFlags.Below & CollisionFlags.Sides)
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
        // TransformDirection: 지역 공간의 벡터를 월드 공간의 벡터로 바꿔주는 함수

        // 중력 적용
        _yVelocity += GRAVITY * Time.deltaTime;
        dir.y = _yVelocity;

        // 플레이어 점프 구현
        if (Input.GetButtonDown("Jump") && _jumpCount < 2 && !_isWallSliding)
        {
            _yVelocity = JumpPower;
            _jumpCount++;
            dir.y = _yVelocity;
        }


        // Shift 키 누르면 -> 플레이어 달리기 구현
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

        // E키 누르면 -> 플레이어 구르기 구현
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
        // 1) 평면 이동 방향 재계산
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 flatDir = new Vector3(h, 0f, v).normalized;
        flatDir = Camera.main.transform.TransformDirection(flatDir);

        // 2) 벽 접촉 체크
        bool touchingWall = (_chracterController.collisionFlags & CollisionFlags.Sides) != 0;
        RaycastHit hit;
        bool rayHit = Physics.Raycast(
            origin: transform.position + Vector3.up * 1f,
            direction: flatDir,
            out hit,
            WallCheckDistance,
            WallLayerMask
        );

        // 3) 공중 + 하강 중 + 벽 접촉 시 슬라이드
        if (!_chracterController.isGrounded
            && (touchingWall || rayHit)
            && _yVelocity < 0f)
        {
            _isWallSliding = true;
            _wallNormal = rayHit ? hit.normal : -flatDir;
            _yVelocity = Mathf.Max(_yVelocity, -WallSlideSpeed);

            // 4) 슬라이드 중 점프 → 벽 점프
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
        // 1) 벽 슬라이드 상태 해제
        _forceFallFromWall = true;
        _isWallSliding = false;
        // 2) 즉시 자유 낙하하도록 수직 속도 리셋
        _yVelocity = 0f;
    }
}
