using UnityEngine;
using System.Collections;
using UniRx;

public class CharacterCreatorPopupController : MonoBehaviour
{
    public static CharacterCreatorPopupController Instance;

    private GameObject popupWindowObjects;
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        var child = transform.GetChild(0);
        popupWindowObjects = child.gameObject;
        popupWindowObjects.SetActive(false);
    }

    public static IObservable<CharacterDataTable> StartPopup()
    {
        return Instance.Popup();
    }

    public IObservable<CharacterDataTable> Popup()
    {
        popupWindowObjects.SetActive(true);
        return Observable.FromCoroutine<CharacterDataTable>(observer => PopupCreationWindow(observer));
    }

    private bool creating = false;
    private CharacterCreator characterCreator;
    IEnumerator PopupCreationWindow(IObserver<CharacterDataTable> observer)
    {
        characterCreator = new CharacterCreator();
        creating = true;

        while(creating)
        {
            yield return null;
        }

        observer.OnNext(characterCreator.Get());
        observer.OnCompleted();

        ClosePopup();
    }

    private void ClosePopup()
    {
        popupWindowObjects.SetActive(false);
        characterCreator = null;
    }

    public void Create()
    {
        characterCreator.ChangeName("aaaaa");
        creating = false;
    }
}
