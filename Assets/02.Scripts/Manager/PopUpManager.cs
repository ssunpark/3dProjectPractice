using System;
using System.Collections.Generic;
using UnityEngine;

public enum PopupType
{
    UI_OptionPopUp,
    UI_CreditPopUp
}

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [Header("팝업 UI 참조")]
    public List<UI_PopUp> Popups;

    private List<UI_PopUp> _openedPopups = new List<UI_PopUp>(); // null은 아니지만 비어있는 리스트

    private void Awake()
    {
        //if (Instance != null)
        //{
        //    Destroy(this.gameObject);
        //}
        //DontDestroyOnLoad(this.gameObject);

        Instance = this;
    }

    public void Open(PopupType type, Action closeCallback = null)
    {
        Open(type.ToString(), closeCallback);
    }

    public void Open(string popupName, Action closeCallback)
    {
        foreach (UI_PopUp popup in Popups)
        {
            if (popup.gameObject.name == popupName)
            {
                popup.Open(closeCallback);
                // 팝업을 열 때마다 닫는다
                _openedPopups.Add(popup);
                break;
            }
        }
    }
    //public void Close(string popupName)
    //{
    //    foreach (UI_PopUp popup in Popups)
    //    {
    //        if (popup.gameObject.name == popupName)
    //        {
    //            popup.Close();
    //            break;
    //        }
    //    }
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (_openedPopups.Count > 0)
            {
                Debug.Log("A");
                while (true)
                {
                    bool opend = _openedPopups[_openedPopups.Count - 1].isActiveAndEnabled;
                    _openedPopups[_openedPopups.Count - 1].Close();
                    _openedPopups.RemoveAt(_openedPopups.Count - 1);

                    // 열려있는 팝업을 닫았거나 || 더이상 닫을 팝업이 없으면 탈출!
                    if (opend || _openedPopups.Count == 0)
                    {
                        break;
                    }
                }

            }
            else
            {
                Debug.Log("B");
                GameManager.Instance.Pause();
            }
        }
    }
}
