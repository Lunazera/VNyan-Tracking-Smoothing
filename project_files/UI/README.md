# LZ UI Files
These UI scripts were originally adapted from Sjatar's UI exaples https://github.com/Sjatar

I've modified and developed out using their examples as a starting point. These UI scripts will manager controlling VNyan paramters and saving those parameters to a settings JSON file. They are also all responsive to VNyan's UI and will change colours of all components to match the user's current theme.

These won't talk to the main plugin directly, rather the main Plugin file will check for changes in these parameters to determine when to change settings for the actual plugin.

Probably a better practice would be to establish a standard set of UI scripts and keep that in it's own DLL file, being totally agnostic to the rest of the plugin and bieng able to be interchangeable. I'm not that good at all this yet though :P

A UI should be set up with at least the UI Manager and the Main Window scripts.

### LZUIManager 
This should be in the top of the plugin GameObject hierarchy, outside of the UI window prefab. It'll maintain the settings dictionary, register a button for the plugin in VNyan's menu, and has some methods other UI scripts can access.
### LZMainWindow 
This should attach to the UI window prefab. It'll establish the main visuals of the plugin window.
