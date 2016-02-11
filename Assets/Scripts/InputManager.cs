using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    [HideInInspector]
    public GameManager gameManager;

	void Update ()
    {
	    if(gameManager != null)
        {
            gameManager.Input(InputToCommand());
        }
	}

    PlayerCommand InputToCommand()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            return PlayerCommand.Menu;
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
