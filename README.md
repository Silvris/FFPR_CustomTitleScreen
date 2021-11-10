# FFPR_CustomTitleScreen
A BepInEx dll that allows for the customization of the title screen within the FINAL FANTASY PIXEL REMASTERs.

# Installation:
1. Install a Bleeding-Edge build of BepInEx 6, which can be found [here](https://builds.bepis.io/projects/bepinex_be) or used from a prior Memoria installation
2. Drop the BepInEx folder from the mod into the game's main directory and merge folders if prompted
3. Place custom title screen images into the "plugins/CustomTitleScreen" folder under the name "customTitleScreen.png" to use them, and "customLogo.png" for any custom logos
4. The main menu text colors can be edited by editing the config file that is generated after running the game with the plugin installed, or by using [sinai-dev's BepInExConfigManager](https://github.com/sinai-dev/BepInExConfigManager) while the game is running
5. To play a video (without sound) in the background of the title screen, set UseVideo to True (and reload the title screen if the game is running) and place your video file in the "plugins/CustomTitleScreen" folder under the name "customTitleScreen". 


This mod supports the following video formats (in priority order):
  * mp4
  * mov
  * m4v
  * mpg
  * ogv
  * webm

# Credits:
* The BepInEx Official Discord Server - for answering all of my dumb Unity questions
