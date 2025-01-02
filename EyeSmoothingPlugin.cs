using System;
using VNyanInterface;
using UnityEngine;
using EyeSmoothLayer;


namespace VNyanEyeSmoothing
{
    public class EyeSmoothingPlugin : MonoBehaviour
    {
        private string paramNameEyeSmoothActive = "LZ_eyeSmoothActive";
        public float eyeSmoothActive = 1f;

        private string paramNameSmoothSpeed = "LZ_eyeSmoothSpeed";
        public float smoothSpeed = 20f;

        IPoseLayer EyeSmooth = new EyeSmoothLayer.EyeSmoothLayer();

        public void Start()
        {
            // Get values from Sjatar's parameter dictionary (if they exist)
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameSmoothSpeed))
            {
                smoothSpeed = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameSmoothSpeed]);
            }

            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameSmoothSpeed, smoothSpeed);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameEyeSmoothActive, eyeSmoothActive);

            VNyanInterface.VNyanInterface.VNyanAvatar.registerPoseLayer(EyeSmooth);
        }

        public void Update()
        {
            // Get current values from VNyan parameters
            smoothSpeed = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameSmoothSpeed);
            eyeSmoothActive = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameEyeSmoothActive);

            // Set layer settings
            EyeSmoothSettings.setEyeSmoothLayerOnOff(eyeSmoothActive);
            EyeSmoothSettings.setEyeSmoothSpeed(smoothSpeed);
        }
    }
}