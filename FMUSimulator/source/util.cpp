#include "util.h"

int causalityEqual(fmi2_import_variable_t* var, void* enumIdx)
{
	// Get causality attribute, i.e., fmi1_causality_enu_input, fmi1_causality_enu_output, etc.
	fmi2_causality_enu_t causality = fmi2_import_get_causality(var);
	// Use static cast to "convert" void* to fmi1_causality_enu_t*. With that, we can compare the attributes.
	fmi2_causality_enu_t* toComp = static_cast<fmi2_causality_enu_t*>(enumIdx);

	//LOGGER_WRITE(std::string("causalityEqual: fmi1_causality_enu_t causality is ") + std::to_string(causality) + " and enumIdx has attribute " + std::to_string(*toComp) + std::string("."), Util::LC_INIT, Util::LL_INFO);
	if (*toComp == causality)
	{
		//LOGGER_WRITE(std::string("They are equal!"), Util::LC_INIT, Util::LL_INFO);
		//std::cout << "the input var: " << fmi1_import_get_variable_name(var) << std::endl;
		return 1;
	}
	else
	{
		//LOGGER_WRITE(std::string("They are not equal!"), Util::LC_INIT, Util::LL_INFO);
		return 0;
	}
}

int baseTypeEqual(fmi2_import_variable_t* var, void* refBaseType)
{
	fmi2_base_type_enu_t baseType = fmi2_import_get_variable_base_type(var);
	fmi2_base_type_enu_t* toComp = static_cast<fmi2_base_type_enu_t*>(refBaseType);

	//LOGGER_WRITE(std::string("baseTypeEqual: fmi1_base_type_enu_t baseType is ") + std::to_string(baseType) + " and refBaseType is of type" + std::to_string(*toComp) + std::string("."), Util::LC_INIT, Util::LL_INFO);
	if (*toComp == baseType)
	{
		//LOGGER_WRITE(std::string("They are equal!"), Util::LC_INIT, Util::LL_INFO);
		return 1;
	}
	else
	{
		//LOGGER_WRITE(std::string("They are not equal!"), Util::LC_INIT, Util::LL_INFO);
		return 0;
	}
}