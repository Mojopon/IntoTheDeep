using UnityEngine;
using System.Collections;
using UniRx;

public class PopupWindow : MonoBehaviour, IInputtable
{
    private static PopupWindow instance;

    public PopupWindowController popupWindowUI;

    void Awake()
    {
        instance = this;
    }

    public static IObservable<bool> PopupYesNoWindow(string message)
    {
        return instance.OpenWindow(message);
    }

    public IObservable<bool> OpenWindow(string message)
    {
        popupWindowUI.gameObject.SetActive(true);
        popupWindowUI.SetMessage(message);
        return Observable.FromCoroutine<bool>(observer => PopUpSelectOptions(observer));
    }

    IEnumerator PopUpSelectOptions(IObserver<bool> observer)
    {
        InputManager.Instance.Register(this);

        while(selected == null)
        {
            yield return null;
        }

        observer.OnNext(selected == 0 ? true : false);
        observer.OnCompleted();
    }


    private int? selected = null;
    private int currentSelect = 0;
    void Select()
    {
        selected = currentSelect;

        popupWindowUI.gameObject.SetActive(false);
        InputManager.Instance.Deregister(this);
    }

    public void Input(PlayerCommand command)
    {
        switch(command)
        {
            case PlayerCommand.Left:
                currentSelect = 0;
                popupWindowUI.OnSelect(0);
                break;
            case PlayerCommand.Right:
                currentSelect = 1;
                popupWindowUI.OnSelect(1);
                break;
            case PlayerCommand.Enter:
                Select();
                break;
        }
    }
}
