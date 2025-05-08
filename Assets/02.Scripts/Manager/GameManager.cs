using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Ready,
    Run,
    Pause,
    Over
}

public class GameManager : MonoBehaviour
{
    //public static GameManager Instance { get; private set; }
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private GameState CurrentState = GameState.Ready;
    public GameState GameState => CurrentState;
    public TextMeshProUGUI ReadyText;

    private void Awake()
    {
        //if (_instance != null)
        //{
        //    Destroy(this.gameObject);
        //}
        //DontDestroyOnLoad(this.gameObject);

        _instance = this;
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

    public void Continue()
    {
        CurrentState = GameState.Run;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReStart()
    {
        CurrentState = GameState.Run;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void Pause()
    {
        CurrentState = GameState.Pause;
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;

        PopUpManager.Instance.Open(PopupType.UI_OptionPopUp, closeCallback: Continue);
    }
}
