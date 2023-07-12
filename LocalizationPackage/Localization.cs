using UnityEngine;
using TMPro;

public class Localization : MonoBehaviour
{
    public string reference;
    TextMeshProUGUI text;
    private void OnEnable()
    {
        if (text==null) text = GetComponent<TextMeshProUGUI>();
        LocalizationManager.onLanguageChange += UpdateTextLanguage;
        UpdateTextLanguage();
    }

    private void OnDisable()
    {
        LocalizationManager.onLanguageChange -= UpdateTextLanguage;
    }  

    void UpdateTextLanguage()
    {
        text.text = LocalizationManager.GetLocalizedValue(reference);
    }

}


