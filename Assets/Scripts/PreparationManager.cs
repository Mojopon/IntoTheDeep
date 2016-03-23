using UnityEngine;
using System.Collections;
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
            PopupCharacterCreator();
            return;
        }

        ActivateUIObject(phase.ToString());
    }

    private void PopupCharacterCreator()
    {
        StartCoroutine(SequenceCharacterCreator());
    }

    IEnumerator SequenceCharacterCreator()
    {
        CharacterDataTable newCharacter = null;

        yield return CharacterCreatorPopupController.StartPopup()
                                                    .StartAsCoroutine(x => newCharacter = x);

        Debug.Log(newCharacter.name);
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
