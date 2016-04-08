using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public enum PreparationPhase
{
    CombatSelect,
    CharacterManagement,
    Shop,
    SkillManagement,
    ItemManagement,
}

public class PreparationManager : MonoBehaviour
{
    public void ChangePhase(int number)
    {
        var phase = (PreparationPhase)number;

        OnChangePhase(phase);
    }

    private void OnChangePhase(PreparationPhase phase)
    {
        if(phase == PreparationPhase.CharacterManagement)
        {
            PopupCharacterMakingPanel();
            return;
        }

        ActivateUIObject(phase.ToString());
    }

    private void PopupCharacterMakingPanel()
    {
        StartCoroutine(SequenceCharacterMaking());
    }

    IEnumerator SequenceCharacterMaking()
    {
        CharacterMakingPanel.Result result = null;

        var characterMakingPanelDetails = new CharacterMakingPanel.Details()
        {
            jobs = new List<string>()
            {
                "剣士",
                "魔術師",
                "クレリック",
            }
        };
        yield return CharacterMakingPanel.Instance.CharacterMakingAsObservable(characterMakingPanelDetails).StartAsCoroutine(x => result = x);

        Debug.Log(result.selectedJob);
        Debug.Log(result.name);
    }

    private GameObject currentActiveObject;
    private void ActivateUIObject(string objectTag)
    {
        if (currentActiveObject != null)
        {
            currentActiveObject.SetActive(false);
            currentActiveObject = null;
        }

        var uiObjectsParent = GameObject.FindGameObjectWithTag(objectTag);
        if (uiObjectsParent == null) return;

        var obj = uiObjectsParent.transform.GetChild(0);
        obj.gameObject.SetActive(true);
        currentActiveObject = obj.gameObject;

    }
}
