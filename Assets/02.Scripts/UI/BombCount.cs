using TMPro;
using UnityEngine;

public class BombCount : MonoBehaviour
{
    public TextMeshProUGUI BombText;
    private int _bombCurrentCount = 3;
    private int _bombMaxCount = 3;

    private void Start()
    {
        _bombCurrentCount = _bombMaxCount;
    }

    private void Update()
    {
        BombState();
    }

    private void BombState()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _bombCurrentCount--;
            _bombCurrentCount = Mathf.Clamp(_bombCurrentCount, 0, _bombMaxCount);
        }
        BombText.text = $"{_bombCurrentCount}";
    }
}
