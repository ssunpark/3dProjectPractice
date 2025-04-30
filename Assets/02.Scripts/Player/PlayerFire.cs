using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFire : MonoBehaviour
{
    [Header("폭탄")]
    public Transform FirePosition;
    public float MinThrowPower = 10f;
    public float MaxThrowPower = 20f;
    public float MaxHoldTime = 0.2f;
    private float __holdTimer = 0f;
    private bool _isHolding = false;

    private int currentBombCount = 0;
    private int maxBombCount = 3;
    public BombCount _bomCount;

    [Header("총알")]
    public ParticleSystem BulletEffect;
    public float bulletCooldown = 0.2f;
    private float _bulletTimer = 0f;
    public BulletCount bulletCount;

    private void Start()
    {
        LockCursor();
        currentBombCount = 0;
        _bomCount?.UpdateBombUI(currentBombCount, maxBombCount);
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!GameManager.Instance.CanMove()) return;

        HandleCursorToggle();
        _bulletTimer += Time.deltaTime;
        ThrowBullet();
        ThrowBomb();
    }

    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UnlockCursor();
        else if (Input.GetKeyDown(KeyCode.Tab))
            LockCursor();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ThrowBullet()
    {
        if (Input.GetMouseButton(0) && _bulletTimer >= bulletCooldown)
        {
            if (!bulletCount.TryUseBullet())
            {
                Debug.Log("[총알] 총알 부족");
                return;
            }

            Ray ray = new Ray(FirePosition.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                BulletEffect.transform.position = hitInfo.point;
                BulletEffect.transform.forward = hitInfo.normal;
                BulletEffect.Play();

                if (hitInfo.collider.gameObject == this.gameObject)
                {
                    return;
                }
                if (hitInfo.collider.TryGetComponent<IDamagable>(out var damagable))
                {
                    Damage damage = new Damage
                    {
                        Value = 10,
                        From = this.gameObject
                    };
                    damagable.TakeDamage(damage);
                    Debug.Log($"[총알 명중] 대상: {hitInfo.collider.name}, 데미지: {damage.Value}");
                }
            }

            _bulletTimer = 0f;
        }
    }

    private void ThrowBomb()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (currentBombCount >= maxBombCount)
            {
                Debug.Log("[폭탄] 더 이상 폭탄을 던질 수 없습니다.");
                return;
            }

            _isHolding = true;
            __holdTimer = 0f;
        }
        else if (Input.GetMouseButton(1) && _isHolding)
        {
            __holdTimer = Mathf.Min(__holdTimer + Time.deltaTime, MaxHoldTime);
        }
        else if (Input.GetMouseButtonUp(1) && _isHolding)
        {
            float t = __holdTimer / MaxHoldTime;
            float throwForce = Mathf.Lerp(MinThrowPower, MaxThrowPower, t);

            GameObject bomb = BombPoolManager.Instance.GetBomb();
            if (bomb == null)
            {
                Debug.LogWarning("[폭탄] 폭탄 풀에 더 이상 사용 가능한 폭탄이 없습니다.");
                return;
            }

            bomb.transform.position = FirePosition.position;
            bomb.transform.rotation = Camera.main.transform.rotation;


            if (bomb.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
                rb.AddTorque(Vector3.one);
            }

            currentBombCount++;
            _bomCount?.UpdateBombUI(currentBombCount, maxBombCount);
            Debug.Log($"[폭탄 사용] 현재 수: {currentBombCount}/{maxBombCount}");

            _isHolding = false;
        }
    }
}
