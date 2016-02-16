using UnityEngine;
using System.Collections;
using System;
using UniRx;

public class SkillSelector : MonoBehaviour, IWorldUtilitiesUser, IMapInstanceUtilitiesUser
{
    public Transform marker;

    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
    public Func<Character, Coord, bool> MoveChecker { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    private GameManager gameManager;
    private Character character;
    private World world;

    private Skill selectedSkill;
    public void Initialize(GameManager gameManager, MapInstance mapInstance, Character character, World world)
    {
        this.character = character;
        this.gameManager = gameManager;
        this.world = world;
        world.ProvideWorldUtilities(this);

        mapInstance.ProvideMapInstanceUtilities(this);

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

            gameManager.PlayerInput.Skip(1)
                                   .Where(x => !gameManager.MenuIsOpened)
                                   .Subscribe(x =>
                                   {
                                       SkillMenu.Current.Input(x);
                                   })
                                   .AddTo(gameObject);

            var cancel = SkillMenu.Current.SubmittedSkill()
                                  .Subscribe(x => selectedSkill = x)
                                  .AddTo(gameObject);

            while (selectedSkill == null)
            {
                yield return null;
            }

            SelectSkill();
        }
    }

    void SelectSkill()
    {
        Debug.Log(selectedSkill.name + " used");
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
}
