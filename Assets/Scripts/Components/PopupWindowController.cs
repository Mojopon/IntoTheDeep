using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupWindowController : MonoBehaviour
{
    [SerializeField]
    private Text message;
    public GameObject yesButton;
    public GameObject noButton;

    public void SetMessage(string text)
    {
        message.text = text;
    }

    void OnEnable()
    {
        yesButton.SetActive(false);
        noButton.SetActive(false);

        yesButton.SetActive(true);
    }

    public void OnSelect(int selected)
    {
        if(selected == 0)
        {
            noButton.SetActive(false);
            yesButton.SetActive(true);
        }
        else if(selected == 1)
        {
            yesButton.SetActive(false);
            noButton.SetActive(true);
        }
    }
}
