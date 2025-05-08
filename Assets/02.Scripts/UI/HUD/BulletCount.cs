using KINEMATION.FPSAnimationPack.Scripts.Player;
using KINEMATION.FPSAnimationPack.Scripts.Weapon;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulletCount : MonoBehaviour
{
    public TextMeshProUGUI BulletText;
    public TextMeshProUGUI BulletCoolTimeText;

    // ÃÑ ¹«±â°¡ ¹Ù²ñ¿¡ µû¶ó Åº¼ö UI ¾÷µ¥ÀÌÆ®
    private FPSWeapon _fpsWeapon;
    private FPSPlayer _fpsPlayer;

    private float _reloadTime = 2.0f;
    private int _bulletCurrentCount = 50;
    private int _bulletMaxCount = 50;

    private bool _isReloading = false;
    private Coroutine _reloadCoroutine;

    private void Start()
    {
        //_fpsPlayer = FindAnyObjectByType<FPSPlayer>();
        //_fpsWeapon = _fpsPlayer.GetActiveWeapon();
        UpdateUI();
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (_isReloading)
            {
                CancelReload();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_isReloading && _bulletCurrentCount < _bulletMaxCount)
            {
                _reloadCoroutine = StartCoroutine(ReloadRoutine());
            }
        }
    }

    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        float timer = 0f;

        while (timer < _reloadTime)
        {
            if (!_isReloading) yield break;

            timer += Time.deltaTime;
            BulletCoolTimeText.text = $"{(_reloadTime - timer):F1}s";
            yield return null;
        }

        _bulletCurrentCount = _bulletMaxCount;
        _isReloading = false;
        BulletCoolTimeText.text = "";
        UpdateUI();
    }

    private void CancelReload()
    {
        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
            _reloadCoroutine = null;
        }

        _isReloading = false;
        BulletCoolTimeText.text = "";
    }

    private void UpdateUI()
    {
        BulletText.text = $"{_bulletCurrentCount} / {_bulletMaxCount}";
    }

    public bool TryUseBullet()
    {
        if (_isReloading) return false;

        if (_bulletCurrentCount > 0)
        {
            _bulletCurrentCount--;
            UpdateUI();
            return true;
        }

        return false; // Åº¾à ¾øÀ½
    }

}
