using TMPro;
using UnityEngine;

public class BombCount : MonoBehaviour
{
    public TextMeshProUGUI BombText;

    public void UpdateBombUI(int current, int max)
    {
        BombText.text = $"{max - current}";
    }
}