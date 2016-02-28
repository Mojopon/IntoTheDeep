using UnityEngine;
using System.Collections;

public class ParticleSpawner : MonoBehaviour
{
    public Transform attackParticle;

    private static ParticleSpawner instance;
    public static ParticleSpawner Instance { get { return instance; } }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
    }

    public void Spawn(ParticleType particleType, Vector3 position)
    {
        var particle = Instantiate(GetParticleFromType(particleType), position, Quaternion.identity) as GameObject;
        Destroy(particle, 10f);
    }

    public Transform GetParticleFromType(ParticleType type)
    {
        Transform particle = null;
        switch(type)
        {
            case ParticleType.AttackParticle:
                particle = attackParticle;
                break;
        }

        return particle;
    }
}
