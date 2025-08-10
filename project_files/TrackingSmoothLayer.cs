using System;
using System.Collections.Generic;
using LZQuaternions;
using UnityEngine;
using VNyanInterface;
using static UnityEngine.GraphicsBuffer;

namespace TrackingSmoothLayer
{
    public class TrackingSmoothSettings
    {
        private bool SmoothLayerActive = true;

        private static List<int> TrackingSmoothingBones = new List<int> {21, 22, 10, 0, 7, 8, 9 };
        private List<int> TrackingSmoothingEyeBones = new List<int> { 21, 22 };
        private List<int> TrackingSmoothingBodyBones = new List<int> { 10, 0, 7, 8, 9 };

        private static Dictionary<int, VNyanQuaternion> createNewBonesDictonary(List<int> boneList)
        {
            Dictionary<int, VNyanQuaternion> rotationDictionary = new Dictionary<int, VNyanQuaternion>();
            foreach (var bone in boneList)
            {
                rotationDictionary.Add(bone, new VNyanQuaternion { });
            }
            return rotationDictionary;
        }

        private static Dictionary<int, VNyanQuaternion> TrackingSmoothCurrent = createNewBonesDictonary(TrackingSmoothingBones);
        private static Dictionary<int, VNyanQuaternion> TrackingSmoothTarget = createNewBonesDictonary(TrackingSmoothingBones);

        public List<int> getBodyBones() => TrackingSmoothingBodyBones;
        public List<int> getEyeBones() => TrackingSmoothingEyeBones;
        public VNyanQuaternion getCurrentBone(int boneNum) => TrackingSmoothCurrent[boneNum];
        public VNyanQuaternion getTargetBone(int boneNum) => TrackingSmoothTarget[boneNum];
        public Dictionary<int, VNyanQuaternion> getCurrentBoneDict() => TrackingSmoothCurrent;


        public void setCurrentBone(int boneNum, VNyanQuaternion bone)
        {
            TrackingSmoothCurrent[boneNum] = bone;
        }
        public void setTargetBone(int boneNum, VNyanQuaternion bone)
        {
            TrackingSmoothTarget[boneNum] = bone;
        }

        private float bodySmoothing = 10.0f;
        private float bodyBoost = 25f;

        private float eyeSmoothing = 10.0f;
        private float eyeBoost = 25f;
        private float blendshapeBlinkThreshold = 50f;

        public void setSmoothLayerOnOff(float val) => SmoothLayerActive = (val == 1f) ? true : false;
        public bool getSmoothLayerActive() => SmoothLayerActive;

        /// <summary>
        /// Rescale the settings range and inverting the diretion
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float rescaleInvertSpeed(float value)
        {
            // newMax * (1 - value / oldMax);
            return 10f * (1 - value / 100f);
        }

        public void setBodyBoost(float val)
        {
            bodyBoost = val / 10f;
        }
        public void setEyeBoost(float val)
        {
            eyeBoost = val / 10f;
        }
        
        public void setBodySmoothing(float val)
        {
            bodySmoothing = rescaleInvertSpeed(val);
        }

        public void setEyeSmoothing(float val)
        {
            eyeSmoothing = rescaleInvertSpeed(val);
        }

        public void setBlendshapeBlinkThreshold(float val)
        {
            blendshapeBlinkThreshold = val;
        }

        public float getBodySmoothing() => bodySmoothing;
        public float getEyeSmoothing() => eyeSmoothing;
        public float getEyeBoost() => eyeBoost;
        public float getBodyBoost() => bodyBoost;
        public float getBlinkThreshold() => blendshapeBlinkThreshold;

        /// <summary>
        /// Updates the bones in the Target dictionaries.
        /// </summary>
        public void updateRotationsTarget(Dictionary<int, VNyanQuaternion> bones, List<int> boneList)
        {
            foreach (int boneNum in boneList)
            {
                setTargetBone(boneNum, bones[boneNum]);
            }
        }

        /// <summary>
        /// Calculates a multiplier based on the angle between two quaternions and a scale.
        /// </summary>
        /// <param name="qFrom"></param>
        /// <param name="qTo"></param>
        /// <param name="adaptiveScale"></param>
        /// <returns></returns>
        public float setAdaptiveAngle(Quaternion qFrom, Quaternion qTo, float angleScale)
        {
            return angleScale * Quaternion.Angle(qFrom, qTo);
        }

        /// <summary>
        /// Applies Quaternion Slerp method, linearly scaling the slerp amount by the angle between the current and target bones.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="slerpAmount"></param>
        /// <param name="adaptiveScale"></param>
        /// <returns></returns>
        public VNyanQuaternion adaptiveSlerp(VNyanQuaternion current, VNyanQuaternion target, float slerpAmount, float angleScale)
        {
            Quaternion currentUnityQ = QuaternionMethods.convertQuaternionV2U(current);
            Quaternion targetUnityQ = QuaternionMethods.convertQuaternionV2U(target);

            float angleSpeed = setAdaptiveAngle(currentUnityQ, targetUnityQ, angleScale);

            return QuaternionMethods.convertQuaternionU2V(Quaternion.Slerp(currentUnityQ, targetUnityQ, (slerpAmount + angleSpeed) * Time.deltaTime));
        }
    }

    public class TrackSmoothLayer : IPoseLayer
    {
        // Set up our frame-by-frame information
        public PoseLayerFrame TrackSmoothFrame = new PoseLayerFrame();

        public TrackingSmoothSettings TrackingSmoothSettings = new TrackingSmoothSettings();

        // Create containers to load pose data each frame
        public Dictionary<int, VNyanQuaternion> BoneRotations;
        public Dictionary<int, VNyanVector3> BonePositions;
        public Dictionary<int, VNyanVector3> BoneScales;
        public VNyanVector3 RootPos;
        public VNyanQuaternion RootRot;

        public Quaternion eyeBoneCurrent;
        public float angleBetweenEyeBones;
        public float angleSpeed;

        public Quaternion headBoneCurrent;
        public float angleBetweenHeadBones;
        public float angleSpeedHead;

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
            return TrackingSmoothSettings.getSmoothLayerActive();
        }

        public TrackingSmoothSettings getLayerSettings() => TrackingSmoothSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boneList"></param>
        /// <param name="slerpAmount"></param>
        /// <param name="adaptiveAmount"></param>
        public void processBoneRotations(List<int> boneList, float slerpAmount, float adaptiveAmount)
        {
            foreach (int boneNum in boneList)
            {
                if (BoneRotations.TryGetValue(boneNum, out VNyanQuaternion vnyanCurrent))
                {
                    // 1. Update target bone
                    TrackingSmoothSettings.setTargetBone(boneNum, vnyanCurrent);
                    // 2. Rotate current towards target
                    rotateTowardsTarget(boneNum, slerpAmount, adaptiveAmount);
                }
            }
        }

        /// <summary>
        /// Updates VNyan's BoneRotations dictionary with new rotations.
        /// </summary>
        /// <param name="newRotations"></param>
        public void updateBoneRotations(Dictionary<int, VNyanQuaternion> newRotations, List<int> boneList)
        {
            foreach (int boneNum in boneList)
            {
                if (BoneRotations.ContainsKey(boneNum))
                {
                    BoneRotations[boneNum] = newRotations[boneNum];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boneNum"></param>
        /// <param name="slerpAmount"></param>
        /// <param name="adaptiveAmount"></param>
        public void rotateTowardsTarget(int boneNum, float slerpAmount, float angleScale)
        {
            VNyanQuaternion target = TrackingSmoothSettings.getTargetBone(boneNum);
            VNyanQuaternion current = TrackingSmoothSettings.getCurrentBone(boneNum);

            if (!(current == target))
            {
                Quaternion target_U = QuaternionMethods.convertQuaternionV2U(target);
                Quaternion current_U = QuaternionMethods.convertQuaternionV2U(current);

                TrackingSmoothSettings.setCurrentBone(boneNum, TrackingSmoothSettings.adaptiveSlerp(current, target, slerpAmount, angleScale));
            }
        }

        public void doUpdate(in PoseLayerFrame TrackSmoothFrame)
        {
            BoneRotations = TrackSmoothFrame.BoneRotation;
            BonePositions = TrackSmoothFrame.BonePosition;
            BoneScales = TrackSmoothFrame.BoneScaleMultiplier;
            RootPos = TrackSmoothFrame.RootPosition;
            RootRot = TrackSmoothFrame.RootRotation;

            // Eyes
            if (TrackingSmoothSettings.getEyeSmoothing() < 10f)
            {
                if (TrackingSmoothSettings.getBlinkThreshold() != 0)
                {
                    blendshape_eyeBlinkLeft = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("eyeBlinkLeft") * 100;
                    blendshape_eyeBlinkRight = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("eyeBlinkRight") * 100;
                    if ((blendshape_eyeBlinkLeft < TrackingSmoothSettings.getBlinkThreshold()) || (blendshape_eyeBlinkRight < TrackingSmoothSettings.getBlinkThreshold()))
                    {
                        processBoneRotations(TrackingSmoothSettings.getEyeBones(), TrackingSmoothSettings.getEyeSmoothing(), TrackingSmoothSettings.getEyeBoost());
                    }
                } 
                else
                {
                    processBoneRotations(TrackingSmoothSettings.getEyeBones(), TrackingSmoothSettings.getEyeSmoothing(), TrackingSmoothSettings.getEyeBoost());
                }
                updateBoneRotations(TrackingSmoothSettings.getCurrentBoneDict(), TrackingSmoothSettings.getEyeBones());
            }

            // Body
            if (TrackingSmoothSettings.getBodySmoothing() < 10f)
            {
                processBoneRotations(TrackingSmoothSettings.getBodyBones(), TrackingSmoothSettings.getBodySmoothing(), TrackingSmoothSettings.getBodyBoost());
                updateBoneRotations(TrackingSmoothSettings.getCurrentBoneDict(), TrackingSmoothSettings.getBodyBones());
            }
        }
    }
}