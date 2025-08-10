using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Security;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using VNyanInterface;
using static UnityEngine.Random;

// UI Core
// modified from Sjatar's UI code example for VNyan Plugin UI's: https://github.com/Sjatar/Screen-Light

namespace LZTrackingSmoothingPlugin
{
    // VNyanInterface.IButtonClickHandler gives access to pluginButtonClicked
    public class LZUIManager : MonoBehaviour, IButtonClickedHandler
    {
        [Header("Main Window Prefab")]
        [SerializeField] public GameObject windowPrefab;
        [SerializeField] private string PluginMenuName = "Tracking Smoothing";

        [Header("Settings File")]
        [SerializeField] private string settingName;

        private GameObject window;

        private static Dictionary<string, string> settings = new Dictionary<string, string>();

        public static Dictionary<string, string> getSettingsDict()
        {
            return settings;
        }

        /// <summary>
        /// Tries to Add the value in the dictionary and set in VNyan
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void addSettingsDictFloat(string key, float value)
        {
            if (!getSettingsDict().TryAdd(key, Convert.ToString(value)))
            {
                getSettingsDict()[key] = Convert.ToString(value);
            }
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(key, value);
        }

        /// <summary>
        /// Tries to set the dictionary value only if the key exists AND set in VNyan
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void setSettingsDictFloat(string key, float value)
        {
            if (getSettingsDict().ContainsKey(key))
            {
                getSettingsDict()[key] = Convert.ToString(value);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(key, value);
            }
        }

        /// <summary>
        /// Tries to get the dictionary value, converting to float
        /// </summary>
        /// <param name="key">Setting param name</param>
        /// <param name="val">Default value</param>
        /// <returns></returns>
        public static float getSettingsDictFloat(string key, float val = 0f)
        {
            if (getSettingsDict().TryGetValue(key, out string value))
            {
                return Convert.ToSingle(value);
            } 
            else
            {
                return val;
            }
        }

        private void Awake()
        {
            if (!Application.isEditor)
            {
                // Load Settings File
                loadSettings();

                // Register UI button
                VNyanInterface.VNyanInterface.VNyanUI.registerPluginButton(PluginMenuName, (IButtonClickedHandler)this);
                this.window = (GameObject)VNyanInterface.VNyanInterface.VNyanUI.instantiateUIPrefab((object)this.windowPrefab);
            }

            if ((UnityEngine.Object)this.window != (UnityEngine.Object)null)
            {
                this.window.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
                this.window.SetActive(false);
            }
        }

        public void loadSettings()
        {
            if (null != VNyanInterface.VNyanInterface.VNyanSettings.loadSettings(settingName))
            {
                settings = VNyanInterface.VNyanInterface.VNyanSettings.loadSettings(settingName);
            }
        }

        

        public void pluginButtonClicked()
        {
            if ((UnityEngine.Object)this.window == (UnityEngine.Object)null)
                return;
            this.window.SetActive(!this.window.activeSelf);
            if ( !this.window.activeSelf )
                return;
            this.window.transform.SetAsLastSibling();
        }
        public static Color hexToColor(string hex, byte alpha = 255)
        {
            // conversion from hex to rgb, needed to read from VNyan theme components.
            hex = hex.Replace("0x", ""); //in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", ""); //in case the string is formatted #FFFFFF

            byte a = alpha; //assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
        }

        public static Color32 darkenColor(Color32 color, byte amount)
        {
            byte r = (byte)Mathf.Max(0, color.r - amount);
            byte g = (byte)Mathf.Max(0, color.g - amount);
            byte b = (byte)Mathf.Max(0, color.b - amount);

            return new Color32(r, g, b, color.a);
        }
    }
}
