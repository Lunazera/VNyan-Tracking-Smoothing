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
        private float eyeSmoothActive_new = 1f;

        private string paramNameSmoothSpeed = "LZ_eyeSmoothSpeed";
        public float smoothSpeed = 20f;
        private float smoothSpeed_new = 20f;

        private string paramNameJitterRemoval = "LZ_eyeSmoothJitter";
        public float jitter_removal = 0.01f;
        private float jitter_removal_new = 0.01f;

        private string paramNameAngleSpeed = "LZ_eyeSmoothAngleSpeed";
        public float angleSpeed = 80f;
        private float angleSpeed_new = 80f;

        private string paramNameEyeBlinkThreshold = "LZ_eyeSmoothBlinkThreshold";
        public float eyeBlinkThreshold = 50f;
        private float eyeBlinkThreshold_new = 50f;

        IPoseLayer EyeSmooth = new EyeSmoothLayer.EyeSmoothLayer();

        public void Start()
        {
            // Get values from Sjatar's parameter dictionary (if they exist)
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameSmoothSpeed))
            {
                smoothSpeed = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameSmoothSpeed]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameJitterRemoval))
            {
                jitter_removal = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameJitterRemoval]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameAngleSpeed))
            {
                angleSpeed = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameAngleSpeed]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameEyeBlinkThreshold))
            {
                eyeBlinkThreshold = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameEyeBlinkThreshold]);
            }

            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameSmoothSpeed, smoothSpeed);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameJitterRemoval, jitter_removal);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameAngleSpeed, angleSpeed);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameEyeBlinkThreshold, eyeBlinkThreshold);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameEyeSmoothActive, eyeSmoothActive);

            EyeSmoothSettings.setEyeSmoothSpeed(smoothSpeed);
            EyeSmoothSettings.setEyeSmoothLayerOnOff(eyeSmoothActive);
            EyeSmoothSettings.setJitterRemoval(jitter_removal);
            EyeSmoothSettings.setAngleSpeed(angleSpeed);
            EyeSmoothSettings.setBlendshapeBlinkThreshold(eyeBlinkThreshold);

            VNyanInterface.VNyanInterface.VNyanAvatar.registerPoseLayer(EyeSmooth);
        }

        public void Update()
        {
            // Get current values from VNyan parameters
        
            smoothSpeed_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameSmoothSpeed);
            if (smoothSpeed_new != smoothSpeed)
            {
                smoothSpeed = smoothSpeed_new;
                EyeSmoothSettings.setEyeSmoothSpeed(smoothSpeed);
            }

            eyeSmoothActive_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameEyeSmoothActive);
            if (eyeSmoothActive_new != eyeSmoothActive)
            {
                eyeSmoothActive = eyeSmoothActive_new;
                EyeSmoothSettings.setEyeSmoothLayerOnOff(eyeSmoothActive);
            }

            jitter_removal_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameJitterRemoval);
            if (jitter_removal_new != jitter_removal)
            {   
                jitter_removal = jitter_removal_new;
                EyeSmoothSettings.setJitterRemoval(jitter_removal);
            }

            angleSpeed_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameAngleSpeed);
            if (angleSpeed_new != angleSpeed)
            {
                angleSpeed = angleSpeed_new;
                EyeSmoothSettings.setAngleSpeed(angleSpeed);
            }

            eyeBlinkThreshold_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameEyeBlinkThreshold);
            if (eyeBlinkThreshold_new != eyeBlinkThreshold)
            {
                eyeBlinkThreshold = eyeBlinkThreshold_new;
                EyeSmoothSettings.setBlendshapeBlinkThreshold(eyeBlinkThreshold);
            }
        }
    }
}
