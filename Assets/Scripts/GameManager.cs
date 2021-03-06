﻿using UnityEngine;
using System.Collections;
using UniRx;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public MapInstance mapPrefab;
    public CharacterTransformManager characterManagerPrefab;

    public PathSelector pathSelectorPrefab;
    public SkillSelector skillSelectorPrefab;

    public SingletonSpawner singletonSpawnerPrefab;

    public ReactiveProperty<CombatPhase> CurrentPhase = new ReactiveProperty<CombatPhase>();
    public ReactiveProperty<PlayerCommand> PlayerInput = new ReactiveProperty<PlayerCommand>();

    // gonna reference world's CurrentActor property later on
    public ReactiveProperty<Character> CurrentActor;

    [HideInInspector]
    public MenuManager menuManager;

    private Map[] maps;

    private TransitionWorld transition;

    // Spawn Maptiles and Manage Maps
    private MapInstance mapInstance;

    // Manage Characters In The World(Map)
    private World world;

    // Spawn Character Transforms to the Scene and Manage it(CharacterControllers)
    private CharacterTransformManager characterManager;

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
        yield return StartCoroutine(SequenceSetupNextWorld());
    }

    IEnumerator SequenceSetupGame()
    {
        Instantiate(singletonSpawnerPrefab);

        yield return null;

        this.maps = DungeonLoader.GetMapsForTheDungeon(DungeonTitle.Beginning);

        var playerOneData = new CharacterDataTable("PlayerOne", new Attributes());
        var playerTwoData = new CharacterDataTable("PlayerTwo", new Attributes());
        var playerThreeData = new CharacterDataTable("PlayerThree", new Attributes());
        var playerFourData = new CharacterDataTable("PlayerFour", new Attributes());

        var enemyOneData = new CharacterDataTable("EnemyOne", new Attributes());
        var enemyTwoData = new CharacterDataTable("EnemyTwo", new Attributes());
        var enemyThreeData = new CharacterDataTable("EnemyThree", new Attributes());

        this.transition = new TransitionWorld(maps);
        transition.AddPlayer(playerOneData);
        transition.AddPlayer(playerTwoData);
        transition.AddPlayer(playerThreeData);
        transition.AddPlayer(playerFourData);

        transition.AddEnemy(enemyOneData);
        transition.AddEnemy(enemyTwoData);
        transition.AddEnemy(enemyThreeData);


        // wait for input manager to finish awake method
        yield return null;
    }
    
    IEnumerator SequenceSetupNextWorld()
    {
        if (gameObjectHolder != null)
        {
            Destroy(gameObjectHolder);
            yield return null;
        }
        gameObjectHolder = new GameObject("GameObjectHolder");

        this.world = transition.GoNext();
        this.CurrentActor = world.CurrentActor;

        this.mapInstance = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity) as MapInstance;
        this.mapInstance.Generate(world.map);
        this.mapInstance.transform.SetParent(gameObjectHolder.transform);

        this.characterManager = Instantiate(characterManagerPrefab);
        this.characterManager.transform.SetParent(gameObjectHolder.transform);
        this.characterManager.Initialize(mapInstance, world);

        CurrentActor.Where(x => x != null)
                    .Subscribe(x => SubscribeForCurrentActor(x))
                    .AddTo(gameObjectHolder);

        StartBattle();
        yield break;
    }

    IEnumerator SequenceBackToPreparationScene()
    {
        SceneManager.LoadScene("Preparation");
        yield break;
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

        subscriptionsForCurrentActor = new CompositeDisposable().AddTo(gameObjectHolder);

        chara.CurrentPhase.Subscribe(x =>
                          {
                              switch(x)
                              {
                                  case Character.Phase.Move:
                                      StartCoroutine(SequenceCharacterMove(chara));
                                      break;
                                  case Character.Phase.Combat:
                                      StartCoroutine(SequenceCharacterAction(chara));
                                      break;
                                  case Character.Phase.TurnEnd:
                                      StartCoroutine(SequenceGoNextCharacterPhase());
                                      break;
                              }
                          })
                          .AddTo(subscriptionsForCurrentActor);
    }

    IEnumerator SequenceCharacterMove(Character moveCharacter)
    {
        yield return null;
        var pathSelector = Instantiate(pathSelectorPrefab) as PathSelector;
        pathSelector.Initialize(this, mapInstance, moveCharacter, world);
        yield return StartCoroutine(pathSelector.SequenceRouting());
    }

    IEnumerator SequenceCharacterAction(Character actCharacter)
    {
        // need to wait for one frame for models to finish all these stuffs
        yield return null;
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

        if(CurrentActor.Value != null && CurrentActor.Value.IsPlayer && CurrentActor.Value.IsOnExit)
        {
            bool goNextFloor = false;

            var modalPanelDetails = new ModalPanel.ModalPanelDetails()
            {
                title = "Confirmation",
                question = "You found a stairs.\nGo next floor?",
                button1Details = new ModalPanel.EventButtonDetails()
                {
                    buttonTitle = "Yes",
                    action = () => { goNextFloor = true; },
                },
                button2Details = new ModalPanel.EventButtonDetails()
                {
                    buttonTitle = "No",
                    action = () => { goNextFloor = false; },
                },
            };

            yield return ModalPanel.Instance.ChoiceAsObservable(modalPanelDetails).StartAsCoroutine();

            yield return null;

            if (goNextFloor)
            {
                goNextFloor = false;

                // go next map or go back to preparation scene when theres no map(world) left
                if (transition.HasNext())
                {
                    StartCoroutine(SequenceSetupNextWorld());
                }
                else
                {
                    StartCoroutine(SequenceBackToPreparationScene());
                }
                yield break;
            }
        }

        world.GoNextCharacterPhase();
    }
}
