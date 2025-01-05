# LZ's VNyan Eyebone Smoothing 👀
Applies smoothing to your eyebones! Can help with reducing jittering from your face tracking.
Just put in the amount of smoothing you want and hit apply. You might need to try different values to find what works for you. Just know that the higher you set this, the less eye movement you will get overall.

![image of plugin window](https://github.com/Lunazera/VNyan-Eye-Smoothing/blob/main/example.png)

## How to install

### How it works
This overwrites your models eyebone rotations with it's own "smoothed" rotation, using VNyan's Pose Layers. It takes the tracked eye rotations from your face tracking, and uses Unity's RotateTowards method to rotate the new rotation towards the tracked data every frame. The speed of this determines how quickly it catches up to the tracked data.
The `smoothing` setting in the plugin goes into this speed value according to `1000/smoothing` https://github.com/Lunazera/VNyan-Eye-Smoothing/blob/c2204b29a9d8827ddae6edc06754bfd4632aa890/project_files/EyeSmoothLayer.cs#L21
