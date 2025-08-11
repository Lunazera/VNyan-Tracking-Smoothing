using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VNyanInterface;
using System;
using TMPro;

namespace LZTrackingSmoothingPlugin
{
    class LZMainWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [Header("Plugin Manifest")]
        [SerializeField] private string PluginVersion = "v0";
        [SerializeField] private string PluginTitle = "";
        [SerializeField] private string PluginAuthor = "Lunazera";
        [SerializeField] private string PluginWebsite = "https://github.com/Lunazera/";

        [Header("Window Components")]
        [Tooltip("Object for background and outline")]
        [SerializeField] private GameObject Background;
        [Tooltip("Text for plugin title")]
        [SerializeField] private TMP_Text Title;
        [Tooltip("Text for version and author credit")]
        [SerializeField] private TMP_Text Version;
        [Tooltip("Free text field 1")]
        [SerializeField] private TMP_Text Desc1;
        [Tooltip("Free text field 2")]
        [SerializeField] private TMP_Text Desc2;

        [Header("Close Button")]
        [Tooltip("Top right close button")]
        [SerializeField] private Button closeButton;

        private RectTransform mainRect;
        private Button VersionURLButton;

        void Start()
        {
            // Set info from manifest
            Title.text = PluginTitle;
            Version.text = PluginVersion + " - " + PluginAuthor;

            // To transform the window we need to get the transform component of the type RectTransform!
            mainRect = GetComponent(typeof(RectTransform)) as RectTransform;

            // Link Close button
            closeButton.onClick.AddListener(delegate { CloseButtonClicked(); });

            VersionURLButton = Version.GetComponent<Button>();
            VersionURLButton.onClick.AddListener(delegate { VersionClicked(); });

            // Theme applies if we aren't in editor
            if (!Application.isEditor)
            {
                changeThemeSettings();
                VNyanInterface.VNyanInterface.VNyanUI.colorThemeChanged += changeThemeSettings; // Re-init colors when this event fires
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            mainRect.anchoredPosition += eventData.delta;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            mainRect.SetAsLastSibling();
        }
        public void CloseButtonClicked()
        {
            this.gameObject.SetActive(false);
        }
        public void VersionClicked()
        {
            Application.OpenURL(PluginWebsite);
        }

        /// <summary>
        /// Method to change colours of the UI's visual components 
        /// </summary>
        public void changeThemeSettings()
        {
            // Set UI Colors from VNyan
            Background.GetComponent<Image>().color = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Panel));
            Background.GetComponent<Outline>().effectColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Borders));
            Title.color = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Text));
            Version.color = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Text));
            Desc1.color = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Text));
            Desc2.color = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Text));
            closeButton.GetComponent<Image>().color = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Panel));
            closeButton.GetComponent<Outline>().effectColor = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Borders));
            closeButton.GetComponentInChildren<TMP_Text>().color = LZUIManager.hexToColor(VNyanInterface.VNyanInterface.VNyanUI.getCurrentThemeColor(ThemeComponent.Text));
        }
    }
}
