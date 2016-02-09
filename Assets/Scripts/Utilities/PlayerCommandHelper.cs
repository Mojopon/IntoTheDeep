using UnityEngine;
using System.Collections;

public static class PlayerCommandHelper
{
    public static Direction ToDirection(this PlayerCommand command)
    {
        switch(command)
        {
            case PlayerCommand.None:
            default:
                return Direction.None;
            case PlayerCommand.Left:
                return Direction.Left;
            case PlayerCommand.Right:
                return Direction.Right;
            case PlayerCommand.Up:
                return Direction.Up;
            case PlayerCommand.Down:
                return Direction.Down;
        }
    }
}
