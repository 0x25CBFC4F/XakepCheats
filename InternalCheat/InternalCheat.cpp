#include "InternalCheat.h"

#include <Windows.h>
#include "psapi.h"
#include <cstdio>
#include <vector>
#include <sstream>

using namespace std;

#define INRANGE(x,a,b)  (x >= a && x <= b)
#define getBits( x )    (INRANGE((x&(~0x20)),'A','F') ? ((x&(~0x20)) - 'A' + 0xa) : (INRANGE(x,'0','9') ? x - '0' : 0))
#define getByte( x )    (getBits(x[0]) << 4 | getBits(x[1]))

internal_cheat* internal_cheat::instance_ = nullptr;

internal_cheat::internal_cheat() = default;

internal_cheat* internal_cheat::get_instance()
{
	if(instance_ == nullptr)
	{
		instance_ = new internal_cheat();
	}

	return instance_;
}

void internal_cheat::initialize()
{
	if(was_initialized_)
	{
		return;
	}
	
	printf("\n\n[CHEAT] Cheat was loaded! Initializing..\n");

	was_initialized_ = true;
	player_base_ = reinterpret_cast<void*>(find_pattern("ED 03 00 00 01 00 00 00"));

	printf("[CHEAT] Found playerbase at 0x%p\n", player_base_);
}

void internal_cheat::run()
{
	printf("[CHEAT] Cheat is now running.\n");

	const auto player_health = reinterpret_cast<int*>(reinterpret_cast<DWORD>(player_base_) + 7);

	while(true)
	{
		*player_health = INT_MAX;
		Sleep(100);
	}
}

DWORD internal_cheat::find_pattern(std::string pattern)
{
	auto mbi = MEMORY_BASIC_INFORMATION();
	DWORD curr_addr = 0;

	while(true)
	{
		if(VirtualQuery(reinterpret_cast<const void*>(curr_addr), &mbi, sizeof mbi) == 0)
		{
			break;
		}

		if((mbi.State == MEM_COMMIT || mbi.State == MEM_RESERVE) &&
			(mbi.Protect == PAGE_READONLY ||
				mbi.Protect == PAGE_READWRITE ||
				mbi.Protect == PAGE_EXECUTE_READ ||
				mbi.Protect == PAGE_EXECUTE_READWRITE)) 
        {
			auto result = find_pattern_in_range(pattern, reinterpret_cast<DWORD>(mbi.BaseAddress), reinterpret_cast<DWORD>(mbi.BaseAddress) + mbi.RegionSize);

			if(result != NULL)
			{
				return result;
			}
		}

		curr_addr += mbi.RegionSize;
	}

	return NULL;
}

DWORD internal_cheat::find_pattern_in_range(std::string pattern, const DWORD range_start, const DWORD range_end)
{
	//printf("Finding value in range: 0x%p -> 0x%p\n", reinterpret_cast<void*>(range_start), reinterpret_cast<void*>(range_end));
	
	auto strstream = istringstream(pattern);

	vector<int> values;
	string s;

	while (getline(strstream, s, ' '))
	{
		if (s.find("??") != std::string::npos)
		{
			values.push_back(-1);
			continue;
		}

		auto parsed = stoi(s, 0, 16);
		values.push_back(parsed);
	}

	for(auto p_cur = range_start; p_cur < range_end; p_cur++ )
	{
		//printf("Searching at 0x%p..\n", reinterpret_cast<void*>(p_cur));

		auto localAddr = p_cur;
		auto found = true;

		for (auto value : values)
		{
			//printf("Value at 0x%p ", reinterpret_cast<void*>(localAddr));

			if(value == -1)
			{
				//printf("is validated due to an unknown byte.\n");
				localAddr += 1;
				continue;
			}

			auto neededValue = static_cast<char>(value);
			auto pCurrentValue = reinterpret_cast<char*>(localAddr);
			auto currentValue = *pCurrentValue;

			if(neededValue != currentValue)
			{
				//printf("not validated.\n");
				found = false;
				break;
			}

			//printf("is validated.\n");
			localAddr += 1;
		}

		if(found)
		{
			return p_cur;
		}
	}

	return NULL;
}
