using System.Collections;
using TMPro;
using UnityEngine;

public enum GameState
{
    Ready,
    Run,
    Over
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState = GameState.Ready;
    public TextMeshProUGUI ReadyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartCoroutine(GameStart());
    }

    private IEnumerator GameStart()
    {
        CurrentState = GameState.Ready;
        ReadyText.text = "Ready...";

        yield return new WaitForSeconds(2f);

        ReadyText.text = "Go!";
        CurrentState = GameState.Run;

        yield return new WaitForSeconds(0.5f);

        ReadyText.text = "";
    }

    public void GameOver()
    {
        CurrentState = GameState.Over;
        ReadyText.text = "Game Over";
    }

    public bool CanMove()
    {
        return CurrentState == GameState.Run;
    }
}
