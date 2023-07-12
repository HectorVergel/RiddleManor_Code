using System.Collections.Generic;
using UnityEngine;

public static class ParticleManager
{
    static Dictionary<string, ParticleSystem> particlesDictionary = new Dictionary<string, ParticleSystem>();
    static ParticleManager()
    {
        ParticleSystem[] particles = Resources.LoadAll<ParticleSystem>("Particles/"); 

        foreach (ParticleSystem particle in particles)
        {
            particlesDictionary.Add(particle.name, particle);
        }
    }
    public static ParticleSystem SpawnParticle(string _Name, Vector3 _Position)
    {
        if (!particlesDictionary.ContainsKey(_Name)) { Debug.Log("No particles matching the path, be sure to put the particles in the folder Particles"); return null; }
        ParticleSystem prefab = particlesDictionary[_Name];
        ParticleSystem instance = MonoBehaviour.Instantiate(prefab, _Position, Quaternion.identity);

        return instance;
    }

    public static ParticleSystem SpawnParticle(string _Name, Transform _Parent)
    {
        if (!particlesDictionary.ContainsKey(_Name)) { Debug.Log("No particles matching the path, be sure to put the particles in the folder Particles"); return null; }
        ParticleSystem prefab = particlesDictionary[_Name];
        ParticleSystem instance = MonoBehaviour.Instantiate(prefab, _Parent);

        return instance;
    }

    public static ParticleSystem SpawnParticle(string _Name, Transform _Parent, Vector3 _Position)
    {
        ParticleSystem instance = SpawnParticle(_Name,_Parent);
        instance.transform.position = _Position;

        return instance;
    }
    public static void Init(){}
}
