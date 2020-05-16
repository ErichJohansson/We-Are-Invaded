using Assets.SimpleLocalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageController : MonoBehaviour
{
    public Image countryFlag;
    public List<Sprite> flags;

    private readonly List<string> langs = new List<string>{ "Russian", "English" };
    private int currentLanguage = 0;

    private void Awake()
    {
        LocalizationManager.Read();
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Russian:
                LocalizationManager.Language = "Russian";
                break;
            default:
                LocalizationManager.Language = "English";
                break;
        }
        currentLanguage = langs.FindIndex(x => x == LocalizationManager.Language);
        countryFlag.sprite = flags[currentLanguage];
    }

    public void SwitchLanguage()
    {
        currentLanguage += currentLanguage == langs.Count - 1 ? -currentLanguage : 1;
        LocalizationManager.Language = langs[currentLanguage];
        countryFlag.sprite = flags[currentLanguage];
    }
}