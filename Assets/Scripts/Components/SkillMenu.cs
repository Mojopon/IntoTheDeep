using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UniRx;
using System.Linq;

public class SkillMenu : MonoBehaviour, IInputtable {

    public static SkillMenu Current;

    public Text[] skillNameDisplays;
    private int selectedSkillNumber;

    [HideInInspector]
    private Skill[] skills;

	void Start ()
    {
        if(Current != null && Current != this)
        {
            Destroy(Current.gameObject);
        }

        Current = this;

        HideSkills();
	}

    private Skill selectedSkill;
    public IObservable<Skill> SelectedSkill()
    {
        return Observable.FromCoroutine<Skill>(observer => SequenceSelectSkill(observer));
    }

    IEnumerator SequenceSelectSkill(IObserver<Skill> observer)
    {
        while(selectedSkill == null)
        {
            yield return null;
        }

        observer.OnNext(selectedSkill);
        selectedSkill = null;
        observer.OnCompleted();
    }

    public void DisplaySkills(Skill[] skills)
    {
        this.skills = skills.Take(skillNameDisplays.Length).ToArray();

        for(int i = 0; i < skillNameDisplays.Length; i++)
        {
            if (i >= skills.Length) break;

            skillNameDisplays[i].gameObject.SetActive(true);
            skillNameDisplays[i].text = skills[i].name;
        }

        skillNameDisplays[selectedSkillNumber].color = Color.red;
    }

    void MoveDown()
    {
        skillNameDisplays[selectedSkillNumber].color = Color.white;

        selectedSkillNumber++;
        if (selectedSkillNumber >= skills.Length) selectedSkillNumber = skills.Length - 1;

        skillNameDisplays[selectedSkillNumber].color = Color.red;
    }

    void MoveUp()
    {
        skillNameDisplays[selectedSkillNumber].color = Color.white;

        selectedSkillNumber--;
        if (selectedSkillNumber < 0) selectedSkillNumber = 0;

        skillNameDisplays[selectedSkillNumber].color = Color.red;
    }

    void SelectSkill()
    {
        selectedSkill = skills[selectedSkillNumber];
        HideSkills();
    }

    public void Input(PlayerCommand command)
    {
        switch(command)
        {
            case PlayerCommand.Up:
                MoveUp();
                break;
            case PlayerCommand.Down:
                MoveDown();
                break;
            case PlayerCommand.Left:
            case PlayerCommand.Right:
                SelectSkill();
                break;
        }
    }

    void HideSkills()
    {
        foreach (var skillNameDisplay in skillNameDisplays)
        {
            skillNameDisplay.text = "";
            skillNameDisplay.gameObject.SetActive(false);
        }
    }
}
