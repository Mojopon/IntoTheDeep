using UnityEngine;
using System.Collections;
using System;
using UniRx;

public class CharacterManager : MonoBehaviour, IMapInstanceUtilitiesUser
{
    public Transform characterPrefab;

    public Func<int, int, bool> MoveChecker { get; set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }

    private Transform characterObj;
    private Character character;

    public void Spawn(Character character)
    {
        characterObj = Instantiate(characterPrefab,
                                   Vector3.zero,
                                   characterPrefab.rotation) as Transform;
        characterObj.SetParent(transform);

        character.Location
                 .Subscribe(coord => Move(coord));

        this.character = character;
    }

    public void Input(Direction direction)
    {
        character.Move(direction, MoveChecker);
    }

    public void Move(Coord coord)
    {
        StartCoroutine(SequenceMove(CoordToWorldPositionConverter(coord.x, coord.y)));
    }

    private float timeToFinishMove = 0.5f;
    IEnumerator SequenceMove(Vector3 destination)
    {
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
