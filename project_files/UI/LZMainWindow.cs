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
        [SerializeField] private string PluginVersion = "v2.1";
        [SerializeField] private string PluginTitle = "Tracking Smoothing";
        [SerializeField] private string PluginAuthor = "Lunazera";
        [SerializeField] private string PluginWebsite = "https://github.com/Lunazera/VNyan-Tracking-Smoothing";

        [Header("Window Components")]
        [SerializeField] private GameObject Background;
        [SerializeField] private TMP_Text Title;
        [SerializeField] private TMP_Text Version;
        [SerializeField] private TMP_Text Desc1;
        [SerializeField] private TMP_Text Desc2;

        [Header("Close Button")]
        [SerializeField] private Button closeButton;

        [Header("Plugin Window Prefab")]
        [SerializeField] private GameObject windowPrefab;

        private RectTransform mainRect;

        void Start()
        {
            // Set info from manifest
            Title.text = PluginTitle;
            Version.text = PluginVersion + " - " + PluginAuthor;

            // To transform the window we need to get the transform component of the type RectTransform!
            mainRect = GetComponent(typeof(RectTransform)) as RectTransform;

            // Link Close button
            closeButton.onClick.AddListener(delegate { CloseButtonClicked(); });

            // Theme applies if we aren't in editor
            if (!Application.isEditor)
            {
                changeThemeSettings();
                VNyanInterface.VNyanInterface.VNyanUI.colorThemeChanged += changeThemeSettings; // Re-init colors when this event fires
            }
        }

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
            this.windowPrefab.SetActive(false);
        }
    }
}
