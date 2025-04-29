using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFire : MonoBehaviour
{
    [Header("폭탄")]
    public Transform FirePosition;
    public GameObject BombPrefab;
    public float MinThrowPower = 10f;
    public float MaxThrowPower = 20f;
    public float MaxHoldTime = 0.2f;
    private float __holdTimer = 0f;
    private bool _isHolding = false;
    public int PoolSize = 10;
    private List<GameObject> _bombPool;

    [Header("총알")]
    public ParticleSystem BulletEffect;
    public float bulletCooldown = 0.2f;
    private float _bulletTimer = 0f;
    public BulletCount bulletCount;

    private void Start()
    {
        LockCursor();
        InitializePool();
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

    private void InitializePool()
    {
        _bombPool = new List<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject bomb = Instantiate(BombPrefab);
            bomb.SetActive(false);
            _bombPool.Add(bomb);
        }
    }

    private GameObject GetBombFromPool()
    {
        foreach (var bomb in _bombPool)
        {
            if (!bomb.activeInHierarchy)
                return bomb;
        }
        return null;
    }

    private void ThrowBullet()
    {
        if (Input.GetMouseButton(0) && _bulletTimer >= bulletCooldown)
        {
            if (!bulletCount.TryUseBullet()) return;

            Ray ray = new Ray(FirePosition.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                BulletEffect.transform.position = hitInfo.point;
                BulletEffect.transform.forward = hitInfo.normal;
                BulletEffect.Play();

                if (hitInfo.collider.TryGetComponent<IDamagable>(out var damagable))
                {
                    Damage damage = new Damage
                    {
                        Value = 10,
                        From = this.gameObject
                    };
                    damagable.TakeDamage(damage);
                    Debug.Log($"[총알 명중] {hitInfo.collider.name}, Damage: {damage.Value}");
                }
            }

            _bulletTimer = 0f;
        }
    }

    private void ThrowBomb()
    {
        if (Input.GetMouseButtonDown(1))
        {
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

            GameObject bomb = GetBombFromPool();
            bomb.transform.position = FirePosition.position;
            bomb.transform.rotation = Camera.main.transform.rotation;
            bomb.SetActive(true);

            if (bomb.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
                rb.AddTorque(Vector3.one);
            }

            _isHolding = false;
        }
    }
}
