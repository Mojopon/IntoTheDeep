using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx;

public class CharacterMakingPanel : MonoBehaviour
{
    public Button submitButton;
    public Button cancelButton;

    public GameObject characterMakingPanelObject;

    private static ModalPanel modalPanel;
    public static ModalPanel Instance
    {
        get
        {
            if (!modalPanel)
            {
                modalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
                if (!modalPanel)
                    Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
            }

            return modalPanel;
        }
    }

    public IObservable<Unit> ChoiceAsObservable()
    {
        Choice();
        return Observable.FromCoroutine<Unit>(SequenceWaitForChoice);
    }

    private IEnumerator SequenceWaitForChoice(IObserver<Unit> observer)
    {
        while (!buttonPressed)
        {
            yield return null;
        }

        observer.OnCompleted();
    }

    private bool buttonPressed;
    public void Choice()
    {
        buttonPressed = false;

        characterMakingPanelObject.SetActive(true);

        submitButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        submitButton.onClick.RemoveAllListeners();
        //submitButton.onClick.AddListener(details.button1Details.action);
        submitButton.onClick.AddListener(ClosePanel);
        submitButton.gameObject.SetActive(true);


        cancelButton.onClick.RemoveAllListeners();
        //cancelButton.onClick.AddListener(details.button2Details.action);
        cancelButton.onClick.AddListener(ClosePanel);
        cancelButton.gameObject.SetActive(true);
    }

    private void ClosePanel()
    {
        buttonPressed = true;
        characterMakingPanelObject.SetActive(false);
    }
}
