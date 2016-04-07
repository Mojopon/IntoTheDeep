using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx;

public class CharacterMakingPanel : ModalPanelBase<CharacterMakingPanel>
{
    public Button submitButton;
    public Button cancelButton;

    public GameObject characterMakingPanelObject;

    public IObservable<Unit> ChoiceAsObservable()
    {
        Choice();
        return Observable.FromCoroutine<Unit>(SequenceWaitForChoice);
    }

    private IEnumerator SequenceWaitForChoice(IObserver<Unit> observer)
    {
        while (!panelClosed)
        {
            yield return null;
        }

        observer.OnCompleted();
    }

    public void Choice()
    {
        panelClosed = false;

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

    protected override void ClosePanel()
    {
        base.ClosePanel();
        characterMakingPanelObject.SetActive(false);
    }
}
