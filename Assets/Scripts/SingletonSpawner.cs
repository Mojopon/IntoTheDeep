using UnityEngine;
using System.Collections;

public class SingletonSpawner : MonoBehaviour
{
    public Transform inputManager;
    public Transform particleSpawner;

    void Awake()
    {
        Instantiate(inputManager);
        Instantiate(particleSpawner);

        Destroy(gameObject);
    }
}
