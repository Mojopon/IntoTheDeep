using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UniRx;

[Serializable]
public class TextMenuUIComponent
{
    public Text text;
    public MenuCommand command;
}

public class MenuManager : MonoBehaviour
{
    public GameObject menuObjects;
    public TextMenuUIComponent[] textMenus;

    private int currentMenuNumber;
    private TextMenuUIComponent currentMenu;

    private GameManager gameManager;

    void Start()
    {
        InputManager.OnMenuButtonObservable
                    .DistinctUntilChanged()
                    .Where(x => x)
                    .Subscribe(x => Toggle())
                    .AddTo(gameObject);
    }

    void OnEnable()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        textMenus[currentMenuNumber].text.color = Color.red;
        currentMenu = textMenus[currentMenuNumber];

        menuObjects.SetActive(false);
    }

    void OnCursorMove()
    {
        if (currentMenu != null) currentMenu.text.color = Color.white;

        textMenus[currentMenuNumber].text.color = Color.red;
        currentMenu = textMenus[currentMenuNumber];
    }

    private bool isActive = false;
    private IDisposable subscription;
    public void Toggle()
    {
        isActive = !isActive;
        menuObjects.SetActive(isActive);

        InputManager.ToggleInput(PlayerInputTarget.Menu);

        if (isActive)
        {
            subscription = SubscribePlayerInput(InputManager.Menu);
        }
        else
        {
            subscription.Dispose();
        }
    }

    IDisposable SubscribePlayerInput(IPlayerInput inputs)
    {
        var compositeDisposable = new CompositeDisposable();

        inputs.MoveDirectionObservable
              .Skip(1)
              .Where(x => x != Direction.None)
              .Subscribe(x => {
                  if(x == Direction.Up)
                  {
                      MoveUp();
                  }
                  else if(x == Direction.Down)
                  {
                      MoveDown();
                  }
              })
              .AddTo(compositeDisposable);

        inputs.OnEnterButtonObservable
              .Skip(1)
              .Where(x => x)
              .Subscribe(x => Submit())
              .AddTo(compositeDisposable);

        inputs.OnCancelButtonObservable
              .Skip(1)
              .Where(x => x)
              .Subscribe(x => Toggle())
              .AddTo(compositeDisposable);

        return compositeDisposable;
    }

    public void MoveUp()
    {
        currentMenuNumber--;
        if (currentMenuNumber < 0) currentMenuNumber = 0;

        OnCursorMove();
    }

    public void MoveDown()
    {
        currentMenuNumber++;
        if (currentMenuNumber >= textMenus.Length) currentMenuNumber = textMenus.Length-1;

        OnCursorMove();
    }

    public void Submit()
    {
        var command = currentMenu.command;

        switch(command)
        {
            case MenuCommand.Resume:
                Toggle();
                return;
            case MenuCommand.ExitGame:
                Debug.Log("Exitting game");
                Application.Quit();
                return;
        }
    }
}
