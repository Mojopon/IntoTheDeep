﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using UniRx;

public class ModalPanel : ModalPanelBase<ModalPanel>
{
    public Text question;
    public Button button1;
    public Button button2;
    public Button button3;

    public Text button1Text;
    public Text button2Text;
    public Text button3Text;

    public GameObject modalPanelObject;

    public class EventButtonDetails
    {
        public string buttonTitle;
        public UnityAction action;
    }

    public class ModalPanelDetails
    {
        public string title;
        public string question;
        public EventButtonDetails button1Details;
        public EventButtonDetails button2Details;
        public EventButtonDetails button3Details;
    }

    public IObservable<Unit> ChoiceAsObservable(ModalPanelDetails details)
    {
        Choice(details);
        return Observable.FromCoroutine<Unit>(SequenceWaitForChoice);
    }

    private IEnumerator SequenceWaitForChoice(IObserver<Unit> observer)
    {
        while(!panelClosed)
        {
            yield return null;
        }

        observer.OnCompleted();
    }

    public void Choice(ModalPanelDetails details)
    {
        panelClosed = false;

        modalPanelObject.SetActive(true);

        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);

        this.question.text = details.question;

        button1.onClick.RemoveAllListeners();
        button1.onClick.AddListener(details.button1Details.action);
        button1.onClick.AddListener(ClosePanel);
        button1Text.text = details.button1Details.buttonTitle;
        button1.gameObject.SetActive(true);

        if (details.button2Details != null)
        {
            button2.onClick.RemoveAllListeners();
            button2.onClick.AddListener(details.button2Details.action);
            button2.onClick.AddListener(ClosePanel);
            button2Text.text = details.button2Details.buttonTitle;
            button2.gameObject.SetActive(true);
        }

        if (details.button3Details != null)
        {
            button3.onClick.RemoveAllListeners();
            button3.onClick.AddListener(details.button3Details.action);
            button3.onClick.AddListener(ClosePanel);
            button3Text.text = details.button3Details.buttonTitle;
            button3.gameObject.SetActive(true);
        }
    }

    protected override void ClosePanel() 
    {
        base.ClosePanel();
        modalPanelObject.SetActive(false);
    }
}
