#include <tchar.h>
#include <string>
#include <windows.h>
#include <CommCtrl.h>
#pragma comment(lib, "comCtl32.lib")

#include "InjCode.h"
#ifdef UNICODE
typedef std::wstring tstring;
#else
typedef std::string tstring;
#endif

HWND SmallestWindowFromPoint(HWND hwnd, const POINT point);
void GetItemText(HWND hwnd, TCHAR* szBuffer, int nMaxLength);