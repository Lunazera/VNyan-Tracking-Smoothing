using System;
using VNyanInterface;
using UnityEngine;
using VNyanExtra;
using System.Collections.Generic;

namespace EyeSmoothLayer
{
    public class EyeSmoothSettings
    {
        public static bool EyeSmoothLayerActive = true;
        public static void setEyeSmoothLayerOnOff(float val) => EyeSmoothLayerActive = (val == 1f) ? true : false;

        public static Quaternion eyeBones;
        public static Quaternion eyeBoneRotateTowards;

        // Eye bones: bone 21, 22
        public static int eyeLeft = 21;
        public static int eyeRight = 22;

        public static float speed = 10.0f;
        public static float angleSpeed = 80f;
        public static float jitter_removal = 0.01f;
        public static float blendshapeBlinkThreshold = 50f;

        public static void setJitterRemoval(float val)
        {
            jitter_removal = val;
        }

        public static void setAngleSpeed(float val)
        {
            angleSpeed = val;
        }
        public static void setBlendshapeBlinkThreshold(float val)
        {
            blendshapeBlinkThreshold = val;
        }

        public static void setEyeSmoothSpeed(float val)
        {
            // Inverted speed value, so that as user increases the "Smooth" setting which calls 'setEyeSmoothSpeed', this number goes from 360 (effectively no smoothing) to 0 (max smoothing)
            // Clamped, so that val cannot be evaluated below 1
            speed = (val < 1f) ? 1000 : 1000 * (1 / val);
        }

        public static float setEyeSmoothAngleAdjustment(Quaternion qFrom, Quaternion qTo)
        {
            // Large rotation differences get a "boost" through this
            float anglescaled = angleSpeed * Quaternion.Angle(qFrom, qTo)/15;

            return anglescaled;
        }
    }

    public class EyeSmoothLayer : IPoseLayer
    {
        // Set up our frame-by-frame information
        public PoseLayerFrame EyeSmoothFrame = new PoseLayerFrame();

        // Create containers to load pose data each frame
        public Dictionary<int, VNyanQuaternion> BoneRotations;
        public Dictionary<int, VNyanVector3> BonePositions;
        public Dictionary<int, VNyanVector3> BoneScales;
        public VNyanVector3 RootPos;
        public VNyanQuaternion RootRot;

        public Quaternion eyeBoneCurrent;
        public float angleBetweenEyeBones;
        public float angleSpeed;

        public float blendshape_eyeBlinkLeft;
        public float blendshape_eyeBlinkRight;

        // VNyan Get Methods, VNyan uses these to get the pose after doUpdate()
        VNyanVector3 IPoseLayer.getBonePosition(int i)
        {
            return BonePositions[i];
        }
        VNyanQuaternion IPoseLayer.getBoneRotation(int i)
        {
            return BoneRotations[i];
        }
        VNyanVector3 IPoseLayer.getBoneScaleMultiplier(int i)
        {
            return BoneScales[i];
        }
        VNyanVector3 IPoseLayer.getRootPosition()
        {
            return RootPos;
        }
        VNyanQuaternion IPoseLayer.getRootRotation()
        {
            return RootRot;
        }
        bool IPoseLayer.isActive()
        {
            return EyeSmoothSettings.EyeSmoothLayerActive;
        }
        public void doUpdate(in PoseLayerFrame EyeSmoothFrame)
        {
            BoneRotations = EyeSmoothFrame.BoneRotation;
            BonePositions = EyeSmoothFrame.BonePosition;
            BoneScales = EyeSmoothFrame.BoneScaleMultiplier;
            RootPos = EyeSmoothFrame.RootPosition;
            RootRot = EyeSmoothFrame.RootRotation;

            eyeBoneCurrent = VNyanExtra.QuaternionMethods.convertQuaternionV2U(BoneRotations[EyeSmoothSettings.eyeRight]);
            angleBetweenEyeBones = Quaternion.Angle(EyeSmoothSettings.eyeBones, eyeBoneCurrent);

            if (angleBetweenEyeBones >= EyeSmoothSettings.jitter_removal)
            {
                blendshape_eyeBlinkLeft = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("eyeBlinkLeft")*100;
                blendshape_eyeBlinkRight = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("eyeBlinkRight")*100;
                if ((blendshape_eyeBlinkLeft < EyeSmoothSettings.blendshapeBlinkThreshold) || (blendshape_eyeBlinkRight < EyeSmoothSettings.blendshapeBlinkThreshold))
                {
                    EyeSmoothSettings.eyeBoneRotateTowards = eyeBoneCurrent;
                }
            }

            angleSpeed = EyeSmoothSettings.setEyeSmoothAngleAdjustment(EyeSmoothSettings.eyeBones, EyeSmoothSettings.eyeBoneRotateTowards);
            EyeSmoothSettings.eyeBones = Quaternion.RotateTowards(EyeSmoothSettings.eyeBones, EyeSmoothSettings.eyeBoneRotateTowards, (EyeSmoothSettings.speed + angleSpeed) * Time.deltaTime);

            // Write to VNyan BoneRotation
            BoneRotations[EyeSmoothSettings.eyeLeft] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(EyeSmoothSettings.eyeBones);
            BoneRotations[EyeSmoothSettings.eyeRight] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(EyeSmoothSettings.eyeBones);

        }
    }
}