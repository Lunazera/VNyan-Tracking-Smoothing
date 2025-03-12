using System;
using VNyanInterface;
using UnityEngine;
using TrackingSmoothLayer;


namespace TrackingSmoothing
{
    public class TrackingSmoothingPlugin : MonoBehaviour
    {
        private string paramNameTrackSmoothActive = "LZ_TrackSmoothActive";
        public float trackSmoothActive = 1f;
        private float trackSmoothActive_new = 1f;

        // Eyes
        private string paramNameTrackEyesActive = "LZ_TrackSmoothEyesActive";
        public float trackEyesActive = 1f;
        private float trackEyesActive_new = 1f;

        private string paramNameEyeSmoothSpeed = "LZ_TrackSmoothEyeSpeed";
        public float eyesmoothSpeed = 40f;
        private float eyesmoothSpeed_new = 40f;

        private string paramNameEyeAngleSpeed = "LZ_TrackSmoothEyeAngleSpeed";
        public float eyeAngleSpeed = 15f;
        private float eyeAngleSpeed_new = 15f;

        private string paramNameEyeJitterRemoval = "LZ_TrackSmoothEyeJitter";
        public float eyeJitterRemoval = 0.1f;
        private float eyeJitterRemoval_new = 0.1f;

        private string paramNameEyeBlinkThreshold = "LZ_TrackSmoothBlinkThreshold";
        public float eyeBlinkThreshold = 60f;
        private float eyeBlinkThreshold_new = 60f;

        // Body
        private string paramNameTrackBodyActive = "LZ_TrackSmoothBodyActive";
        public float trackBodyActive = 1f;
        private float trackBodyActive_new = 1f;

        private string paramNameSmoothSpeed = "LZ_TrackSmoothSpeed";
        public float smoothSpeed = 50f;
        private float smoothSpeed_new = 5f;

        private string paramNameAngleSpeed = "LZ_TrackSmoothAngleSpeed";
        public float angleSpeed = 25f;
        private float angleSpeed_new = 25f;

        IPoseLayer TrackSmoothing = new TrackingSmoothLayer.TrackSmoothLayer();

        public void Start()
        {
            // Get values from Sjatar's parameter dictionary (if they exist)
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameTrackSmoothActive))
            {
                trackSmoothActive = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameTrackSmoothActive]);
            }
            // Eyes
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameTrackEyesActive))
            {
                trackEyesActive = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameTrackEyesActive]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameEyeSmoothSpeed))
            {
                eyesmoothSpeed = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameEyeSmoothSpeed]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameEyeAngleSpeed))
            {
                eyeAngleSpeed = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameEyeAngleSpeed]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameEyeJitterRemoval))
            {
                eyeJitterRemoval = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameEyeJitterRemoval]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameEyeBlinkThreshold))
            {
                eyeBlinkThreshold = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameEyeBlinkThreshold]);
            }
            // Body
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameTrackBodyActive))
            {
                trackBodyActive = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameTrackBodyActive]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameSmoothSpeed))
            {
                smoothSpeed = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameSmoothSpeed]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(paramNameAngleSpeed))
            {
                angleSpeed = Convert.ToSingle(Sja_UICore.VNyanParameters[paramNameAngleSpeed]);
            }

            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameTrackSmoothActive, trackSmoothActive);

            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameTrackEyesActive, trackEyesActive);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameEyeSmoothSpeed, eyesmoothSpeed);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameEyeAngleSpeed, eyeAngleSpeed);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameEyeJitterRemoval, eyeJitterRemoval);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameEyeBlinkThreshold, eyeBlinkThreshold);

            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameTrackBodyActive, trackBodyActive);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameSmoothSpeed, smoothSpeed);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(paramNameAngleSpeed, angleSpeed);

            TrackingSmoothSettings.setSmoothLayerOnOff(trackSmoothActive);

            TrackingSmoothSettings.setSmoothLayerOnOff(trackEyesActive);
            TrackingSmoothSettings.setEyeSmoothSpeed(eyesmoothSpeed);
            TrackingSmoothSettings.setEyeAngleSpeed(eyeAngleSpeed);
            TrackingSmoothSettings.setEyeJitterRemoval(eyeJitterRemoval);
            TrackingSmoothSettings.setBlendshapeBlinkThreshold(eyeBlinkThreshold);

            TrackingSmoothSettings.setSmoothLayerOnOff(trackBodyActive);
            TrackingSmoothSettings.setSmoothSpeed(smoothSpeed);
            TrackingSmoothSettings.setAngleSpeed(angleSpeed);

            VNyanInterface.VNyanInterface.VNyanAvatar.registerPoseLayer(TrackSmoothing);
        }

        public void Update()
        {
            // Get current values from VNyan parameters
            // This is only to grab the new values as they are changed, and should only really apply when the plugin window is open.
            trackSmoothActive_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameTrackSmoothActive);
            if (trackSmoothActive_new != trackSmoothActive)
            {
                trackSmoothActive = trackSmoothActive_new;
                TrackingSmoothSettings.setSmoothLayerOnOff(trackSmoothActive);
            }

            // Eyes
            trackEyesActive_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameTrackEyesActive);
            if (trackEyesActive_new != trackEyesActive)
            {
                trackEyesActive = trackEyesActive_new;
                TrackingSmoothSettings.setEyesLayerOnOff(trackEyesActive);
            }
            eyesmoothSpeed_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameEyeSmoothSpeed);
            if (eyesmoothSpeed_new != eyesmoothSpeed)
            {
                eyesmoothSpeed = eyesmoothSpeed_new;
                TrackingSmoothSettings.setEyeSmoothSpeed(eyesmoothSpeed);
            }
            eyeBlinkThreshold_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameEyeBlinkThreshold);
            if (eyeBlinkThreshold_new != eyeBlinkThreshold)
            {
                eyeBlinkThreshold = eyeBlinkThreshold_new;
                TrackingSmoothSettings.setBlendshapeBlinkThreshold(eyeBlinkThreshold);
            }
            eyeJitterRemoval_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameEyeJitterRemoval);
            if (eyeJitterRemoval_new != eyeJitterRemoval)
            {
                eyeJitterRemoval = eyeJitterRemoval_new;
                TrackingSmoothSettings.setEyeJitterRemoval(eyeJitterRemoval);
            }
            eyeAngleSpeed_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameEyeAngleSpeed);
            if (eyeAngleSpeed_new != eyeAngleSpeed)
            {
                eyeAngleSpeed = eyeAngleSpeed_new;
                TrackingSmoothSettings.setEyeAngleSpeed(eyeAngleSpeed);
            }

            trackBodyActive_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameTrackBodyActive);
            if (trackBodyActive_new != trackBodyActive)
            {
                trackBodyActive = trackBodyActive_new;
                TrackingSmoothSettings.setBodyLayerOnOff(trackBodyActive);
            }
            smoothSpeed_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameSmoothSpeed);
            if (smoothSpeed_new != smoothSpeed)
            {
                smoothSpeed = smoothSpeed_new;
                TrackingSmoothSettings.setSmoothSpeed(smoothSpeed);
            }
            angleSpeed_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(paramNameAngleSpeed);
            if (angleSpeed_new != angleSpeed)
            {
                angleSpeed = angleSpeed_new;
                TrackingSmoothSettings.setAngleSpeed(angleSpeed);
            }
        }
    }
}