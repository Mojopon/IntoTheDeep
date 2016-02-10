using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class DisplayPhase : MonoBehaviour
{
    public Text text;

    void Start()
    {
        var gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameManager.GetComponent<GameManager>().currentPhase
                                               .Subscribe(x =>
                                               {
                                                   text.text = x.ToString();
                                                   Debug.Log(x);
                                               })
                                               .AddTo(gameObject);
    }
}
