using TrackingSmoothLayer;
using UnityEngine;
using VNyanInterface;


namespace LZTrackingSmoothingPlugin
{
    public class TrackingSmoothingPlugin : MonoBehaviour
    {
        [Header("Main plugin Settings")]
        [SerializeField] private string paramNameTrackSmoothActive;
        [SerializeField] private float trackSmoothActive = 1f;

        [Header("Eyes Settings")]
        [SerializeField] private string paramNameEyeSmoothing;
        [SerializeField] private float eyeSmoothing = 0f;
        [SerializeField] private float eyeSmoothingScale = 10f;

        [SerializeField] private string paramNameEyeBoost;
        [SerializeField] private float eyeBoost = 0f;
        [SerializeField] private float eyeBoostScale = 25f;

        [SerializeField] private string paramNameEyeBlinkThreshold;
        [SerializeField] private float eyeBlinkThreshold;

        [Header("Body Settings")]
        [SerializeField] private string paramNameBodySmoothing;
        [SerializeField] private float bodySmoothing = 0f;
        [SerializeField] private float bodySmoothingScale = 10f;

        [SerializeField] private string paramNameBodyBoost;
        [SerializeField] private float bodyBoost = 0f;
        [SerializeField] private float bodyBoostScale = 25f;

        TrackSmoothLayer TrackSmoothing = new TrackingSmoothLayer.TrackSmoothLayer();

        public static void setInitialValue(string paramName, float value)
        {
            float checkValue = value;
            checkValue = LZUIManager.getSettingsDictFloat(paramName, value);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramName, checkValue);
        }

        public void Start()
        {
            if (!Application.isEditor)
            {
                VNyanInterface.VNyanInterface.VNyanAvatar.registerPoseLayer(TrackSmoothing);

                setInitialValue(paramNameTrackSmoothActive, trackSmoothActive);
                setInitialValue(paramNameEyeSmoothing, eyeSmoothing);
                setInitialValue(paramNameEyeBoost, eyeBoost);
                setInitialValue(paramNameEyeBlinkThreshold, eyeBlinkThreshold);
                setInitialValue(paramNameBodySmoothing, bodySmoothing);
                setInitialValue(paramNameBodyBoost, bodyBoost);
            }
            TrackSmoothing.getLayerSettings().setSmoothLayerOnOff(trackSmoothActive);
            TrackSmoothing.getLayerSettings().setEyeSmoothing(eyeSmoothing, eyeSmoothingScale);
            TrackSmoothing.getLayerSettings().setEyeBoost(eyeBoost, eyeBoostScale);
            TrackSmoothing.getLayerSettings().setBlendshapeBlinkThreshold(eyeBlinkThreshold);
            TrackSmoothing.getLayerSettings().setBodySmoothing(bodySmoothing, bodySmoothingScale);
            TrackSmoothing.getLayerSettings().setBodyBoost(bodyBoost, bodyBoostScale);
        }

        /// <summary>
        /// Checks if current VNyan parameter is different than 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static bool checkForNewValue(string paramName, float currentValue)
        {
            return (!(currentValue == LZUIManager.getSettingsDictFloat(paramName)));
        }

        public void Update()
        {
            // Get current values from VNyan parameters
            // This is only to grab the new values as they are changed, and should only really apply when the plugin window is open.
            if (checkForNewValue(paramNameTrackSmoothActive, trackSmoothActive))
            {
                trackSmoothActive = LZUIManager.getSettingsDictFloat(paramNameTrackSmoothActive);
                TrackSmoothing.getLayerSettings().setSmoothLayerOnOff(trackSmoothActive);
            }

            if (checkForNewValue(paramNameEyeSmoothing, eyeSmoothing))
            {
                eyeSmoothing = LZUIManager.getSettingsDictFloat(paramNameEyeSmoothing);
                TrackSmoothing.getLayerSettings().setEyeSmoothing(eyeSmoothing, eyeSmoothingScale);
            }
            if (checkForNewValue(paramNameEyeBoost, eyeBoost))
            {
                eyeBoost = LZUIManager.getSettingsDictFloat(paramNameEyeBoost);
                TrackSmoothing.getLayerSettings().setEyeBoost(eyeBoost, eyeBoostScale);
            }

            if (checkForNewValue(paramNameEyeBlinkThreshold, eyeBlinkThreshold))
            {
                eyeBlinkThreshold = LZUIManager.getSettingsDictFloat(paramNameEyeBlinkThreshold);
                TrackSmoothing.getLayerSettings().setBlendshapeBlinkThreshold(eyeBlinkThreshold);
            }

            

            if (checkForNewValue(paramNameBodySmoothing, bodySmoothing))
            {
                bodySmoothing = LZUIManager.getSettingsDictFloat(paramNameBodySmoothing);
                TrackSmoothing.getLayerSettings().setBodySmoothing(bodySmoothing, bodySmoothingScale);
            }

            if (checkForNewValue(paramNameBodyBoost, bodyBoost))
            {
                bodyBoost = LZUIManager.getSettingsDictFloat(paramNameBodyBoost);
                TrackSmoothing.getLayerSettings().setBodyBoost(bodyBoost, bodyBoostScale);
            }
        }
    }
}