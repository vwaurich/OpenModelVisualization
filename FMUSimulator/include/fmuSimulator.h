#pragma once

#include <stdio.h>
#include <stdlib.h>
#include <stdarg.h>
#include <fmilib.h>
#include <string>
#include <iostream>
#include <fstream>

#include "util.h"
class simulatorAPI
{

public:
	simulatorAPI();
	~simulatorAPI();
	int loadFMU(const char* path, const char* file);
	int initFMU();
	int simulateFMU(float nextTime);
	void do_event_iteration(fmi2_import_t *fmu, fmi2_event_info_t *eventInfo);

	int fmuVersion;
	fmi2_callback_functions_t callBackFunctions;
	jm_callbacks callbacks;
	fmi_import_context_t* context;
	fmi_version_enu_t version;
	jm_status_enu_t status;
	fmi2_import_t* fmu;
	fmi2_status_t fmistatus;
	jm_status_enu_t jmstatus;
	fmi2_real_t tstart;
	fmi2_real_t tcur;
	fmi2_real_t hcur;
	fmi2_real_t hdef;
	size_t n_states;
	size_t n_event_indicators;
	fmi2_real_t* states;
	fmi2_real_t* states_der;
	fmi2_real_t* event_indicators;
	fmi2_real_t* event_indicators_prev;
	fmi2_boolean_t callEventUpdate;
	fmi2_boolean_t terminateSimulation;
	fmi2_boolean_t toleranceControlled;
	fmi2_real_t relativeTolerance;
	fmi2_event_info_t eventInfo;
	size_t k;
};



