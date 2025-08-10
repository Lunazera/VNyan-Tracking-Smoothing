# VNyan Tracking Smoothing 👀
Applies smoothing to your eyes and head tracking! This can help with reducing noise and jittering.

Set the smoothing slider's up to increase the amount of smoothing applied to your bones. This will also slow down your model, but you can increase the Smooth Boost slider to make up for that! 

![image of plugin window](https://github.com/Lunazera/VNyan-Eye-Smoothing/blob/main/example.png)

## Installation
1. Download the latest zip file from [releases]([https://github.com/Lunazera/VNyan-Tracking-Detection/releases/](https://github.com/Lunazera/VNyan-Eye-Smoothing/releases))
2. Unzip the contents in your VNyan folder. This will put the `.dll` and `.vnobj` inside `Items/Assemblies` for you.
3. The plugin should be present when you load VNyan! (you should see it in the plugin menu)

## How it works
This overwrites your models eyebones and body/neck/chest/hip/spine bone rotations with it's own "smoothed" rotation using VNyan's Pose Layers. It takes the tracked rotations from your face tracking and uses and adaptive slerp method to rotate the bones towards the tracked data every frame, scaled by how big a rotation your tracking tries to make.

### Smoothing
Amount of smoothing to apply. 0 will turn off smoothing.

### Smooth Boost
Makes up for the slowdown caused by smoothing by turning it down automatically for making bigger rotations.

### Blink Threshold
Blinking can throw your eyetracking off for a moment. This setting will make your model's eyes ignore your eyetracking when both your left and right eyeBlink blendshapes are greater than the set value. ie if this is set to 50, then when both your eyeBlinkLeft and eyeBlinkRight blendshapes are above 50, your eye tracking will freeze. Set to 0 to turn off.
