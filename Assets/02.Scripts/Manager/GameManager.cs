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
    private static GameManager instance;

    public TextMeshProUGUI ReadyText;

    public GameState CurrentState = GameState.Ready;

    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = new GameManager();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance = null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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

    private void Update()
    {
        switch (CurrentState)
        {
            case GameState.Ready:

                break;

            case GameState.Run:

                break;

            case GameState.Over:

                break;

        }
    }

    IEnumerator GameStart()
    {
        CurrentState = GameState.Ready;
        ReadyText.text = "Ready...";

        yield return new WaitForSeconds(2f);

        ReadyText.text = "Go!";
        CurrentState = GameState.Run;

        yield return new WaitForSeconds(0.5f);
        ReadyText.text = "";
    }
}
