#pragma once
#include <string>
#include <windows.h>

class internal_cheat
{
public:
	static internal_cheat* get_instance();
	void initialize();
	void run();
	DWORD find_pattern(std::string pattern);

private:
	static internal_cheat* instance_;
	bool was_initialized_ = false;
	void* player_base_;

	internal_cheat();
	static DWORD find_pattern_in_range(std::string pattern, DWORD range_start, DWORD range_end);
};

