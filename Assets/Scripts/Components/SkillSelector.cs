using UnityEngine;
using System.Collections;
using System;
using UniRx;

public class SkillSelector : MonoBehaviour, IMapInstanceUtilitiesUser
{
    public Transform marker;

    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
    public Func<Character, Coord, bool> MoveChecker { get; set; }

    private GameManager gameManager;
    private Character character;
    private World world;

    private Skill selectedSkill;
    public void Initialize(GameManager gameManager, MapInstance mapInstance, Character character, World world)
    {
        this.character = character;
        this.gameManager = gameManager;
        this.world = world;

        GetMapInstanceUtilities(mapInstance);

        selectedSkill = null;
    }

    public IEnumerator SequenceSelectSkill()
    {
        // sequence for npc
        if (!character.IsPlayer)
        {
            yield return StartCoroutine(SequenceSelectNPCSkill());
        }
        else {

            SkillMenu.Current.DisplaySkills(character);

            SubscribePlayerInput(InputManager.Root).AddTo(gameObject);

            SkillMenu.Current.SubmittedSkill()
                     .Subscribe(x => selectedSkill = x)
                     .AddTo(gameObject);

            while (selectedSkill == null)
            {
                yield return null;
            }

            SelectSkill();
        }
    }

    IDisposable SubscribePlayerInput(IPlayerInput inputs)
    {
        var compositeDisposable = new CompositeDisposable();

        inputs.MoveDirectionObservable
              .Skip(1)
              .Where(x => x != Direction.None)
              .Subscribe(x =>
              {
                  if (x == Direction.Up)
                  {
                      SkillMenu.Current.MoveUp();
                  }
                  else if (x == Direction.Down)
                  {
                      SkillMenu.Current.MoveDown();
                  }
              })
              .AddTo(compositeDisposable);

        inputs.OnEnterButtonObservable
              .Skip(1)
              .Where(x => x)
              .Subscribe(x => SkillMenu.Current.Submit())
              .AddTo(compositeDisposable);

        return compositeDisposable;
    }

    void SelectSkill()
    {
        world.ApplyUseSkill(character, selectedSkill);

        Destroy(gameObject);
    }

    IEnumerator SequenceSelectNPCSkill()
    {
        // simulating wait time
        yield return new WaitForSeconds(1);

        var availableSkills = character.GetSkills();
        selectedSkill = availableSkills[UnityEngine.Random.Range(0, availableSkills.Length - 1)];

        SelectSkill();
    }

    public void GetMapInstanceUtilities(IMapInstanceUtilitiesProvider provider)
    {
        CoordToWorldPositionConverter = provider.CoordToWorldPositionConverter;
    }
}
