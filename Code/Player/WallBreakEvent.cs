using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.VFX;

public class WallBreakEvent : MonoBehaviour
{
    public List<GameObject> woodPlates;
    public List<GameObject> woodPlatesBroken;
    public List<GameObject> monsterHead;
    public ParticleSystem particles;
    public Transform explosionTransform;


    public float forceApplied = 5f;
    public float timeForPieces = 5f;
    public float timeToActivateHead = 5f;
    public float radiusExplosion;
    public UnityEvent externalEvent;

    private void ApplyForces()
    {
        if (woodPlatesBroken == null) return;
        foreach (GameObject piece in woodPlatesBroken)
        {
            Rigidbody pieceRb = piece.GetComponent<Rigidbody>();
            pieceRb.AddExplosionForce(forceApplied,explosionTransform.position, radiusExplosion); //No idea how explosion forces works.
        }
    }

    private void DesactivateWoodPlates()
    {
        if (woodPlates == null) return;
        foreach (GameObject wood in woodPlates)
        {
            wood.SetActive(false);
        }
    }

    private void ActivateBrokenPlates()
    {
        if (woodPlatesBroken == null) return;
        foreach (GameObject piece in woodPlatesBroken)
        {
            piece.SetActive(true);
        }
    }

    private void InstantiateParticles()
    {
        if (particles == null) return;
        particles.Play();
    }

    private void ActivateMonsterHead()
    {
        StartCoroutine(ActivateMonsterHeadCoroutine());
    }
    IEnumerator ActivateMonsterHeadCoroutine()
    {
        yield return new WaitForSeconds(timeToActivateHead);
        if (monsterHead != null) 
        {
            foreach (GameObject obj in monsterHead)
            {
                obj.SetActive(true);
            }
        }
        
    }
    public void DoEvent()
    {
        InstantiateParticles(); 
        externalEvent?.Invoke(); //Camera Shake, not sure if should go here. Maybe it also should invoke the platform falling event.
        DesactivateWoodPlates();
        ActivateBrokenPlates();
        ApplyForces();
        ActivateMonsterHead();
        StartCoroutine(DisableWoodPices());
    }

    IEnumerator DisableWoodPices()
    {
        yield return new WaitForSeconds(timeForPieces);

        foreach (GameObject piece in woodPlatesBroken)
        {
            piece.SetActive(false);
        }

    }
}

