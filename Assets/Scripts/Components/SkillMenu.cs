using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UniRx;
using System.Linq;

public class SkillInfo
{
    public Character user;
    public Skill skill;
    public SkillInfo(Character user, Skill skill)
    {
        this.user = user;
        this.skill = skill;
    }
}

public class SkillMenu : MonoBehaviour {

    public static SkillMenu Current;

    public Character SkillUser;
    public ReactiveProperty<SkillInfo> SelectedSkill = new ReactiveProperty<SkillInfo>();
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

        CloseSkillMenu();
	}

    private Skill skillToSubmit;
    public IObservable<Skill> SubmittedSkill()
    {
        return Observable.FromCoroutine<Skill>(observer => SequenceSelectSkill(observer));
    }

    IEnumerator SequenceSelectSkill(IObserver<Skill> observer)
    {
        while(skillToSubmit == null)
        {
            yield return null;
        }

        observer.OnNext(skillToSubmit);
        skillToSubmit = null;
        observer.OnCompleted();
    }

    public void DisplaySkills(Character user)
    {
        this.SkillUser = user;

        var skills = user.GetSkills();
        this.skills = skills.Take(skillNameDisplays.Length).ToArray();

        for(int i = 0; i < skillNameDisplays.Length; i++)
        {
            if (i >= skills.Length) break;

            skillNameDisplays[i].gameObject.SetActive(true);
            skillNameDisplays[i].text = skills[i].name;
        }

        OnSelect();
    }

    public void MoveDown()
    {
        skillNameDisplays[selectedSkillNumber].color = Color.white;

        selectedSkillNumber++;
        if (selectedSkillNumber >= skills.Length) selectedSkillNumber = skills.Length - 1;

        OnSelect();
    }

    public void MoveUp()
    {
        skillNameDisplays[selectedSkillNumber].color = Color.white;

        selectedSkillNumber--;
        if (selectedSkillNumber < 0) selectedSkillNumber = 0;

        OnSelect();
    }

    void OnSelect()
    {
        skillNameDisplays[selectedSkillNumber].color = Color.red;
        SelectedSkill.Value = new SkillInfo(SkillUser, skills[selectedSkillNumber]);
    }

    public void Submit()
    {
        skillToSubmit = skills[selectedSkillNumber];
        CloseSkillMenu();
    }

    void CloseSkillMenu()
    {
        foreach (var skillNameDisplay in skillNameDisplays)
        {
            skillNameDisplay.text = "";
            skillNameDisplay.gameObject.SetActive(false);
            SkillUser = null;
            SelectedSkill.Value = null;
        }
    }
}
