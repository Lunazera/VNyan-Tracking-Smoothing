using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VNyanInterface;

namespace LZTrackingSmoothingPlugin
{
    class LZInputField : MonoBehaviour
    {
        [Header("Setting Labels")]
        [Tooltip("Setting name to be shown in plugin UI")]
        [SerializeField] private string settingName;

        [Tooltip("Setting name to be used in VNyan and settings JSON")]
        [SerializeField] private string vnyanParamName;
        private float paramValue = 0;

        private TMP_InputField mainField;
        private TMP_Text textLabel;
        private Button mainButton;

        public void Awake()
        {
            // Get components
            textLabel = this.GetComponentInChildren<TMP_Text>();
            mainField = this.transform.GetChild(1).GetComponentInChildren<TMP_InputField>();
            mainButton = this.transform.GetChild(1).GetComponentInChildren<Button>();

            textLabel.text = settingName;

            mainButton.onClick.AddListener(delegate { ButtonPressCheck(); });

            if (!Application.isEditor)
            {
                changeThemeSettings();
                VNyanInterface.VNyanInterface.VNyanUI.colorThemeChanged += changeThemeSettings; // Re-init colors when this event fires

                if (LZUIManager.getSettingsDict().TryGetValue(vnyanParamName, out string value))
                {
                    paramValue = Convert.ToSingle(value);
                    mainField.text = value;
                    VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(vnyanParamName, paramValue);
                } 
                else
                {
                    LZUIManager.addSettingsDictFloat(vnyanParamName, paramValue);
                    mainField.text = Convert.ToString(paramValue);
                }
            }
        }

        /// <summary>
        /// Method to change colours of the UI's visual components 
        /// </summary>
        public void changeThemeSettings()
        {
            Color32 TextColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Text));
            
            Color32 ComponentColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Component));
            Color32 ComponentHighlight = LZUIManager.darkenColor(ComponentColor, 10);

            Color32 ButtonColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Button));
            Color32 ButtonColorHighlight = LZUIManager.darkenColor(ButtonColor, 10);
            
            Color32 ButtonTextColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.ButtonText));

            Color32 PanelComponentTextColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.PanelComponentText));
            Color32 PanelComponentTextColorTransparent = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.PanelComponentText), 70);

            // Label color
            textLabel.color = TextColor;

            // Input field colors
            ColorBlock cbmainField = mainField.colors;
            mainField.GetComponent<Image>().color = ComponentColor;
            cbmainField.normalColor = ComponentColor;
            cbmainField.highlightedColor = ComponentHighlight;
            cbmainField.selectedColor = ComponentColor;

            mainField.textComponent.color = TextColor;
            mainField.placeholder.color = PanelComponentTextColorTransparent;

            // Button colors
            ColorBlock cbmainButton = mainButton.colors;
            mainButton.GetComponent<Image>().color = ButtonColor;
            cbmainButton.normalColor = ButtonColor;
            cbmainButton.highlightedColor = ButtonColorHighlight;
            cbmainButton.selectedColor = ButtonColor;

            mainButton.GetComponentInChildren<TMP_Text>().color = ButtonTextColor;
        }

        public void ButtonPressCheck()
        {
            // We need to sanitate the input a bit. Unless the input can be converted to a float we can't use it.
            if (float.TryParse(mainField.text, out float fieldValue))
            {
                LZUIManager.setSettingsDictFloat(vnyanParamName, fieldValue);
            }
            else
            {
                mainField.text = Convert.ToString(LZUIManager.getSettingsDictFloat(vnyanParamName));
            }
        }
    }
}
