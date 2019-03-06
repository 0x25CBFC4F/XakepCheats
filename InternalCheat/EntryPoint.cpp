#include <Windows.h>

#include "InternalCheat.h"

BOOL WINAPI DllMain(
  _In_ HINSTANCE hinstDLL,
  _In_ DWORD     fdwReason,
  _In_ LPVOID    lpvReserved
)
{
	if(fdwReason == DLL_PROCESS_ATTACH)
	{
		auto cheat = internal_cheat::get_instance();
		cheat->initialize();
		cheat->run();
	}

	return 0;
}