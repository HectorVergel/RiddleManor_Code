using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LightEmiter : MonoBehaviour
{
    LightBeam beam;
    public Transform rayStartPos;
    public float width;
    public float growthSpeed;
    public Material material;
    public int maxBounces;
    public RayColor rayColor = RayColor.Anyone;
    public bool active = true;
    public ColorPropertySetter colorPropertySetter;
    public float offIntensity;
    public float onIntensity;
    public float fadeOutSpeed;
    public bool inverted;
    public ParticleSystem hitParticles;
    AudioSourceHandler sound;
    private bool played;
    LayerMask masks;
    bool locked;

    private void Start()
    {
        masks = Physics.AllLayers;
        masks &= ~(1 << LayerMask.NameToLayer("NoBeam"));
        beam = new LightBeam(transform.position, inverted ? -transform.forward : transform.forward, material, masks, maxBounces, rayColor, width, transform, growthSpeed, hitParticles);
        SetPower(active);
    }

    private void Update()
    {
        hitParticles.gameObject.SetActive(active);
        if (active) beam.ExecuteRay(rayStartPos.position, inverted ? -rayStartPos.forward : rayStartPos.forward, beam.lineRenderer);
    }
    public void SetPower(bool state)
    {
        if(locked) return;
        StopAllCoroutines();
        active = state;
        if (colorPropertySetter != null) colorPropertySetter.SetIntensity(material, active ? onIntensity : offIntensity);
        if (state == false)
        {
            StartCoroutine(LowerBeamAlpha());
            if (sound != null) sound.FadeOut(2);
        }
        else
        {
            sound = AudioManager.Play("rayLoop").SpatialBlend(transform.position, 20).FadeIn(2, 0.15f).Loop(true);

            PlayActivateSound();

        }
    }

    public void PlayActivateSound()
    {
        AudioManager.Play("emitterActivated").SpatialBlend(transform.position, 20).Volume(1f);
    }
    IEnumerator LowerBeamAlpha()
    {
        while (beam.lineRenderer.material.GetColor("_Color").a > 0)
        {
            beam.lineRenderer.material.SetColor("_Color", beam.lineRenderer.material.GetColor("_Color") - new Color(0, 0, 0, fadeOutSpeed * Time.deltaTime));
            yield return null;
        }
        Color newColor = beam.lineRenderer.material.GetColor("_Color");
        beam.lineRenderer.material.SetColor("_Color", new Color(newColor.r, newColor.g, newColor.b, 0));
        beam.ResetBeam();
    }
    public void SetLocked(bool state)
    {
        locked = state;
    }

}