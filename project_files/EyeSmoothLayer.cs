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
        // Eye bones: bone 21, 22
        public static int eyeLeft = 21;
        public static int eyeRight = 22;

        public static float speed = 10.0f;
        public static void setEyeSmoothSpeed(float val)
        {
            speed = (val < 1f) ? 1000 : 1000 * (1 / val);
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
        public float smoothedSpeed = 0f;

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

            // get current Eye Bone rotations as Unity Quat

            eyeBoneCurrent = VNyanExtra.QuaternionMethods.convertQuaternionV2U(BoneRotations[EyeSmoothSettings.eyeRight]);
            EyeSmoothSettings.eyeBones = Quaternion.RotateTowards(EyeSmoothSettings.eyeBones, eyeBoneCurrent, EyeSmoothSettings.speed * Time.deltaTime);

            // Write to VNyan BoneRotation
            BoneRotations[EyeSmoothSettings.eyeLeft] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(EyeSmoothSettings.eyeBones);
            BoneRotations[EyeSmoothSettings.eyeRight] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(EyeSmoothSettings.eyeBones);
        }
    }
}