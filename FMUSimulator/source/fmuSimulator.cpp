#include "fmuSimulator.h"
#include "simulationAPI.h"


DLLEXPORT int getFMUVersion(void* simulator)
{
	simulatorAPI* sim = (simulatorAPI*)simulator;
	return sim->fmuVersion;
}

DLLEXPORT void* createFMUsimulator(const char* fmuPath, const char* workDir)
{
	simulatorAPI* simAPI = new simulatorAPI();
	int fmuVersion = simAPI->loadFMU(workDir, fmuPath);
	simAPI->fmuVersion = fmuVersion;
	return simAPI;
}

DLLEXPORT int simulateToTime(void* simulator, float nextTime) {
	simulatorAPI* sim = (simulatorAPI*)simulator;
	sim->simulateFMU(nextTime);
	return 4;
}
DLLEXPORT int initSimulation(void* simulator) {
	simulatorAPI* sim = (simulatorAPI*)simulator;
	sim->initFMU();
	return 5;
}

DLLEXPORT int destroySimulator(void* simulator)
{
	delete (simulatorAPI*)simulator;
	return 1;
}

DLLEXPORT int getVarRefForVarName(void* simulator, const char* name)
{
	simulatorAPI* sim = (simulatorAPI*)simulator;
	try {
		fmi2_import_variable_t* var = fmi2_import_get_variable_by_name(sim->fmu, name);
		fmi2_value_reference_t t = fmi2_import_get_variable_vr(var);
		return (int)t;
	}
	catch (...) {
		return -1;
	}
}

DLLEXPORT int getRealValuesForVarRefLst(void* simulator, void* varRef, int numVars, void* val)
{
	simulatorAPI* sim = (simulatorAPI*)simulator;
	fmi2_import_get_real(sim->fmu, (unsigned int*)varRef, numVars, (double*)val);
	return 1;
}

DLLEXPORT float getRealValueForVarRef(void* simulator, int varRef)
{
	simulatorAPI* sim = (simulatorAPI*)simulator;
	double val = 0;
	unsigned int addr = (unsigned int)varRef;
	fmi2_import_get_real(sim->fmu, &addr, 1, &val);
	return (float)val;
}

DLLEXPORT int setEulerStepSize(void* simulator, float h)
{
	simulatorAPI* sim = (simulatorAPI*)simulator;
	sim->hdef = h;
	return 1;
}


simulatorAPI::simulatorAPI() : 
	fmuVersion(-1),
	tstart(0.0),
	hdef(0.001),
	terminateSimulation(fmi2_false),
	toleranceControlled(fmi2_true),
	relativeTolerance(0.001)
{
}

int simulatorAPI::loadFMU(const char* path, const char* file)
{
	int fmuVersion = 123;

	callbacks.malloc = malloc;
	callbacks.calloc = calloc;
	callbacks.realloc = realloc;
	callbacks.free = free;
	callbacks.logger = jm_default_logger;
	callbacks.log_level = jm_log_level_debug;
	callbacks.context = 0;

	context = fmi_import_allocate_context(&callbacks);
	version = fmi_import_get_fmi_version(context, file, path);

	if (version != fmi_version_2_0_enu) {
		printf("Only version 2.0 is supported by this code\n");
		fmuVersion = 0;
	}
	else
	{
		fmuVersion = 2;
	}

	fmu = fmi2_import_parse_xml(context, path, 0);

	if (!fmu) {
		printf("Error parsing XML, exiting\n");
	}

	if (fmi2_import_get_fmu_kind(fmu) == fmi2_fmu_kind_cs) {
		printf("Only ME 2.0 is supported by this code\n");
	}

	callBackFunctions.logger = fmi2_log_forwarding;
	callBackFunctions.allocateMemory = calloc;
	callBackFunctions.freeMemory = free;
	callBackFunctions.componentEnvironment = fmu;

	status = fmi2_import_create_dllfmu(fmu, fmi2_fmu_kind_me, &callBackFunctions);
	if (status == jm_status_error) {
		printf("Could not create the DLL loading mechanism(C-API test).\n");
	}
	return fmuVersion;
};

int simulatorAPI::initFMU() {

	printf("Version returned from FMU:   %s\n", fmi2_import_get_version(fmu));
	printf("Platform type returned:      %s\n", fmi2_import_get_types_platform(fmu));

	n_states = fmi2_import_get_number_of_continuous_states(fmu);
	n_event_indicators = fmi2_import_get_number_of_event_indicators(fmu);
	states = (fmi2_real_t*)calloc(n_states, sizeof(double));
	states_der = (fmi2_real_t*)calloc(n_states, sizeof(double));
	event_indicators = (fmi2_real_t*)calloc(n_event_indicators, sizeof(double));
	event_indicators_prev = (fmi2_real_t*)calloc(n_event_indicators, sizeof(double));

	jmstatus = fmi2_import_instantiate(fmu, "Test", fmi2_model_exchange, "Win32", 0);
	if (jmstatus == jm_status_error) {
		printf("fmi2_import_instantiate failed\n");
	}

	fmistatus = fmi2_import_set_debug_logging(fmu, fmi2_false, 0, 0);
	printf("fmi2_import_set_debug_logging:  %s\n", fmi2_status_to_string(fmistatus));
	fmi2_import_set_debug_logging(fmu, fmi2_false, 0, 0);

	fmistatus = fmi2_import_setup_experiment(fmu, toleranceControlled,
		relativeTolerance, tstart, fmi2_false, 0.0);

	fmistatus = fmi2_import_enter_initialization_mode(fmu);
	fmistatus = fmi2_import_exit_initialization_mode(fmu);

	tcur = tstart;
	hcur = hdef;
	callEventUpdate = fmi2_false;

	eventInfo.newDiscreteStatesNeeded = fmi2_false;
	eventInfo.terminateSimulation = fmi2_false;
	eventInfo.nominalsOfContinuousStatesChanged = fmi2_false;
	eventInfo.valuesOfContinuousStatesChanged = fmi2_true;
	eventInfo.nextEventTimeDefined = fmi2_false;
	eventInfo.nextEventTime = -0.0;

	/* fmiExitInitializationMode leaves FMU in event mode */
	do_event_iteration(fmu, &eventInfo);
	fmi2_import_enter_continuous_time_mode(fmu);

	fmistatus = fmi2_import_get_continuous_states(fmu, states, n_states);
	fmistatus = fmi2_import_get_event_indicators(fmu, event_indicators, n_event_indicators);


	return 9;
}

void simulatorAPI::do_event_iteration(fmi2_import_t *fmu, fmi2_event_info_t *eventInfo)
{
	eventInfo->newDiscreteStatesNeeded = fmi2_true;
	eventInfo->terminateSimulation = fmi2_false;
	while (eventInfo->newDiscreteStatesNeeded && !eventInfo->terminateSimulation) {
		fmi2_import_new_discrete_states(fmu, eventInfo);
	}
}

int simulatorAPI::simulateFMU(float nextTime)
{
	while ((tcur < nextTime) && (!(eventInfo.terminateSimulation || terminateSimulation)))
	{
		size_t k;
		fmi2_real_t tlast;
		int zero_crossing_event = 0;

		fmistatus = fmi2_import_set_time(fmu, tcur);

		{ /* Swap event_indicators and event_indicators_prev so that we can get new indicators */
			fmi2_real_t *temp = event_indicators;
			event_indicators = event_indicators_prev;
			event_indicators_prev = temp;
		}
		fmistatus = fmi2_import_get_event_indicators(fmu, event_indicators, n_event_indicators);

		/* Check if an event indicator has triggered */
		for (k = 0; k < n_event_indicators; k++) {
			if ((event_indicators[k] > 0) != (event_indicators_prev[k] > 0)) {
				zero_crossing_event = 1;
				break;
			}
		}

		/* Handle any events */
		if (callEventUpdate || zero_crossing_event ||
			(eventInfo.nextEventTimeDefined && tcur == eventInfo.nextEventTime)) {
			fmistatus = fmi2_import_enter_event_mode(fmu);
			do_event_iteration(fmu, &eventInfo);
			fmistatus = fmi2_import_enter_continuous_time_mode(fmu);

			fmistatus = fmi2_import_get_continuous_states(fmu, states, n_states);
			fmistatus = fmi2_import_get_event_indicators(fmu, event_indicators, n_event_indicators);
		}

		/* Calculate next time step */
		tlast = tcur;
		tcur += hdef;
		if (eventInfo.nextEventTimeDefined && (tcur >= eventInfo.nextEventTime)) {
			tcur = eventInfo.nextEventTime;
		}
		hcur = tcur - tlast;
		/*
		if (tcur > nextTime - hcur / 1e16) {
			tcur = nextTime;
			hcur = tcur - tlast;
		}
		*/

		/* Integrate a step */
		fmistatus = fmi2_import_get_derivatives(fmu, states_der, n_states);
		for (k = 0; k < n_states; k++) {
			states[k] = states[k] + hcur*states_der[k];
		}

		/* Set states */
		fmistatus = fmi2_import_set_continuous_states(fmu, states, n_states);
		/* Step is complete */
		fmistatus = fmi2_import_completed_integrator_step(fmu, fmi2_true, &callEventUpdate,
			&terminateSimulation);
	}
	return 5;
}


simulatorAPI::~simulatorAPI()
{
	fmistatus = fmi2_import_terminate(fmu);
	fmi2_import_free_instance(fmu);
	free(states);
	free(states_der);
	free(event_indicators);
	free(event_indicators_prev);
}
