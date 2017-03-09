cmake_minimum_required (VERSION 2.6)
project (FMUSimulator)

# The version number.
set (Tutorial_VERSION_MAJOR 1)
set (Tutorial_VERSION_MINOR 0)

#Append cmake module path
LIST(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_LIST_DIR}/cmake")

#------------
#Dependencies
#------------

# Find FMI library
FIND_PACKAGE(FMILib)
IF(FMILIB_FOUND)
  MESSAGE(STATUS "FMI library found.")
ELSE(FMILIB_FOUND)
  MESSAGE(FATAL_ERROR "FMI library not found but required. Set FMILIB_HOME to the location containing lib and include folder of FMI library.")
ENDIF(FMILIB_FOUND)

#------------
# Setup Build
#------------

#macro for DLL build
add_definitions(-D_USRDLL)

#target platform
SET(CMAKE_GENERATOR_PLATFORM  "Visual Studio 14 2015 Win64")

# Find Sources
FILE(GLOB_RECURSE SRCS "source/*.cpp")
#include 
INCLUDE_DIRECTORIES("include" ${FMILIB_INCLUDE_DIR})

#add_executable(FMUSimulator ${SRCS})
add_library(FMUSimulator SHARED ${SRCS})
TARGET_LINK_LIBRARIES(FMUSimulator ${FMILIB_LIBRARIES} ${LIBRARIES_EXTRA})
