#pragma once

#ifdef _USRDLL
#define DLLEXPORT __declspec(dllexport)
#else
#define DLLEXPORT __declspec(dllimport)
#endif

extern "C" {
	DLLEXPORT void* createFMUsimulator(const char* fmuPath, const char* workDir);
	DLLEXPORT int setEulerStepSize(void*simulator, float h);
	DLLEXPORT int simulateToTime(void* simulator, float nextTime);
	DLLEXPORT int getVarRefForVarName(void* simulator, const char* name);
	DLLEXPORT int getRealValuesForVarRefLst(void* simulator, void* varRef, int numVars, void* val);
	DLLEXPORT float getRealValueForVarRef(void* simulator, int varRef);
	DLLEXPORT int initSimulation(void* simulator);
	DLLEXPORT int destroySimulator(void* simulator);
	DLLEXPORT int getFMUVersion(void* simulator);
}