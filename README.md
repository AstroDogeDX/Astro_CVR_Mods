# Astro's ChilloutVR Mods

A collection of small, random mods made by AstroDoge.

---

## [ScrollZoom](ScrollZoom)

*A rather simple mod to control zoom level on desktop using the scroll wheel!*

Additional options include switching from 'hold' to 'toggle' for zooming, the maximum zoom distance and amount of zoom per wheel turn.
Current zoom level is passed to CVR's parameter stream as `Zoom Factor` - so could be used to drive animations via Motion Time!

---

## [TurnKeys](TurnKeys)

*A super niche mod to turn using the keyboard in desktop mode.*

You can now turn using Q/E without needing to touch the mouse and... that's all!

---

## [BringMeBoost](BringMeBoost)

*A quick 'n' dirty fix for missing a missing input.*

Adds the missing VR input for `Input Car Boost` making the parameter stream actually useful.

---

## Building

In order to build this project follow the instructions (thanks [@Daky](https://github.com/dakyneko) & [@kafeijao](https://github.com/kafeijao)!):

- (1) Install `NStrip.exe` from https://github.com/BepInEx/NStrip into this directory (or into your PATH). This tools
  converts all assembly symbols to public ones! If you don't strip the dlls, you won't be able to compile some mods.
- (2) Follow the instructions below to [Set CVR Folder Environment Variable](#set-cvr-folder-environment-variable)
- (3) Run `copy_and_nstrip_dll.ps1` on the Power Shell. This will copy the required CVR, MelonLoader, and Mod DLLs into
  this project's `/ManagedLibs`. Note if some of the required mods are not found, it will display the url from the CVR
  Modding Group API so you can download.

### Set CVR Folder Environment Variable

To build the project you need `CVRPATH` to be set to your ChilloutVR Folder, so we get the path to grab the libraries 
we need to compile. By running the `copy_and_nstrip_dll.ps1` script that env variable is set automatically, but only
works if the ChilloutVR folder is at the location `D:\SteamLibrary\steamapps\common\ChilloutVR`.

Otherwise you need to set the `CVRPATH` env variable yourself, you can do that by either updating the default path in
the `copy_and_nstrip_dll.ps1` and then run it, or manually set it via the windows menus.


#### Setup via editing copy_and_nstrip_dll.ps1

Edit `copy_and_nstrip_dll.ps1` and look the line bellow, and then replace the Path with your actual path.

```$cvrDefaultPath = "D:\SteamLibrary\steamapps\common\ChilloutVR"```

Now you're all set and you can go to the step (3) of the [Building](#building) instructions!


#### Setup via Windows menus

In Windows Start Menu, search for `Edit environment variables for your account`, and click `New` on the top panel.
Now you input `CVRPATH` for the **Variable name**, and the location of your ChilloutVR folder as the **Variable value**

By default this value would be `C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR`. 
Make sure it points to the folder where your `ChilloutVR.exe` is located.

Now you're all set and you can go to the step (3) of the [Building](#building) instructions! If you already had a power
shell window opened, you need to close and open again, so it refreshes the Environment Variables.

---

# Disclosure  

Here is the block of text where I tell you this mod is not affiliated or endorsed by ABI.

https://documentation.abinteractive.net/official/legal/tos/#7-modding-our-games

> This mod is an independent creation and is not affiliated with, supported by or approved by Alpha Blend Interactive. 

> Use of this mod is done so at the user's own risk and the creator cannot be held responsible for any issues arising from its use.

> To the best of my knowledge, I have adhered to the Modding Guidelines established by Alpha Blend Interactive.
