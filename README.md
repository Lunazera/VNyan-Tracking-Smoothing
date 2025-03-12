# VNyan Tracking Smoothing 👀
Applies smoothing to your eyes and head tracking! This can help with reducing jittering from your face tracking.
Set the base tracking speed and an acceleration. The acceleration will speed up your model's movement when you are making large movements. That way, you can have a lot of smoothing applied and now have your model slow down completely.
You can set whether you want this to apply to only your eyes or your body (head, neck, chest, hips, and spine).

![image of plugin window](https://github.com/Lunazera/VNyan-Eye-Smoothing/blob/main/example.png)

## Installation
1. Download the latest zip file from [releases]([https://github.com/Lunazera/VNyan-Tracking-Detection/releases/](https://github.com/Lunazera/VNyan-Eye-Smoothing/releases))
2. Unzip the contents in your VNyan folder. This will put the `.dll` and `.vnobj` inside `Items/Assemblies` for you.
3. The plugin should be present when you load VNyan! (you should see it in the plugin menu)

## How it works
This overwrites your models eyebones and body/neck/chest/hip/spine bone rotations with it's own "smoothed" rotation, using VNyan's Pose Layers. It takes the tracked rotations from your face tracking and uses Unity's slerp method to rotate the bones towards the tracked data every frame. 

### Speed
Base speed of your tracking; the smaller the value, the slower and smoother your tracking will be.

### Acceleration
If you set your speed to low, you'll find your model will move slowly even when you're making big movements irl. This setting increases your model's bone speeds when making these big jumps. This way you can set your speed lower (more smoothing) and then set this value higher to account for that.

### Jitter removal
Sets minimum threshold your eyes need to move irl before the model will move. This can help remove tiny jitterying in your eye tracking, set this very low or to 0 to turn off.

### Blink Threshold
Blinking can throw your eyetracking off for a moment. This setting will make your model's eyes ignore your eyetracking when both your left and right eyeBlink blendshapes are greater than the set value. ie if this is set to 50, then when both your eyeBlinkLeft and eyeBlinkRight blendshapes are above 50, your eye tracking will freeze. Set to 0 to turn off.
