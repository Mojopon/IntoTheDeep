using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx;
using System.Collections.Generic;

public class CharacterMakingPanel : ModalPanelBase<CharacterMakingPanel>
{
    public Button submitButton;
    public Button cancelButton;

    public InputField nameInputField;
    public Dropdown jobSelectDropdown;

    public GameObject characterMakingPanelObject;

    public class Result
    {
        public string name;
        public int selectedJob;
    }

    public class Details
    {
        public List<string> jobs;
    }

    public IObservable<Result> CharacterMakingAsObservable(Details details)
    {
        OpenPanel(details);
        return Observable.FromCoroutine<Result>(SequenceWaitForCharacterMaking);
    }

    private IEnumerator SequenceWaitForCharacterMaking(IObserver<Result> observer)
    {
        while (!panelClosed)
        {
            yield return null;
        }

        var result = new Result()
        {
            name = nameInputField.text,
            selectedJob = jobSelectDropdown.value,
        };

        observer.OnNext(result);
        observer.OnCompleted();
    }

    public void OpenPanel(Details details)
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

        jobSelectDropdown.options = details.jobs.ToOptionDatas();
    }

    protected override void ClosePanel()
    {
        base.ClosePanel();
        characterMakingPanelObject.SetActive(false);
    }
}
