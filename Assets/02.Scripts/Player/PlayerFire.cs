using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFire : MonoBehaviour
{
    // 필요 속성
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
        HandleCursorToggle();
        _bulletTimer += Time.deltaTime;
        ThrowBullet();
        ThrowBomb();
    }

    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            LockCursor();
        }
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
            {
                return bomb;
            }
        }
        return null;
    }

    private void ThrowBullet()
    {
        if (Input.GetMouseButton(0) && _bulletTimer >= bulletCooldown)
        {
            if (bulletCount.TryUseBullet())
            {
                Ray ray = new Ray(FirePosition.transform.position, Camera.main.transform.forward);
                RaycastHit hitInfo = new RaycastHit();

                bool isHit = Physics.Raycast(ray, out hitInfo);
                if (isHit)
                {
                    BulletEffect.transform.position = hitInfo.point;
                    BulletEffect.transform.forward = hitInfo.normal;
                    BulletEffect.Play();

                    if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                    {
                        Enemy enemy = hitInfo.collider.GetComponent<Enemy>();
                        Damage damage = new Damage();
                        damage.Value = 10;
                        damage.From = this.gameObject;
                        enemy.TakeDamage(damage);
                        Debug.Log($"Enemy: {damage}");
                    }
                    if (hitInfo.collider.gameObject.CompareTag("Barrel"))
                    {
                        Barrel barrel = hitInfo.collider.GetComponent<Barrel>();
                        Damage damage = new Damage();
                        damage.Value = 1;
                        damage.From = this.gameObject;
                        barrel.TakeDamage(damage);
                        Debug.Log($"Barrel: {damage}");
                    }
                }
                _bulletTimer = 0f;
            }
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
