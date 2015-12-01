#include "pumpcode.h"

HWND GetPumpNumber(TCHAR* szBuffer, int nMaxLength)
{
	POINT pt;
	pt.x = /*592;*/ 253;
	pt.y = /*814;*/ 635;
  	HWND hwnd = FindWindow(0, _T("基本信息版本V1.2"));
// 	HWND hwnd = FindWindow(0, _T("Dialog"));
	RECT rect;
	GetWindowRect(hwnd, &rect);
// 	ClientToScreen(hwnd, &rect);
	pt.x += rect.left;
	pt.y += rect.top;
	hwnd = SmallestWindowFromPoint(hwnd, pt);
// 	GetItemText(hwnd, szBuffer, nMaxLength);
	GetWindowText(hwnd, szBuffer, nMaxLength);
	SendMessage(hwnd, WM_GETTEXT, MAX_PATH, (LPARAM)szBuffer);
	return hwnd;
}

HWND GetPumpUintCode(TCHAR* szBuffer, int nMaxLength)
{
	POINT pt;
	pt.x = /*240;*/  500;
	pt.y = /*100; */ 620;
	HWND hwnd = FindWindow(0, _T("基本信息版本V1.2"));
	RECT rect;	
// 	HWND hwnd = FindWindow(0, _T("油量修正码打标工具"));

	GetWindowRect(hwnd, &rect);
	pt.x += rect.left;
	pt.y += rect.top;
	hwnd = SmallestWindowFromPoint(hwnd, pt);
	GetWindowText(hwnd, szBuffer, nMaxLength);
	SendMessage(hwnd, WM_GETTEXT, MAX_PATH, (LPARAM)szBuffer);
	
	ListView_GetItemText(hwnd, 1, 1, szBuffer, nMaxLength);
	return (HWND)SendMessage(hwnd, LVM_GETITEMCOUNT, 0, 0);
// 	GetItemText(hwnd, szBuffer, nMaxLength);
	return hwnd;
}

HWND SmallestWindowFromPoint(HWND hwnd, const POINT point)
{
	RECT rect, rcTemp;
	HWND hParent, hWnd, hTemp;

	hWnd = WindowFromPoint( point);
	if (hWnd != NULL)
	{
		GetWindowRect(hWnd, &rect);
		hParent = GetParent(hWnd);

		// Has window a parent?
		if (hParent != NULL)
		{
			// Search down the Z-Order
			hTemp = hWnd;
			do{
				hTemp = GetWindow(hTemp, GW_HWNDNEXT);

				// Search window contains the point, hase the same parent, and is visible?
				GetWindowRect(hTemp, &rcTemp);
				if (PtInRect(&rcTemp, point) && GetParent(hTemp) == hParent && IsWindowVisible(hTemp))
				{
					// Is it smaller?
					if (((rcTemp.right - rcTemp.left) * (rcTemp.bottom - rcTemp.top)) < ((rect.right - rect.left) * (rect.bottom - rect.top)))
					{
						// Found new smaller window!
						hWnd = hTemp;
						GetWindowRect(hWnd, &rect);
					}
				}
			} while (hTemp != NULL);
		}
	}

	return hWnd;
}


void GetItemText(HWND hwnd, TCHAR* szBuffer, int nMaxLength)
{
	DWORD PID;
	GetWindowThreadProcessId(hwnd, &PID);
	if (GetCurrentProcessId() != PID)
	{
			HANDLE hProcess =
				OpenProcess(
				PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ,
				FALSE, PID);

			if (hProcess != NULL) {
				MessageBeep(MB_OK);
				GetWindowTextRemote(hProcess, hwnd, szBuffer);
				CloseHandle(hProcess);
			}
			else
				_tcscpy_s(szBuffer, nMaxLength, __TEXT(""));
	}
}
