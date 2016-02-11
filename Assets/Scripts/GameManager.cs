using UnityEngine;
using System.Collections;
using UniRx;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public MapInstance mapPrefab;
    public CharacterManager characterManagerPrefab;
    public GameObject menuObjects;

    [HideInInspector]
    public ReactiveProperty<CombatPhase> CurrentPhase = new ReactiveProperty<CombatPhase>();

    private ReactiveProperty<PlayerCommand> InputCommand = new ReactiveProperty<PlayerCommand>();

    private MapInstance map;

    // Manage Characters In The World(Map)
    private WorldCharacters worldCharacters;
    // Add Character to this Container When its Spawned to Find Character Manager for The Character
    private Dictionary<Character, CharacterManager> characters = new Dictionary<Character, CharacterManager>();

    private GameObject gameObjectHolder;

    void Start()
    {
        StartCoroutine(SequenceSetupGame());
    }

    public void Input(PlayerCommand command)
    {
        InputCommand.Value = command;
    }

    //Input to This instead of subscribed input target when the Menu is opened
    private MenuManager openedMenu;
    private ReactiveProperty<bool> MenuIsOpened = new ReactiveProperty<bool>(false);
    private IDisposable menuInputSubscription;
    public void OpenMenu()
    {
        menuObjects.SetActive(true);
        var menuManager = menuObjects.GetComponentInChildren<MenuManager>();
        openedMenu = menuManager;

        // we add skip operator otherwise it will submit menu command to the menu
        // by the frame we openned menu as well
        menuInputSubscription = InputCommand.Skip(1)
                                            .Subscribe(x => { menuManager.Input(x); })
                                            .AddTo(gameObject);
    }

    public void CloseMenu()
    {
        openedMenu = null;

        menuInputSubscription.Dispose();
        menuInputSubscription = null;

        menuObjects.SetActive(false);
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

        // subscribe input to open menu
        InputCommand.Where(x => x == PlayerCommand.Menu)
                    .Subscribe(x =>
                    {
                        if(openedMenu == null)
                        {
                            OpenMenu();
                        }
                    });

        bool inCombat = true;

        // loop when in combat
        while (inCombat)
        {
            var nextCharacter = worldCharacters.GetNextCharacterToAction();
            if(nextCharacter.IsPlayer)
            {
                CurrentPhase.Value = CombatPhase.PlayerMove;
            }
            else
            {
                CurrentPhase.Value = CombatPhase.EnemyMove;
            }
            var characterManager = characters[nextCharacter];

            yield return StartCoroutine(SequenceCharacterMove(characterManager));

            if (nextCharacter.IsPlayer)
            {
                CurrentPhase.Value = CombatPhase.PlayerAction;
            }
            else
            {
                CurrentPhase.Value = CombatPhase.EnemyAction;
            }
            yield return StartCoroutine(SequenceCharacterAction(characterManager));
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

        var subscription = SubscribeInputForTheTarget(moveCharacter);
        yield return StartCoroutine(moveCharacter.SequenceMove());
        subscription.Dispose();
    }

    IEnumerator SequenceCharacterAction(CharacterManager actCharacter)
    {
        //wait one frame for input command to be reset or it would input same command submitted to previous character
        yield return null;

        var subscription = SubscribeInputForTheTarget(actCharacter);
        yield return StartCoroutine(actCharacter.SequenceSelectNextCombatAction());
        subscription.Dispose();
    }

    IDisposable SubscribeInputForTheTarget(IInputtable target)
    {
        // subscribe to input command to the target.
        // this will be ignored when a menu is opened
        return InputCommand.Where(x => openedMenu == null)
                           .Subscribe(x => {
                                               target.Input(x);
                                           });
    }

    void SpawnCharacterToWorld(Character character)
    {
        var newCharacter = Instantiate(characterManagerPrefab, Vector3.zero, Quaternion.identity) as CharacterManager;
        newCharacter.Spawn(character, map);
        newCharacter.transform.SetParent(gameObjectHolder.transform);

        characters.Add(character, newCharacter);
    }
}
