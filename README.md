# Aviator Editor
![](https://cdn.discordapp.com/emojis/1212119918133379182.webp)

**Warning: This editor is still in very early development and** ***NOT*** **production-ready.**

Aviator is a Visual Editor for LuaSTG ExPlus, Sub, -x and [Chambersite-K](https://github.com/AtaeKurri/Chambersite-K).
It's aim is to have a simpler codebase to manage and edit than what Sharp and Sharp-X are. (Really no offense but it's a pain to work with this codebase).
It is ***NOT*** meant to replace Sharp X by any means, just another choice (since they basically do the same things, just differently).

A portion of the code is borrowed from [LuaSTG Editor Sharp X](https://github.com/RyannThi/LuaSTG-Editor-Sharp-X/).

Aviator supports both various LuaSTG versions and Chambersite-K (but still very limited by how new the framework is).

## Aviator for LuaSTG

This is the main purpose of Aviator. This is what the editor will compile to by default.
Aviator supports LuaSTG Plus, ExPlus, Sub and -x branches.
You can choose to build your project code and images into folders format or a single zip file.

## Aviator for Chambersite-K

Since Chambersite-K is still a new project, the support for this is really limited.
What Aviator does for CBS is generate a new Visual Studio 2022 compatible solution with the framework and, by default, Boleite compiled with it.
You can, if you want to use your own library, specify a path to a pre-compiled .dll file in the settings.