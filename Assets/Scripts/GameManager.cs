using UnityEngine;
using System.Collections;
using UniRx;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float characterMoveSpeed = 0.1f;

    public MapInstance mapPrefab;
    public CharacterManager characterManagerPrefab;
    public GameObject menuObjects;

    public PathSelector pathSelectorPrefab;
    public SkillSelector skillSelectorPrefab;

    public ReactiveProperty<CombatPhase> CurrentPhase = new ReactiveProperty<CombatPhase>();
    public ReactiveProperty<PlayerCommand> PlayerInput = new ReactiveProperty<PlayerCommand>();

    public ReactiveProperty<Character> CurrentActor;

    // Spawn Maptiles and Manage Maps
    private MapInstance mapInstance;

    // Manage Characters In The World(Map)
    private World world;

    // Spawn Character Transforms to the Scene and Manage it(CharacterControllers)
    private CharacterManager characterManager;

    private GameObject gameObjectHolder;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return StartCoroutine(SequenceSetupGame());

        StartBattle();
    }

    public void Input(PlayerCommand command)
    {
        PlayerInput.Value = command;
    }

    //Input to This instead of subscribed input target when the Menu is opened
    private MenuManager openedMenu;
    public bool MenuIsOpened = false;
    private IDisposable menuInputSubscription;
    public void OpenMenu()
    {
        MenuIsOpened = true;
        menuObjects.SetActive(true);
        var menuManager = menuObjects.GetComponentInChildren<MenuManager>();
        openedMenu = menuManager;

        // we add skip operator otherwise it will submit menu command to the menu
        // by the frame we openned menu as well
        menuInputSubscription = PlayerInput.Skip(1)
                                           .Subscribe(x => { menuManager.Input(x); })
                                           .AddTo(gameObject);
    }

    public void CloseMenu()
    {
        openedMenu = null;

        menuInputSubscription.Dispose();
        menuInputSubscription = null;

        menuObjects.SetActive(false);
        MenuIsOpened = false;
    }

    IEnumerator SequenceSetupGame()
    {
        if(gameObjectHolder != null)
        {
            Destroy(gameObjectHolder);
            yield return null;
        }
        gameObjectHolder = new GameObject("GameObjectHolder");

        this.mapInstance = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity) as MapInstance;
        this.mapInstance.Generate();
        this.mapInstance.transform.SetParent(gameObjectHolder.transform);
        yield return null;

        // Create World Characters Class To Manage All of Characters
        var currentMap = mapInstance.GetCurrentMap();
        this.world = new World(currentMap);
        this.CurrentActor = world.CurrentActor;

        this.characterManager = Instantiate(characterManagerPrefab);
        this.characterManager.transform.SetParent(gameObjectHolder.transform);
        this.characterManager.Initialize(mapInstance, world);
        StartCoroutine(SequenceSetupPlayers());
        StartCoroutine(SequenceSetupEnemies());

        // create InputManager
        var inputManager = new GameObject("InputManager");
        inputManager.AddComponent<InputManager>().gameManager = this;
        yield return null;

        // subscribe input to open menu
        PlayerInput.Where(x => x == PlayerCommand.Menu)
                   .Subscribe(x =>
                   {
                       if(openedMenu == null)
                       {
                           OpenMenu();
                       }
                   });

        CurrentActor.Where(x => x != null)
                    .Subscribe(x => SubscribeForCurrentActor(x))
                    .AddTo(gameObject);
    }

    void StartBattle()
    {
        StartCoroutine(SequenceGoNextCharacterPhase());
    }

    CompositeDisposable subscriptionsForCurrentActor;
    private PathSelector pathSelector;

    void SubscribeForCurrentActor(Character chara)
    {
        if(subscriptionsForCurrentActor != null)
        {
            subscriptionsForCurrentActor.Dispose();
            subscriptionsForCurrentActor = null;
        }

        subscriptionsForCurrentActor = new CompositeDisposable();

        chara.CurrentPhase.Subscribe(x =>
                          {
                              switch(x)
                              {
                                  case Character.Phase.Move:
                                      StartCoroutine(SequenceCharacterMove(chara));
                                      break;
                                  case Character.Phase.CombatAction:
                                      StartCoroutine(SequenceCharacterAction(chara));
                                      break;
                                  case Character.Phase.Idle:
                                      StartCoroutine(SequenceGoNextCharacterPhase());
                                      break;
                              }
                          })
                          .AddTo(subscriptionsForCurrentActor);
    }

    IEnumerator SequenceSetupPlayers()
    {
        // create characters
        var character = Character.Create();
        world.AddCharacter(character);

        yield break;
    }

    IEnumerator SequenceSetupEnemies()
    {
        var character = Character.Create();
        world.AddCharacterAsEnemy(character, 3, 3);

        yield break;
    }

    IEnumerator SequenceCharacterMove(Character moveCharacter)
    {
        var pathSelector = Instantiate(pathSelectorPrefab) as PathSelector;
        pathSelector.Initialize(this, mapInstance, moveCharacter, world);
        yield return StartCoroutine(pathSelector.SequenceRouting());
    }

    IEnumerator SequenceCharacterAction(Character actCharacter)
    {
        var skillSelector = Instantiate(skillSelectorPrefab) as SkillSelector;
        skillSelector.Initialize(this, mapInstance, actCharacter, world);
        yield return StartCoroutine(skillSelector.SequenceSelectSkill());
    }

    IEnumerator SequenceGoNextCharacterPhase()
    {
        while(!characterManager.AllActionsDone)
        {
            yield return null;
        }

        world.GoNextCharacterPhase();
    }
}
