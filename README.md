# OpenModelVisualization
FMU based Visualzation features

This repository contains:

3rdParty:
dependencies for the project, i.e. the FMILibrary by Modelon, it includes headers and binaries for msvc x86 and x64

FMUSimulator:
a library that wraps the FMILibrary but provides a special interface that fits the Unity project and is less chatty

FMU_UnityPlugin:
this contains editor script and the behaviour scripts to run the FMU-visualization, note that the dlls have to be in the project directoy
