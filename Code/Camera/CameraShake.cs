using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float magnitude;
    public static CameraShake instance;
    void Awake()
    {
        instance = this;
    }


    public void StartShake(float time)
    {
        if(OptionsManager.cameraShake) StartCoroutine(Shake(time));
    }
    IEnumerator Shake(float time)
    {
        float timer = 0f;

        while (timer < time)
        {
            timer += Time.deltaTime;
            ShakeCam();
            yield return null;
        }

    }
    public void ShakeCam()
    {
        float x = Random.Range(-0.5f, 0.5f) * magnitude;
        float y = Random.Range(-0.5f, 0.5f) * magnitude;

        transform.localPosition = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
    }

    public void PlayShakeSound()
    {
        AudioManager.Play("shakeSound").Volume(1f);
    }

    public void PlayEyeSound()
    {
        AudioManager.Play("eyeHitSound").Volume(1f);
    }

}
