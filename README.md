# JLL (Jacob's Lethal Libraries)
This is a BepinEx library mod for the game Lethal Company that was intended for personal use for re-usable code between several of my mods.

As time has gone on, many creators in the game's community have reached out to me for feature requests, and it is now used by a lot of community creators.

## Bug Reports?
Please submit bug reports under the issues tab on this GitHub page.

## For Developers
If you plan on using JLL within your Lethal Company project, almost all of the features can be used without code by adding components to your game objects within the editor.

Take all the DLL files found within the zip file you downloaded from [Thunderstore](https://thunderstore.io/c/lethal-company/p/JacobG5/JLL/) and put them within your project's Assets\LethalCompany\Tools\Plugins\BepInEx` folder.

It is also recommended to download the `JLLEditorModule.dll` file found on this page. It is technically optional, but it adds warnings for common mistakes and helpful gizmos within the Unity Editor.

Once your dlls are placed within the folder mentioned, make sure to select them all and **uncheck** `Validate References` at the top.

JLL contains compatibility features for several other community mods. If you do not **turn off** `Validate References`, the editor will throw errors on code compilation. 

JLL is designed to run safely without these other mods present, but the Unity Editor doesn't have a way to understand that.

## Further Help?
Feel free to reach out to me about any of my mods in their respective threads in either:

[The Official Lethal Company Modding Discord](https://discord.gg/XeyYqRdRGC) 

My public Discord [Jacob's Portal](https://discord.gg/MdRVdAY)
