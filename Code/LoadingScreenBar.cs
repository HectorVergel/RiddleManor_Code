using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreenBar : MonoBehaviour
{
    [SerializeField] private Image barImage;    
    private void Update()
    {
        barImage.fillAmount = Mathf.MoveTowards(barImage.fillAmount, Loader.instance.GetLoadingProgress(), 1f * Time.deltaTime);

        if(barImage.fillAmount >= 1f)
        {
            Loader.instance.GetAsyncOperation().allowSceneActivation = true;
        }
    }
    
    
}
