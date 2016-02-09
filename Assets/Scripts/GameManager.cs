using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class GameManager : MonoBehaviour
{
    public MapInstance map;
    public CharacterManager characterManagerPrefab;

    private ReactiveProperty<PlayerCommand> InputCommand = new ReactiveProperty<PlayerCommand>();
    private IDisposable InputSubscription;

    private CharacterManager characterManager;

    private bool receivingInput;

    void Start()
    {
        StartCoroutine(SequenceSetupGame());
    }

    IEnumerator SequenceSetupGame()
    {
        // wait for one frame for other scripts to finish start method
        yield return null;

        // create characters
        var character = new Character();
        SpawnCharacterToWorld(character);

        // create InputManager
        var inputManager = new GameObject("InputManager");
        inputManager.AddComponent<InputManager>().gameManager = this;
        SubscribeInputCommand();

        receivingInput = true;
    }

    IEnumerator SequenceCharacterAction(PlayerCommand command)
    {
        receivingInput = false;

        characterManager.Input(command.ToDirection());

        yield return new WaitForSeconds(0.5f);

        receivingInput = true;
    }

    void SubscribeInputCommand()
    {
        if(InputSubscription != null)
        {
            InputSubscription.Dispose();
        }

        InputSubscription = InputCommand.Skip(1)
                                        .Where(x => receivingInput && x != PlayerCommand.None)
                                        .Subscribe(x => StartCoroutine(SequenceCharacterAction(x)));
    }

    public void Input(PlayerCommand command)
    {
        InputCommand.Value = command;
    }

    void SpawnCharacterToWorld(Character character)
    {
        characterManager = Instantiate(characterManagerPrefab, Vector3.zero, Quaternion.identity) as CharacterManager;
        map.RegisterUtilityUser(characterManager);
        characterManager.Spawn(character);
    }
}
