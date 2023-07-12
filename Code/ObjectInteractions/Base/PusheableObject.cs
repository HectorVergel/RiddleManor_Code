using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
[RequireComponent(typeof(Rigidbody))]
public class PusheableObject : MonoBehaviour
{
    public UnityEvent OnSelected;
    public UnityEvent OnUnselected;
    public ParticleSystem fallParticles;
    [NonSerialized]
    public Rigidbody rb;
    bool constrained;
    Vector3 constrainDirection;
    public GameObject particles;
    public Transform focus;
    public float groundDetectionDistance = 0.2f;
    public float distanceToCheckOnGrounded = 0.3f;
    public float speedUp = 1f;
    public bool tongue;
    AudioSourceHandler sound;
    public bool canBePushed = true;
    bool particlesBlocked;
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position,transform.position-new Vector3(0,groundDetectionDistance,0));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,transform.position+new Vector3(distanceToCheckOnGrounded,0,0));
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    private void OnEnable() {
        PlayerController.instance.OnObjectPushed += HideParticles;
        PlayerController.instance.OnStoppedPushing += ShowParticles;
        PlayerController.instance.OnBookActivated += HideParticles;
        PlayerController.instance.OnPlayerActivated += ShowParticles;
    }
    private void OnDisable() {
        PlayerController.instance.OnObjectPushed -= HideParticles;
        PlayerController.instance.OnStoppedPushing -= ShowParticles;
        PlayerController.instance.OnBookActivated -= HideParticles;
        PlayerController.instance.OnPlayerActivated -= ShowParticles;
    }
    public void MakePusheable()
    {
        rb.isKinematic = false;
        CameraController.instance.ChangeFocus(PlayerController.instance.cameraFocus);
        OnSelected?.Invoke();
        sound = AudioManager.Play("dragObjectLoop").Volume(0.15f).Loop(true).Mute(true).Pitch(0.6f);
        if(!tongue) AudioManager.Play("kidGrabObject3").Volume(0.2f).Pitch(0.7f);
        else AudioManager.Play("kidGrabTongue2").Volume(0.4f);
    }
    public void NotPusheable()
    {
        rb.isKinematic = true;
        CameraController.instance.ChangeFocus(PlayerController.instance.cameraFocus);
        OnUnselected?.Invoke();
        sound.Stop();
        if(tongue) AudioManager.Play("kidGrabTongue1").Volume(0.4f);
    }

    public void BoxFall()
    {
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.freezeRotation = false;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.None;
    }
    public void AddForceTowardsDirection(float force, Vector2 direction)
    {
        direction.Normalize();
        Vector3 newDir = new Vector3(direction.x,0,direction.y);
        if(constrained)
        {
            if(Vector3.Dot(newDir,constrainDirection) <= 0)
            {
                rb.velocity = Vector3.zero;
                sound.Mute(true);
                return;
            }
        }
        if(!OnGround(transform.position))
        {
            if(!OnGround(transform.position+newDir*distanceToCheckOnGrounded))
            {
                rb.velocity = Vector3.zero;
                sound.Mute(true);
                return;
            }
        }
        if(!OnGround(PlayerController.instance.transform.position))
        {
            if(!OnGround(PlayerController.instance.transform.position+newDir*distanceToCheckOnGrounded))
            {
                rb.velocity = Vector3.zero;
                sound.Mute(true);
                return;
            }
        }
        if(newDir!=Vector3.zero) sound.Mute(false);
        else sound.Mute(true);
        rb.velocity = newDir * force * speedUp *Time.fixedDeltaTime;
    }
    public void SetConstraint(bool state, Vector3 ropeDirection)
    {
        constrained = state;
        constrainDirection = new Vector3(ropeDirection.x,0,ropeDirection.z).normalized;
    }
    void ShowParticles()
    {
        if (!particles) return;
        if(particlesBlocked) return;
        particles.SetActive(true);
    }
    void HideParticles()
    {
        if (!particles) return;
        if(particlesBlocked) return;
        particles.SetActive(false);
    }
    public void ChangeFocus(Transform newFocus)
    {
        focus = newFocus;
    }
    bool OnGround(Vector3 pos)
    {
        return Physics.Raycast(pos,Vector3.down,groundDetectionDistance,Physics.AllLayers, QueryTriggerInteraction.Ignore);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (rb.freezeRotation) return;
        AudioManager.Play("boxDropping");
        fallParticles?.Play();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        transform.position += new Vector3(0, 0.2f, 0);
    }
    public void BlockParticles()
    {
        particlesBlocked = true;
        particles.SetActive(false);
    }
}

