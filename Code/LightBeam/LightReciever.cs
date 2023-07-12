using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

public class LightReciever : MonoBehaviour
{
    public UnityEvent OnLightRecived;
    public UnityEvent OnLightNotRecived;
    public bool lightGoesThrough;
    public RayColor currentColor = RayColor.Anyone;
    public Material linkedBeamMaterial;
    public ColorPropertySetter colorPropertySetter;
    public float offIntensity;
    public float onIntensity;
    Dictionary<LightBeam, LightBeamData> crossingBeams = new Dictionary<LightBeam, LightBeamData>();

    public ParticleSystem particles;
    private AudioSourceHandler laserSound;
    private void Start()
    {
        OnLightRecived.AddListener(() => colorPropertySetter.SetIntensity(linkedBeamMaterial, onIntensity));
        if (particles != null) OnLightRecived.AddListener(() => particles.Play());
        OnLightNotRecived.AddListener(() => colorPropertySetter.SetIntensity(linkedBeamMaterial, offIntensity));
        colorPropertySetter.SetIntensity(linkedBeamMaterial, offIntensity);
    }
    public void DoAction(LightBeam beam, ParticleSystem hitParticles)
    {
        if (beam.rayType != currentColor && currentColor != RayColor.Anyone) return;
        if (crossingBeams.Count == 0)
        {

            OnLightRecived?.Invoke();
            AudioManager.Play("reciverHit").SpatialBlend(transform.position, 20f).Volume(1f);
        }
        if (lightGoesThrough) StartCoroutine(AddBeam(beam, hitParticles));
        else crossingBeams.Add(beam, new LightBeamData(null, Vector3.zero, Vector3.zero));
    }
    public void UpdatePoint(LightBeam beam, Vector3 _pos, Vector3 _dir)
    {
        if (beam.rayType != currentColor && currentColor != RayColor.Anyone) return;
        if (!lightGoesThrough) return;
        foreach (KeyValuePair<LightBeam, LightBeamData> entry in crossingBeams)
        {
            if (entry.Key == beam)
            {
                entry.Value.pos = _pos;
                entry.Value.dir = _dir;
            }
        }
    }
    public void UndoAction(LightBeam beam)
    {
        if (beam.rayType != currentColor && currentColor != RayColor.Anyone) return;
        if (lightGoesThrough) CheckChildBeams(beam);
        else
        {
            crossingBeams.Remove(beam);
            if (crossingBeams.Count == 0) OnLightNotRecived?.Invoke();
        }
    }
    private void LateUpdate()
    {
        if (!lightGoesThrough) return;
        foreach (KeyValuePair<LightBeam, LightBeamData> entry in crossingBeams)
        {
            entry.Value.beam.ExecuteRay(entry.Value.pos, entry.Value.dir, entry.Value.beam.lineRenderer);
        }
    }
    void CheckChildBeams(LightBeam beam)
    {
        if (crossingBeams.ContainsKey(beam))
        {
            CheckChildBeams(crossingBeams[beam].beam);
            StartCoroutine(DestroyBeamChild(beam));
        }
    }
    IEnumerator DestroyBeamChild(LightBeam beam)
    {
        yield return new WaitForEndOfFrame();
        GameObject beamToDestroy = crossingBeams[beam].beam.lightGameObject;
        crossingBeams.Remove(beam);
        Destroy(beamToDestroy);
        if (crossingBeams.Count == 0) OnLightNotRecived?.Invoke();
    }
    IEnumerator AddBeam(LightBeam beam, ParticleSystem hitParticles)
    {
        yield return new WaitForEndOfFrame();
        LightBeam extraLightBeam = new LightBeam(beam, hitParticles);
        LightBeamData extraData = new LightBeamData(extraLightBeam, Vector3.zero, Vector3.zero);
        crossingBeams.Add(beam, extraData);
    }

    public void LaserSound()
    {
        StartCoroutine(PlayLaserSound());
    }

   
    IEnumerator PlayLaserSound()
    {
        laserSound = AudioManager.Play("spellActivation1").Volume(0.3f);
        yield return new WaitForSeconds(3);
        laserSound.Stop();
        laserSound = AudioManager.Play("fireLaser").Volume(0.3f);
    }
}
