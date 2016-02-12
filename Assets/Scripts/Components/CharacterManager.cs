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
    private WorldCharacters worldCharacters;

    private ReactiveProperty<PlayerCommand> PlayerInput;

    void Awake()
    {
        PlayerInput = new ReactiveProperty<PlayerCommand>();
    }

    public void Spawn(Character character, MapInstance mapToSpawn, WorldCharacters worldCharacters)
    {
        mapToSpawn.RegisterUtilityUser(this);

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
        this.worldCharacters = worldCharacters;
    }

    public void Input(PlayerCommand command)
    {
        PlayerInput.Value = command;
    }

    private int movePerTurns = 3;
    private int moved;
    // Process Move Phase For the Character
    public IEnumerator SequenceMove()
    {

        if (!character.IsPlayer)
        {
            yield return StartCoroutine(SequenceCPUMove());
            yield break;
        }
        else {

            moved = 3;

            PlayerInput.Value = PlayerCommand.None;
            var subscription = PlayerInput.Subscribe(x =>
                                           {
                                               switch (x)
                                               {
                                                   case PlayerCommand.Left:
                                                   case PlayerCommand.Right:
                                                   case PlayerCommand.Up:
                                                   case PlayerCommand.Down:
                                                       {
                                                           if (character.CanMove(x.ToDirection(), MoveChecker))
                                                           {
                                                               moved--;
                                                           }
                                                       }
                                                       break;
                                               }
                                           });

            while (moved > 0)
            {
                // wait for player input to finish character move
                yield return null;
            }

            subscription.Dispose();
        }
    }

    public IEnumerator SequenceSelectNextCombatAction()
    {
        if(!character.IsPlayer)
        {
            yield break;
        }
        else
        {
            Skill selectedSkill = null;
            SkillMenu.Current.DisplaySkills(character.GetSkills());

            CompositeDisposable combatActionSubscriptions = new CompositeDisposable();

            PlayerInput.Skip(1)
                       .Subscribe(x =>
                       {
                           SkillMenu.Current.Input(x);
                       })
            .AddTo(combatActionSubscriptions);

            var cancel = SkillMenu.Current.SelectedSkill()
                                          .Subscribe(x => selectedSkill = x);

            while (selectedSkill == null)
            {
                yield return null;
            }

            Debug.Log("Used " + selectedSkill.name);

            combatActionSubscriptions.Dispose();
        }
    }

    private IEnumerator SequenceCPUMove()
    {
        for(int i = 0; i < movePerTurns; i++)
        {
            var rand = UnityEngine.Random.Range(0, 5);
            var direction = Direction.None;
            switch(rand)
            {
                case 0:
                    direction = Direction.None;
                    break;
                case 1:
                    direction = Direction.Up;
                    break;
                case 2:
                    direction = Direction.Right;
                    break;
                case 3:
                    direction = Direction.Down;
                    break;
                case 4:
                    direction = Direction.Left;
                    break;
            }

            character.Move(direction);
            yield return new WaitForSeconds(0.1f);
        }

        yield break;
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
