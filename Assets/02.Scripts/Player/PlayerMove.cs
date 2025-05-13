using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("속도")]
    public float MoveSpeed = 10f;
    public float RunSpeed = 15f;
    public float RollSpeed = 20f;

    [Header("점프")]
    public float JumpPower = 5f;
    private const float GRAVITY = -9.8f;
    private float _yVelocity = 0f;
    public int _jumpCount = 0;

    [Header("벽타기")]
    public float WallSlideSpeed;
    public float WallCheckDistance = 0.7f;
    public Vector2 WallJumpForce = new Vector2(5f, 8f);
    private Vector3 _wallNormal;
    public LayerMask WallLayerMask;

    [Header("상태 체크")]
    public bool _isRunning = false;
    public bool _isRolling = false;
    public bool _isWallSliding = false;
    private bool _forceFallFromWall = false;

    private CharacterController _chracterController;
    private Animator _playerAnimator;

    private void Awake()
    {
        _chracterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        IfGrounded();
        WallSlide();
        PlayerMoving();
        //UpdateAnimation();
    }

    private void IfGrounded()
    {
        if (_chracterController.isGrounded)
        {
            _isRunning = false;
            _isRolling = false;
            _jumpCount = 0;
            _isWallSliding = false;
            _forceFallFromWall = false;
        }
    }

    public void InitTPSMode()
    {
        _playerAnimator = GetComponentInChildren<Animator>();
    }

    //private void UpdateAnimation()
    //{
    //    if (_playerAnimator == null || !_playerAnimator.isActiveAndEnabled || _playerAnimator.runtimeAnimatorController == null) return;

    //    float h = Input.GetAxisRaw("Horizontal");
    //    float v = Input.GetAxisRaw("Vertical");

    //    float moveAmount = new Vector2(h, v).magnitude;

    //    // 달릴 때만 속도를 더해줘
    //    float speed = _isRunning ? moveAmount + 0.5f : moveAmount;
    //    float clampedSpeed = Mathf.Clamp01(speed);

    //    Debug.Log($"[애니메이션] MoveSpeed: {clampedSpeed}");

    //    _playerAnimator.SetFloat("MoveSpeed", clampedSpeed);
    //}

    private void PlayerMoving()
    {
        if (!GameManager.Instance.CanMove())
        {
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;
        dir = Camera.main.transform.TransformDirection(dir);

        // 중력 적용
        _yVelocity += GRAVITY * Time.deltaTime;
        dir.y = _yVelocity;

        if (Input.GetButtonDown("Jump") && _jumpCount < 2 && !_isWallSliding)
        {
            _yVelocity = JumpPower;
            _jumpCount++;
            dir.y = _yVelocity;
        }


        // Shift 키 누르면 달리기
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

        // Q 키 누르면 롤
        if (!_isWallSliding && Input.GetKey(KeyCode.Q) && _jumpCount == 0)
        {
            _chracterController.Move(dir * RollSpeed * Time.deltaTime);
            _isRolling = true;
        }
        else
        {
            _isRolling = false;
        }
    }

    private void WallSlide()
    {
        if (_forceFallFromWall)
        {
            _isWallSliding = false;
            return;
        }

        // 1) 벽타기 체크
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 flatDir = new Vector3(h, 0f, v).normalized;
        flatDir = Camera.main.transform.TransformDirection(flatDir);

        // 2) 벽타기 체크
        bool touchingWall = (_chracterController.collisionFlags & CollisionFlags.Sides) != 0;
        RaycastHit hit;
        bool rayHit = Physics.Raycast(
            origin: transform.position + Vector3.up * 1f,
            direction: flatDir,
            out hit,
            WallCheckDistance,
            WallLayerMask
        );

        // 3) 땅에 있지 않고, 벽에 붙어있거나, 벽에 붙어있고, 중력이 작용하고 있으면
        if (!_chracterController.isGrounded
            && (touchingWall || rayHit)
            && _yVelocity < 0f)
        {
            _isWallSliding = true;
            _wallNormal = rayHit ? hit.normal : -flatDir;
            _yVelocity = Mathf.Max(_yVelocity, -WallSlideSpeed);

            // 4) 점프 키 누르면 벽타기 점프
            if (Input.GetButtonDown("Jump"))
            {
                _yVelocity = WallJumpForce.y;
                Vector3 pushOff = _wallNormal * WallJumpForce.x;
                _chracterController.Move(pushOff * Time.deltaTime);

                _isWallSliding = false;
                _jumpCount = 1;
                _playerAnimator.SetTrigger("Jump");
            }
        }
        else
        {
            _isWallSliding = false;
        }
    }

    public void ForceFallFromWall()
    {
        // 1) 벽타기 체크 해제
        _forceFallFromWall = true;
        _isWallSliding = false;
        // 2) 중력 적용 해제
        _yVelocity = 0f;
    }
}
