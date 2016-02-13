using UnityEngine;
using System.Collections;
using System;
using UniRx;

public class CharacterManager : MonoBehaviour, IWorldUtilitiesUser, IMapInstanceUtilitiesUser
{
    public Transform characterPrefab;

    public Func<Character,Coord, bool> MoveChecker { get; set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    private Transform characterObj;
    private Character character;
    private World world;

    public void Spawn(Character character, MapInstance mapToSpawn, World world)
    {
        this.world = world;
        world.ProvideWorldUtilities(this);

        mapToSpawn.ProvideMapInstanceUtilities(this);

        characterObj = Instantiate(characterPrefab,
                                   Vector3.zero,
                                   characterPrefab.rotation) as Transform;
        characterObj.SetParent(transform);

        character.Location
                 .Subscribe(coord => 
                 {
                     StopCoroutine("SequenceMoveTransform");
                     StartCoroutine("SequenceMoveTransform", coord);
                 });

        this.character = character;
    }

    private float timeToFinishMove = 0.1f;
    // Process Move Object in the Game Scene
    IEnumerator SequenceMoveTransform(Coord coord)
    {
        var destination = CoordToWorldPositionConverter(coord.x, coord.y);

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
