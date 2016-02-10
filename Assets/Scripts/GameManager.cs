﻿using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class GameManager : MonoBehaviour
{
    public MapInstance mapPrefab;
    public CharacterManager characterManagerPrefab;

    [HideInInspector]
    public ReactiveProperty<CombatPhase> currentPhase = new ReactiveProperty<CombatPhase>();

    private ReactiveProperty<PlayerCommand> InputCommand = new ReactiveProperty<PlayerCommand>();

    private MapInstance map;

    // Manage Enemies
    private Enemies enemies;

    private GameObject gameObjectHolder;
    private CharacterManager currentCharacter;
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

        StartCoroutine(SequenceSetupPlayers());
        StartCoroutine(SequenceSetupEnemies());

        // create InputManager
        var inputManager = new GameObject("InputManager");
        inputManager.AddComponent<InputManager>().gameManager = this;
        yield return null;

        yield return StartCoroutine(SequenceCharacterMove(currentCharacter));
        currentPhase.Value = CombatPhase.EnemyMove;

    }

    IEnumerator SequenceSetupPlayers()
    {
        // create characters
        var character = new Character();
        currentCharacter = SpawnCharacterToWorld(character);

        yield break;
    }

    IEnumerator SequenceSetupEnemies()
    {
        enemies = new Enemies();

        var character = new Character();
        enemies.AddCharacterAsEnemy(character);
        SpawnCharacterToWorld(character);
        character.Location.Value = new Coord(3, 3);

        yield break;
    }

    IEnumerator SequenceCharacterMove(CharacterManager moveCharacter)
    {
        currentPhase.Value = CombatPhase.PlayerMove;
        var subscription = SubscribeInputCommand(moveCharacter);

        yield return StartCoroutine(moveCharacter.SequenceMoveInput());

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

    CharacterManager SpawnCharacterToWorld(Character character)
    {
        var newCharacter = Instantiate(characterManagerPrefab, Vector3.zero, Quaternion.identity) as CharacterManager;
        newCharacter.Spawn(character, map);
        newCharacter.transform.SetParent(gameObjectHolder.transform);

        return newCharacter;
    }
}
