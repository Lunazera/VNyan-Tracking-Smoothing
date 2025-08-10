using System;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VNyanInterface;

namespace LZTrackingSmoothingPlugin
{
    class LZSlider : MonoBehaviour
    {
        [SerializeField] private string settingName;
        [Header("VNyan Parameter Name")]
        [SerializeField] private string paramName;
        private float paramValue = 0f;

        private TMP_Text textLabel;
        private Slider mainSlider;
        private TMP_InputField mainField;

        public void Awake()
        {
            // Get components
            textLabel = this.GetComponentInChildren<TMP_Text>();
            mainField = this.transform.GetChild(1).GetComponentInChildren<TMP_InputField>();
            mainSlider = this.transform.GetChild(1).GetComponentInChildren<Slider>();

            textLabel.text = settingName;

            mainSlider.onValueChanged.AddListener(delegate { sliderChangedCheck(); });
            mainField.onValueChanged.AddListener(delegate { fieldChangedCheck(); });

            if (!Application.isEditor)
            {
                changeThemeSettings();
                VNyanInterface.VNyanInterface.VNyanUI.colorThemeChanged += changeThemeSettings; // Re-init colors when this event fires

                if (LZUIManager.getSettingsDict().TryGetValue(paramName, out string value))
                {
                    paramValue = Convert.ToSingle(value);
                    mainField.text = value;
                    mainSlider.value = paramValue * 10f;  
                    VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramName, paramValue);
                }
                else
                {
                    LZUIManager.addSettingsDictFloat(paramName, paramValue);
                    mainField.text = Convert.ToString(paramValue);
                    mainSlider.value = paramValue * 10f;
                }
            }
        }

        public void changeThemeSettings()
        {
            Color32 TextColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Text));

            Color32 ComponentColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Component));
            Color32 ComponentHighlight = LZUIManager.darkenColor(ComponentColor, 10);

            Color32 ButtonColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Button));

            Color32 FillColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Icon));

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

            // Input field text
            mainField.textComponent.color = PanelComponentTextColor;
            mainField.placeholder.color = PanelComponentTextColorTransparent;

            // Slider colors
            mainSlider.targetGraphic.color = ButtonColor;
            mainSlider.fillRect.GetComponent<Image>().color = FillColor;
            mainSlider.GetComponentInChildren<Image>().color = ComponentColor;
        }

        private void sliderChangedCheck()
        {
            paramValue = mainSlider.value * 0.1f;
            LZUIManager.setSettingsDictFloat(paramName, paramValue);

            if (float.TryParse(mainField.text, out float value))
            {
                if (!(paramValue == value))
                {
                    mainField.text = paramValue.ToString();
                }
            }
            else
            {
                mainField.text = paramValue.ToString();
            }
        }

        private void fieldChangedCheck()
        {
            if (float.TryParse(mainField.text, out float value))
            {
                float inRangeValue = Mathf.Clamp(value, 0f, 100f);
                mainField.text = inRangeValue.ToString();
                if (!(paramValue == inRangeValue))
                {
                    mainSlider.value = inRangeValue * 10f;
                }
            }
        }
    }
}
