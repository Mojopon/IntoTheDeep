using UnityEngine;
using System.Collections;
using UniRx;

public class TestModalPanel : MonoBehaviour
{
    public void DoTestModalPanel()
    {
        var details = new ModalPanel.ModalPanelDetails()
        {
            question = "おまんじゅうたべます？",
            title = "選択肢",
            button1Details = new ModalPanel.EventButtonDetails()
            {
                buttonTitle = "はい",
                action = () => Debug.Log("はいじゃないが"),
            },
            button2Details = new ModalPanel.EventButtonDetails()
            {
                buttonTitle = "いいえ",
                action = () => Debug.Log("おう"),
            },
            button3Details = new ModalPanel.EventButtonDetails()
            {
                buttonTitle = "やめる",
                action = () => Debug.Log("わかりました"),
            },
        };

        StartCoroutine(SequenceModalPanelChoice(details));
    }

    IEnumerator SequenceModalPanelChoice(ModalPanel.ModalPanelDetails details)
    {
        Debug.Log("選択中");

        yield return ModalPanel.Instance.ChoiceAsObservable(details).StartAsCoroutine();

        Debug.Log("選択完了");
    }

}
