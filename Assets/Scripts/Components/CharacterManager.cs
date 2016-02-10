using UnityEngine;
using System.Collections;
using System;
using UniRx;

public class CharacterManager : MonoBehaviour, IMapInstanceUtilitiesUser, IInputtable
{
    public Transform characterPrefab;

    public Func<int, int, bool> MoveChecker { get; set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }

    private Transform characterObj;
    private Character character;

    private ReactiveProperty<PlayerCommand> InputCommand;

    void Awake()
    {
        InputCommand = new ReactiveProperty<PlayerCommand>();
    }


    public void Spawn(Character character, MapInstance mapToSpawn)
    {
        mapToSpawn.RegisterUtilityUser(this);

        characterObj = Instantiate(characterPrefab,
                                   Vector3.zero,
                                   characterPrefab.rotation) as Transform;
        characterObj.SetParent(transform);

        character.Location
                 .Subscribe(coord => 
                 {
                     StopAllCoroutines();
                     StartCoroutine(SequenceMove(character.X, character.Y));
                 });

        this.character = character;
    }

    public void Input(PlayerCommand command)
    {
        InputCommand.Value = command;
    }

    private int movePerTurns = 3;
    private int moved;
    public IEnumerator SequenceMoveInput()
    {
        moved = 3;

        var subscription = InputCommand.Subscribe(x =>
                                       {
                                           switch (x)
                                           {
                                               case PlayerCommand.Left:
                                               case PlayerCommand.Right:
                                               case PlayerCommand.Up:
                                               case PlayerCommand.Down:
                                                   {
                                                       if(character.Move(x.ToDirection(), MoveChecker))
                                                       {
                                                           moved--;
                                                       }
                                                   }
                                                   break;
                                           }
                                       });

        while(moved > 0)
        {
            yield return null;
        }

        subscription.Dispose();
    }

    private float timeToFinishMove = 0.5f;
    IEnumerator SequenceMove(int x, int y)
    {
        var destination = CoordToWorldPositionConverter(x, y);

        float speed = 1 / timeToFinishMove;

        float progress = 0;
        while(Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            progress += speed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, destination, progress);
            yield return null;
        }

        yield break;
    }
}
