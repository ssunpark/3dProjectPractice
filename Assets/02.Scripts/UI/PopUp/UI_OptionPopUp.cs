public class UI_OptionPopUp : UI_PopUp
{
    public void OnClickContinueButton()
    {
        GameManager.Instance.Continue();
        gameObject.SetActive(false);
    }

    public void OnClickRetryButton()
    {
        gameObject.SetActive(false);
    }
    public void OnClickQuitButton()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        //Application.Quit();
    }
    public void OnClickCreditButton()
    {
        PopUpManager.Instance.Open(PopupType.UI_CreditPopUp);
    }

}
