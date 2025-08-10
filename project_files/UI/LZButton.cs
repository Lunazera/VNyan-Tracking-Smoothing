using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VNyanInterface;

namespace LZTrackingSmoothingPlugin
{
    class LZButton : MonoBehaviour
    {
        public string buttonName;
        private float buttonState;
        private Button mainButton;

        [Header("Button Text")]
        [SerializeField] private string ButtonOnText;
        [SerializeField] private string ButtonOffText;

        // set Default colors
        Color32 ButtonOnColor = new Color(0.4f, 0.8f, 0.4f);
        Color32 ButtonOffColor = new Color(0.9f, 0.5f, 0.5f);
        Color32 ButtonTextColor = new Color(1f, 1f, 1f);
        Color32 ButtonTextOffColor = new Color(1f, 1f, 1f);

        public void Start()
        {
            mainButton = GetComponent(typeof(Button)) as Button;
            mainButton.onClick.AddListener(delegate { ButtonPressCheck(); });

            if (!Application.isEditor)
            {
                changeThemeSettings();
                VNyanInterface.VNyanInterface.VNyanUI.colorThemeChanged += changeThemeSettings;

                if (LZUIManager.getSettingsDict().TryGetValue(buttonName, out string value))
                {
                    buttonState = Convert.ToSingle(value);
                    VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(buttonName, buttonState);
                    ChangeButtonColor(buttonState == 1f);
                }
                else
                {
                    LZUIManager.addSettingsDictFloat(buttonName, buttonState);
                    ChangeButtonColor(buttonState == 1f);
                }
            } else
            {
                ChangeButtonColor(false);
            }
        }

        public void ChangeButtonColor(bool boolbuttonState)
        {
            // On State
            if (boolbuttonState)
            {
                ColorBlock cb = mainButton.colors;
                cb.normalColor = ButtonOnColor;
                cb.highlightedColor = LZUIManager.darkenColor(ButtonOnColor, 15);
                cb.pressedColor = LZUIManager.darkenColor(ButtonOnColor, 30);
                cb.selectedColor = ButtonOnColor;
                mainButton.colors = cb;

                mainButton.GetComponentInChildren<TMP_Text>().color = ButtonTextColor;
                mainButton.GetComponentInChildren<TMP_Text>().text = ButtonOnText;
            }
            // Off State
            else
            {
                ColorBlock cb = mainButton.colors;
                cb.normalColor = ButtonOffColor;
                cb.highlightedColor = LZUIManager.darkenColor(ButtonOffColor, 15);
                cb.pressedColor = LZUIManager.darkenColor(ButtonOffColor, 30);
                cb.selectedColor = ButtonOffColor;
                mainButton.colors = cb;

                mainButton.GetComponentInChildren<TMP_Text>().color = ButtonTextOffColor;
                mainButton.GetComponentInChildren<TMP_Text>().text = ButtonOffText;
            }
        }

        public void setButtonVNyanState(float state)
        {
            if (!Application.isEditor)
            {
                LZUIManager.setSettingsDictFloat(buttonName, state);
            }
        }
        
        // Cool stuff happening!
        public void ButtonPressCheck()
        {
            // If button state was off, run on part of script
            if (buttonState == 0f)
            {
                buttonState = 1f;
                setButtonVNyanState(buttonState);
                ChangeButtonColor(true);
            }
            // If the button was not off, run off part of the script!
            else
            {
                buttonState = 0f;
                setButtonVNyanState(buttonState);
                ChangeButtonColor(false);
            }
        }

        public void changeThemeSettings()
        {
            ButtonOnColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Button));
            ButtonOffColor = LZUIManager.darkenColor(ButtonOnColor, 50);
            ButtonTextColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.ButtonText));
            ButtonTextOffColor = LZUIManager.darkenColor(ButtonOnColor, 20);

            ChangeButtonColor(buttonState == 1f);
        }
    }
}
