using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class GameManager : MonoBehaviour
{
    public MapInstance mapPrefab;
    public CharacterManager characterManagerPrefab;

    private ReactiveProperty<PlayerCommand> InputCommand = new ReactiveProperty<PlayerCommand>();
    private IDisposable InputSubscription;

    private MapInstance map;
    private CharacterManager currentCharacter;

    private GameObject gameObjectHolder;
    void Start()
    {
        StartCoroutine(SequenceSetupGame());
    }

    IEnumerator SequenceSetupGame()
    {
        if(gameObjectHolder != null)
        {
            Destroy(gameObjectHolder);
            yield return null;
        }
        gameObjectHolder = new GameObject("GameObjectHolder");

        map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity) as MapInstance;
        map.Generate();
        map.transform.SetParent(gameObjectHolder.transform);
        yield return null;

        // create characters
        var character = new Character();
        SpawnCharacterToWorld(character);

        // create InputManager
        var inputManager = new GameObject("InputManager");
        inputManager.AddComponent<InputManager>().gameManager = this;
        SubscribeInputCommand();

    }

    IEnumerator SequenceCharacterAction(PlayerCommand command)
    {
        yield return StartCoroutine(currentCharacter.SequenceInput(command));
    }

    void SubscribeInputCommand()
    {
        if(InputSubscription != null)
        {
            InputSubscription.Dispose();
        }

        InputSubscription = InputCommand.Where(x => x != PlayerCommand.None)
                                        .Subscribe(x => StartCoroutine(SequenceCharacterAction(x)));
    }

    public void Input(PlayerCommand command)
    {
        InputCommand.Value = command;
    }

    void SpawnCharacterToWorld(Character character)
    {
        currentCharacter = Instantiate(characterManagerPrefab, Vector3.zero, Quaternion.identity) as CharacterManager;
        currentCharacter.Spawn(character, map);
        currentCharacter.transform.SetParent(gameObjectHolder.transform);
    }
}
