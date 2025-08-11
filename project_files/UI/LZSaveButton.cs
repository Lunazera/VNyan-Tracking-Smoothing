using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VNyanInterface;

namespace LZTrackingSmoothingPlugin
{
    class LZSaveButton : MonoBehaviour
    {
        [Header("Settings File")]
        [Tooltip("Filename to use for settings JSON. Should be the same as in LZUIManager")]
        [SerializeField] private string settingName;

        private Button mainButton;

        public void Start()
        {
            // Get button!
            mainButton = GetComponent(typeof(Button)) as Button;
            // Add listener to if button is pressed. It will run ButtonPressCheck if it is!
            mainButton.onClick.AddListener(delegate { ButtonPressCheck(); });

            if (!Application.isEditor)
            {
                changeThemeSettings();
                VNyanInterface.VNyanInterface.VNyanUI.colorThemeChanged += changeThemeSettings; // Re-init colors when this event fires
            }
        }

        public void ButtonPressCheck()
        {
            // If the dictionary exists, which it always should but just in case.
            if (!(LZUIManager.getSettingsDict() == null) && !Application.isEditor)
            {
                // Write the dictionary to a settings file!
                VNyanInterface.VNyanInterface.VNyanSettings.saveSettings(settingName, LZUIManager.getSettingsDict());
            }
        }

        /// <summary>
        /// Method to change colours of the UI's visual components 
        /// </summary>
        public void changeThemeSettings()
        {
            Color32 ButtonColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Button));
            Color32 ButtonColorHighlight = LZUIManager.darkenColor(ButtonColor, 10);
            Color32 ButtonColorSelect = LZUIManager.darkenColor(ButtonColor, 30);
            Color32 ButtonTextColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.ButtonText));

            ColorBlock cb = mainButton.colors;
            this.GetComponent<Image>().color = ButtonColor;
            cb.normalColor = ButtonColor;
            cb.highlightedColor = ButtonColorHighlight;
            cb.selectedColor = ButtonColorSelect;

            mainButton.GetComponentInChildren<TMP_Text>().color = ButtonTextColor;
        }
    }
}
