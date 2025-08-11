using System.Collections.Generic;
using LZQuaternions;
using UnityEngine;
using VNyanInterface;

namespace TrackingSmoothLayer
{
    public class TrackingSmoothSettings
    {
        private bool SmoothLayerActive = true;
        private float bodySmoothing = 10.0f;
        private float bodyBoost = 25f;

        private float eyeSmoothing = 10.0f;
        private float eyeBoost = 25f;
        private float blendshapeBlinkThreshold = 50f;

        private static List<int> TrackingSmoothingBones = new List<int> {21, 22, 10, 0, 7, 8, 9 };

        public void setSmoothLayerOnOff(float val) => SmoothLayerActive = (val == 1f) ? true : false;
        public bool getSmoothLayerActive() => SmoothLayerActive;

        /// <summary>
        /// Rescale the settings range and inverting the direction. Is expecting incoming value is between 0 and 100.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float rescaleInvertSpeed(float value, float newScale) => newScale * (1 - value / 100f);

        public void setBodyBoost(float val, float scale) => bodyBoost = val / scale;
        public void setEyeBoost(float val, float scale) => eyeBoost = val / scale;
        public void setBodySmoothing(float val, float scale) => bodySmoothing = rescaleInvertSpeed(val, scale);
        public void setEyeSmoothing(float val, float scale) => eyeSmoothing = rescaleInvertSpeed(val, scale);
        public void setBlendshapeBlinkThreshold(float val) => blendshapeBlinkThreshold = val;
        
        public float getBodySmoothing() => bodySmoothing;
        public float getEyeSmoothing() => eyeSmoothing;
        public float getEyeBoost() => eyeBoost;
        public float getBodyBoost() => bodyBoost;
        public float getBlinkThreshold() => blendshapeBlinkThreshold;


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

        public List<int> getBodyBones() => BoneLists.BodyBones;
        public List<int> getEyeBones() => BoneLists.EyeBones;
        public VNyanQuaternion getCurrentBone(int boneNum) => TrackingSmoothCurrent[boneNum];
        public VNyanQuaternion getTargetBone(int boneNum) => TrackingSmoothTarget[boneNum];
        public Dictionary<int, VNyanQuaternion> getCurrentBoneDict() => TrackingSmoothCurrent;

        public void setCurrentBone(int boneNum, VNyanQuaternion bone) => TrackingSmoothCurrent[boneNum] = bone;
        public void setTargetBone(int boneNum, VNyanQuaternion bone) => TrackingSmoothTarget[boneNum] = bone;

        /// <summary>
        /// Rotates current bone towards target bone using adaptive slerp method.
        /// </summary>
        /// <param name="boneNum">Bone number to get and set</param>
        /// <param name="slerpAmount">Slerp value</param>
        /// <param name="angleScale">Scale for boosting</param>
        public void rotateTowardsTarget(int boneNum, float slerpAmount, float angleScale)
        {
            VNyanQuaternion target = getTargetBone(boneNum);
            VNyanQuaternion current = getCurrentBone(boneNum);

            if (!(current == target))
            {
                Quaternion target_U = QuaternionMethods.convertQuaternionV2U(target);
                Quaternion current_U = QuaternionMethods.convertQuaternionV2U(current);

                setCurrentBone(boneNum, QuaternionMethods.adaptiveSlerp(current, target, slerpAmount, angleScale));
            }
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
        /// Main processing for bone rotations, updating the target bones with current VNyan rotations and then rotating current bones towards them.
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
                    TrackingSmoothSettings.rotateTowardsTarget(boneNum, slerpAmount, adaptiveAmount);
                }
            }
        }

        /// <summary>
        /// Updates VNyan's BoneRotations dictionary with new current rotations.
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