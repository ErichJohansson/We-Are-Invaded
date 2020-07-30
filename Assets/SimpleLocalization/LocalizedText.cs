using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleLocalization
{
	/// <summary>
	/// Localize text component.
	/// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        public string LocalizationKey;
        private Text txt;

        private void Awake()
        {
            if (txt == null)
                txt = GetComponent<Text>();
            if (LocalizationKey.Length == 0 && Regex.IsMatch(txt.text, "[a-z].[a-z]", RegexOptions.IgnoreCase))
                LocalizationKey = txt.text;
        }

        public void Start()
        {
            Localize();
            LocalizationManager.LocalizationChanged += Localize;
        }

        public void OnDestroy()
        {
            LocalizationManager.LocalizationChanged -= Localize;
        }

        public void Localize()
        {
            if (LocalizationKey.Length == 0) Awake();
            txt.text = LocalizationManager.Localize(LocalizationKey);
        }
    }
}