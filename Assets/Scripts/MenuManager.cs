using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[Serializable]
public class TextMenuUIComponent
{
    public Text text;
    public MenuCommand command;
}

public class MenuManager : MonoBehaviour, IInputtable
{
    public TextMenuUIComponent[] textMenus;

    private int currentMenuNumber;
    private TextMenuUIComponent currentMenu;

    private GameManager gameManager;
    
    void OnEnable()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        textMenus[currentMenuNumber].text.color = Color.red;
        currentMenu = textMenus[currentMenuNumber];
    }

    void OnCursorMove()
    {
        if (currentMenu != null) currentMenu.text.color = Color.white;

        textMenus[currentMenuNumber].text.color = Color.red;
        currentMenu = textMenus[currentMenuNumber];
    }

    void MoveUp()
    {
        currentMenuNumber--;
        if (currentMenuNumber < 0) currentMenuNumber = 0;

        OnCursorMove();
    }

    void MoveDown()
    {
        currentMenuNumber++;
        if (currentMenuNumber >= textMenus.Length) currentMenuNumber = textMenus.Length-1;

        OnCursorMove();
    }

    void RunCommand()
    {
        var command = currentMenu.command;

        switch(command)
        {
            case MenuCommand.Resume:
                gameManager.CloseMenu();
                return;
            case MenuCommand.ExitGame:
                Debug.Log("Exitting game");
                Application.Quit();
                return;
        }
    }

    public void Input(PlayerCommand command)
    {
        switch(command)
        {
            case PlayerCommand.Up:
                MoveUp();
                break;
            case PlayerCommand.Down:
                MoveDown();
                break;
            case PlayerCommand.Menu:
                RunCommand();
                break;
        }
    }
}
