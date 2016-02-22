using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private List<IInputtable> inputtables = new List<IInputtable>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (inputtables.Count <= 0) return;
        inputtables[0].Input(InputToCommand());
    }

    public void Register(IInputtable inputtable)
    {
        inputtables.Insert(0, inputtable);
    }

    public void Deregister(IInputtable inputtable)
    {
        inputtables.Remove(inputtable);
    }

    PlayerCommand InputToCommand()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            return PlayerCommand.Menu;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            return PlayerCommand.Enter;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            return PlayerCommand.Cancel;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            return PlayerCommand.Right;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return PlayerCommand.Left;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            return PlayerCommand.Up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            return PlayerCommand.Down;
        }

        return PlayerCommand.None;
    }
}
