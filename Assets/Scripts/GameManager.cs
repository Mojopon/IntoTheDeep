using UnityEngine;
using System.Collections;
using UniRx;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public MapInstance mapPrefab;
    public CharacterManager characterManagerPrefab;

    [HideInInspector]
    public ReactiveProperty<CombatPhase> currentPhase = new ReactiveProperty<CombatPhase>();

    private ReactiveProperty<PlayerCommand> InputCommand = new ReactiveProperty<PlayerCommand>();

    private MapInstance map;

    // Manage Characters In The World(Map)
    private WorldCharacters worldCharacters;
    private GameObject gameObjectHolder;

    private Dictionary<Character, CharacterManager> characters = new Dictionary<Character, CharacterManager>();
    void Start()
    {
        StartCoroutine(SequenceSetupGame());
    }


    void Update()
    {

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

        // Create World Characters Class To Manage All of Characters
        worldCharacters = new WorldCharacters();
        StartCoroutine(SequenceSetupPlayers());
        StartCoroutine(SequenceSetupEnemies());

        // create InputManager
        var inputManager = new GameObject("InputManager");
        inputManager.AddComponent<InputManager>().gameManager = this;
        yield return null;

        bool inCombat = true;

        // loop when in combat
        while (inCombat)
        {
            var nextCharacter = worldCharacters.GetNextCharacterToAction();
            if(nextCharacter.IsPlayer)
            {
                currentPhase.Value = CombatPhase.PlayerMove;
            }
            else
            {
                currentPhase.Value = CombatPhase.EnemyMove;
            }
            var characterManager = characters[nextCharacter];

            yield return StartCoroutine(SequenceCharacterMove(characterManager));
        }

    }

    IEnumerator SequenceSetupPlayers()
    {
        // create characters
        var character = new Character();
        worldCharacters.AddCharacter(character);
        SpawnCharacterToWorld(character);

        yield break;
    }

    IEnumerator SequenceSetupEnemies()
    {

        var character = new Character();
        worldCharacters.AddCharacterAsEnemy(character);
        SpawnCharacterToWorld(character);
        character.Location.Value = new Coord(3, 3);

        yield break;
    }

    IEnumerator SequenceCharacterMove(CharacterManager moveCharacter)
    {
        //wait one frame for input command to be reset or it would input same command submitted to previous character
        yield return null;

        var subscription = SubscribeInputCommand(moveCharacter);
        yield return StartCoroutine(moveCharacter.SequenceMove());
        subscription.Dispose();
    }

    IDisposable SubscribeInputCommand(IInputtable target)
    {
        return InputCommand.Subscribe(x => { target.Input(x); });
    }

    public void Input(PlayerCommand command)
    {
        InputCommand.Value = command;
    }

    void SpawnCharacterToWorld(Character character)
    {
        var newCharacter = Instantiate(characterManagerPrefab, Vector3.zero, Quaternion.identity) as CharacterManager;
        newCharacter.Spawn(character, map);
        newCharacter.transform.SetParent(gameObjectHolder.transform);

        characters.Add(character, newCharacter);
    }
}
