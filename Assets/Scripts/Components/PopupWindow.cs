using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class PopupWindow : MonoBehaviour
{
    private static PopupWindow instance;

    public PopupWindowController popupWindowUI;

    void Start()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;

        SubscribePlayerInput(InputManager.Popup).AddTo(gameObject);
    }

    IDisposable SubscribePlayerInput(IPlayerInput inputs)
    {
        var compositeDisposable = new CompositeDisposable();

        inputs.MoveDirectionObservable
              .Skip(1)
              .Where(x => x != Direction.None)
              .Subscribe(x => {
                  if(x == Direction.Left)
                  {
                      MoveLeft();
                  }
                  else if(x == Direction.Right)
                  {
                      MoveRight();
                  }
              })
              .AddTo(compositeDisposable);

        inputs.OnEnterButtonObservable
              .Skip(1)
              .Where(x => x)
              .Subscribe(x => Select())
              .AddTo(compositeDisposable);

        return compositeDisposable;
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
        InputManager.ToggleInput(PlayerInputTarget.PopupWindow);

        while(selected == null)
        {
            yield return null;
        }

        InputManager.ToggleInput(PlayerInputTarget.PopupWindow);

        observer.OnNext(selected == 0 ? true : false);
        observer.OnCompleted();

        selected = null;
    }


    private int? selected = null;
    private int currentSelect = 0;
    void Select()
    {
        selected = currentSelect;

        popupWindowUI.gameObject.SetActive(false);
    }

    void MoveLeft()
    {
        currentSelect = 0;
        popupWindowUI.OnSelect(0);
    }

    void MoveRight()
    {
        currentSelect = 1;
        popupWindowUI.OnSelect(1);
    }
}
