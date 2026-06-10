#pragma once

#ifdef CPPFUNCTIONS_EXPORTS
#define CPPFUNCTIONS_API __declspec(dllexport)
#else
#define CPPFUNCTIONS_API __declspec(dllimport)
#endif