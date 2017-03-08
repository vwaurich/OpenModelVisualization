#pragma once

#include <fmilib.h>

int causalityEqual(fmi2_import_variable_t* var, void* enumIdx);
int baseTypeEqual(fmi2_import_variable_t* var, void* refBaseType);

