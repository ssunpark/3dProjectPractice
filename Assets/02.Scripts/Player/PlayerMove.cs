using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 목표: wasd를 누르면 캐릭터를 카메라 방향에 맞게 이동시키고 싶다.
    // 필요 속성:
    // - 이동 속도
    [Header("속도")]
    public float MoveSpeed = 7f;
    public float RunSpeed = 12f; // 플레이어 뛸 때 속도
    public float RollSpeed = 20f;

    [Header("점프")]
    public float JumpPower = 5f;
    private const float GRAVITY = -9.8f; // 중력

    private float _yVelocity = 0f; // 중력가속도
    public bool _isRunning = false;
    public bool _isRolling = false;
    public int _jumpCount = 0;

    private CharacterController _chracterController;

    private void Awake()
    {
        _chracterController = GetComponent<CharacterController>();
    }

    // 구현 순서:
    // 1. 키보드 입력을 받는다.
    // 2. 입력으로부터 방향을 설정한다.
    // 3. 방향에 따라 플레이어가 이동한다.

    private void Update()
    {
        InitializeState();
        PlayerMoving();
    }

    private void InitializeState()
    {
        // 캐릭터가 땅 위에 있다면 
        if (_chracterController.isGrounded) // 같은 의미로, if(_chracterController.collisionFlags == CollisionFlags.Below & CollisionFlags.Sides)
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
        // TransformDirection: 지역 공간의 벡터를 월드 공간의 벡터로 바꿔주는 함수

        // 중력 적용
        _yVelocity += GRAVITY * Time.deltaTime;
        dir.y = _yVelocity;

        // 플레이어 점프 구현
        if (Input.GetButtonDown("Jump") && _jumpCount < 2)
        {
            _yVelocity = JumpPower;
            _jumpCount++;
            //dir.y = _yVelocity;
        }


        // Shift 키 누르면 -> 플레이어 달리기 구현
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

        // E키 누르면 -> 플레이어 구르기 구현
        if (Input.GetKey(KeyCode.Q) && _jumpCount == 0)
        {
            _chracterController.Move(dir * RollSpeed * Time.deltaTime);
            _isRolling = true;
        }

        // 벽타기 구현

    }
}
