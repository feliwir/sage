# SAGE-Engine

This project is trying to reimplement the functionality from the SAGE (Strategy Action Game Engine) written by EA. The reason for this is that the current engine is not working very well on modern CPUs since has many technical limitations

# Build status
[![Build status](https://ci.appveyor.com/api/projects/status/f4re4pcqyr5g0naf?svg=true)](https://ci.appveyor.com/project/feliwir/sage)

# Components
- BIG:
    * Big files are the archives used inside the SAGE Engine, it stores most data. It is the most 
    basic component
- W3D:
    * The 3D format used inside the SAGE Engine. The full name is Westwood 3D, which was the game studio who developed the predecessor W3D-Engine (used for Command and Conquer and others).
- APT:
    * Apt files are used for the GUI inside the engine. The Apt files are mostly identical to SWF files, which are flash files.
    The format was added to the engine by EA and is used for many other games aswell (Need for Speed, Madden NFL etc.).
- VP6:
    * The video format used inside the SAGE Engine. It was the default codec for Flash at the time (flash is used inside the Engine for UI, see APT)

