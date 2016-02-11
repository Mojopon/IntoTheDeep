using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillMenu : MonoBehaviour, IInputtable {

    public static SkillMenu Current;

    public Text[] skillNameDisplays;
    private int selectedSkillNumber;

    [HideInInspector]
    private Skill[] skillsToDisplay;

	void Start ()
    {
        if(Current != null && Current != this)
        {
            Destroy(Current.gameObject);
        }

        Current = this;

        HideSkills();
	}

    public void DisplaySkills(Skill[] skills)
    {
        skillsToDisplay = skills;

        for(int i = 0; i < skillNameDisplays.Length; i++)
        {
            if (i >= skills.Length) return;

            skillNameDisplays[i].text = skills[i].name;
            skillNameDisplays[i].gameObject.SetActive(true);
        }

        skillNameDisplays[selectedSkillNumber].color = Color.red;
    }

    public void Input(PlayerCommand command)
    {

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
