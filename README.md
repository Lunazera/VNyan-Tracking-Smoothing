# LZ's VNyan Eyebone Smoothing 👀
Applies smoothing to your eyebones! Can help with reducing jittering from your face tracking.
Just put in the amount of smoothing you want and hit apply. You might need to try different values to find what works for you. Just know that the higher you set this, the less eye movement you will get overall.

![image of plugin window](https://github.com/Lunazera/VNyan-Eye-Smoothing/blob/main/example.png)

## Installation
1. Download the latest zip file from [releases]([https://github.com/Lunazera/VNyan-Tracking-Detection/releases/](https://github.com/Lunazera/VNyan-Eye-Smoothing/releases))
2. Unzip the contents in your VNyan folder. This will put the `.dll` and `.vnobj` inside `Items/Assemblies` for you.
3. The plugin should be present when you load VNyan! (you should see it in the plugin menu)

## How it works
This overwrites your models eyebone rotations with it's own "smoothed" rotation, using VNyan's Pose Layers. It takes the tracked eye rotations from your face tracking, and uses Unity's RotateTowards method to rotate the new rotation towards the tracked data every frame. 

### Smoothing amount
Smooths out eye bone rotation; the larger the smoothing, the slower your eye tracking will move.
The `smoothing` setting in the plugin goes into this speed value according to `1000/smoothing` https://github.com/Lunazera/VNyan-Eye-Smoothing/blob/52af3deed55e40c0abc007b6b856429fecc9a3a6/project_files/EyeSmoothLayer.cs#L20-L23

### Jitter removal
Sets minimum threshold for your eyes to need to move before eyetracking is applied. This can help remove tiny jitterying in your eye tracking.

### Speed Increase for big eye rotations
If you set your smoothing amount high, you'll find your eyes will move slowly even when making a big change in your eye movement. This setting increases your eye tracking speed when your eyes make these big jumps (this way you could set your smoothing higher, and then set this value high to account for that).

### eyeBlink blendshape Threshold for tracking
Many times blinking can throw your eyetracking off for a moment. This setting ignores your eye tracking when both your left and right eyeBlink blendshapes are greater than the set value. ie if this is set to 50, then when both your eyeBlinkLeft and eyeBlinkRight blendshapes are above 50, your eye tracking will freeze.
