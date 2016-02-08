using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public MapInstance map;
    public CharacterManager characterManager;

    private Character character;

    void Start()
    {
        StartCoroutine(SequenceSetupGame());
    }

    IEnumerator SequenceSetupGame()
    {
        // wait for one frame for other scripts to finish start method
        yield return null;

        
    }

    void SpawnCharacterToWorld(Character character)
    {

    }
}
