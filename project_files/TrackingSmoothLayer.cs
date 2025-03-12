using System;
using VNyanInterface;
using UnityEngine;
using VNyanExtra;
using System.Collections.Generic;

namespace TrackingSmoothLayer
{
    public class TrackingSmoothSettings
    {
        public static bool SmoothLayerActive = true;
        public static bool EyesLayerActive = true;
        public static bool BodyLayerActive = true;
        public static void setSmoothLayerOnOff(float val) => SmoothLayerActive = (val == 1f) ? true : false;
        public static void setEyesLayerOnOff(float val) => EyesLayerActive = (val == 1f) ? true : false;

        public static void setBodyLayerOnOff(float val) => BodyLayerActive = (val == 1f) ? true : false;

        public static Quaternion eyeBones;
        public static Quaternion eyeBoneRotateTowards;

        public static Quaternion headBone;
        public static Quaternion headBoneRotateTowards;

        public static Quaternion hipBone;
        public static Quaternion hipBoneRotateTowards;
        public static Quaternion spineBone;
        public static Quaternion spineBoneRotateTowards;
        public static Quaternion chestBone;
        public static Quaternion chestBoneRotateTowards;
        public static Quaternion neckBone;
        public static Quaternion neckBoneRotateTowards;
        public static Quaternion upperChestBone;
        public static Quaternion upperChestBoneRotateTowards;

        // Do we need to make reset methods for when toggling on/off?

        // Bone definitions
        // Eye bones: bone 21, 22
        public static int eyeLeft = 21;
        public static int eyeRight = 22;
        public static int head = 10;
        public static int hips = 0;
        public static int spine = 7;
        public static int chest = 8;
        public static int neck = 9;

        public static float speed = 10.0f;
        public static float angleSpeed = 25f;

        public static float eyespeed = 10.0f;
        public static float eyeAngleSpeed = 25f;
        public static float eyeJitterRemoval = 0f;
        public static float blendshapeBlinkThreshold = 50f;

        public static void setAngleSpeed(float val)
        {
            angleSpeed = val/10;
        }
        public static void setEyeAngleSpeed(float val)
        {
            eyeAngleSpeed = val/10;
        }
        public static void setBlendshapeBlinkThreshold(float val)
        {
            blendshapeBlinkThreshold = val;
        }
        public static void setEyeJitterRemoval(float val)
        {
            eyeJitterRemoval = val;
        }


        public static void setSmoothSpeed(float val)
        {
            // Inverted speed value, so that as user increases the "Smooth" setting which calls 'setEyeSmoothSpeed', this number goes from 360 (effectively no smoothing) to 0 (max smoothing)
            // Clamped, so that val cannot be evaluated below 1
            //speed = (val < 1f) ? 1000 : 1000 * (1 / val);
            speed = val/10;
        }
        public static void setEyeSmoothSpeed(float val)
        {
            eyespeed = val/10;
        }

        public static float setSmoothAngleAdjustment(Quaternion qFrom, Quaternion qTo)
        {
            // Large rotation differences get a "boost" through this
            float anglescaled = angleSpeed * Quaternion.Angle(qFrom, qTo)/15;

            return anglescaled;
        }
    }

    public class TrackSmoothLayer : IPoseLayer
    {
        // Set up our frame-by-frame information
        public PoseLayerFrame TrackSmoothFrame = new PoseLayerFrame();

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
            return TrackingSmoothSettings.SmoothLayerActive;
        }

        bool isEyesActive()
        {
            return TrackingSmoothSettings.EyesLayerActive;
        }
        bool isBodyActive()
        {
            return TrackingSmoothSettings.BodyLayerActive;
        }

        Quaternion AdaptiveTrackingSmoothing(Quaternion smoothedBone, VNyanQuaternion trackingBone, Quaternion rotateTowardsBone, float slerpAmount, float angleSpeedScale, float jitter = 0f)
        {
            // uses spherical linear interpolation to smooth out rotations, scaled by deltatime
            Quaternion trackingBoneCurrent = VNyanExtra.QuaternionMethods.convertQuaternionV2U(trackingBone);            

            // Don't bother with smoothing if the bones are the same rotation already.
            if (rotateTowardsBone != smoothedBone)
            {
                float angleBetweenBones = Quaternion.Angle(smoothedBone, rotateTowardsBone);

                // We are going to max out the angle speed increase at 50, this is additive onto the slerp amount
                //
                float angleSpeed = angleSpeedScale * angleBetweenBones / 50;
                if (angleSpeed >= 50)
                {
                    angleSpeed = 50;
                }
                if (angleSpeed <= 0)
                {
                    angleSpeed = 0;
                }

                if (jitter != 0)
                {
                    if (angleBetweenBones > jitter)
                    {
                        rotateTowardsBone = trackingBoneCurrent;
                    }
                }
                else
                {
                    rotateTowardsBone = trackingBoneCurrent;
                }

                smoothedBone = Quaternion.Slerp(smoothedBone, rotateTowardsBone, (slerpAmount + angleSpeed) * Time.deltaTime);
            }
               return smoothedBone;
        }

        public void doUpdate(in PoseLayerFrame TrackSmoothFrame)
        {
            BoneRotations = TrackSmoothFrame.BoneRotation;
            BonePositions = TrackSmoothFrame.BonePosition;
            BoneScales = TrackSmoothFrame.BoneScaleMultiplier;
            RootPos = TrackSmoothFrame.RootPosition;
            RootRot = TrackSmoothFrame.RootRotation;

            // Eyes
            if (isEyesActive() == true)
            {
                eyeBoneCurrent = VNyanExtra.QuaternionMethods.convertQuaternionV2U(BoneRotations[TrackingSmoothSettings.eyeRight]);

                // If we are using the blink threshold setting, then read blink blendshapes and apply tracking when blinking is below
                if (TrackingSmoothSettings.blendshapeBlinkThreshold != 0)
                {
                    blendshape_eyeBlinkLeft = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("eyeBlinkLeft") * 100;
                    blendshape_eyeBlinkRight = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("eyeBlinkRight") * 100;
                    if ((blendshape_eyeBlinkLeft < TrackingSmoothSettings.blendshapeBlinkThreshold) || (blendshape_eyeBlinkRight < TrackingSmoothSettings.blendshapeBlinkThreshold))
                    {
                        TrackingSmoothSettings.eyeBones = AdaptiveTrackingSmoothing(TrackingSmoothSettings.eyeBones, BoneRotations[TrackingSmoothSettings.eyeRight], TrackingSmoothSettings.eyeBoneRotateTowards, TrackingSmoothSettings.eyespeed, TrackingSmoothSettings.eyeAngleSpeed, TrackingSmoothSettings.eyeJitterRemoval);
                    }
                } 
                else
                {
                    TrackingSmoothSettings.eyeBones = AdaptiveTrackingSmoothing(TrackingSmoothSettings.eyeBones, BoneRotations[TrackingSmoothSettings.eyeRight], TrackingSmoothSettings.eyeBoneRotateTowards, TrackingSmoothSettings.eyespeed, TrackingSmoothSettings.eyeAngleSpeed, TrackingSmoothSettings.eyeJitterRemoval);
                }

                // Write to VNyan eye bones
                BoneRotations[TrackingSmoothSettings.eyeLeft] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(TrackingSmoothSettings.eyeBones);
                BoneRotations[TrackingSmoothSettings.eyeRight] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(TrackingSmoothSettings.eyeBones);
            }

            // Body
            if (isBodyActive() == true)
            {
                // Smooth out body parts controlled directly by tracking
                TrackingSmoothSettings.headBone = AdaptiveTrackingSmoothing(TrackingSmoothSettings.headBone, BoneRotations[TrackingSmoothSettings.head], TrackingSmoothSettings.headBoneRotateTowards, TrackingSmoothSettings.speed, TrackingSmoothSettings.angleSpeed);
                TrackingSmoothSettings.hipBone = AdaptiveTrackingSmoothing(TrackingSmoothSettings.hipBone, BoneRotations[TrackingSmoothSettings.hips], TrackingSmoothSettings.hipBoneRotateTowards, TrackingSmoothSettings.speed, TrackingSmoothSettings.angleSpeed);
                TrackingSmoothSettings.spineBone = AdaptiveTrackingSmoothing(TrackingSmoothSettings.spineBone, BoneRotations[TrackingSmoothSettings.spine], TrackingSmoothSettings.spineBoneRotateTowards, TrackingSmoothSettings.speed, TrackingSmoothSettings.angleSpeed);
                TrackingSmoothSettings.chestBone = AdaptiveTrackingSmoothing(TrackingSmoothSettings.chestBone, BoneRotations[TrackingSmoothSettings.chest], TrackingSmoothSettings.chestBoneRotateTowards, TrackingSmoothSettings.speed, TrackingSmoothSettings.angleSpeed);
                TrackingSmoothSettings.neckBone = AdaptiveTrackingSmoothing(TrackingSmoothSettings.neckBone, BoneRotations[TrackingSmoothSettings.neck], TrackingSmoothSettings.neckBoneRotateTowards, TrackingSmoothSettings.speed, TrackingSmoothSettings.angleSpeed);

                // Write to VNyan BoneRotation
                BoneRotations[TrackingSmoothSettings.head] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(TrackingSmoothSettings.headBone);
                BoneRotations[TrackingSmoothSettings.hips] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(TrackingSmoothSettings.hipBone);
                BoneRotations[TrackingSmoothSettings.spine] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(TrackingSmoothSettings.spineBone);
                BoneRotations[TrackingSmoothSettings.chest] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(TrackingSmoothSettings.chestBone);
                BoneRotations[TrackingSmoothSettings.neck] = VNyanExtra.QuaternionMethods.convertQuaternionU2V(TrackingSmoothSettings.neckBone);
            }
        }
    }
}