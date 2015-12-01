// GetPumpCodePlug_C.cpp : ����Ӧ�ó������ڵ㡣
//

#include "stdafx.h"
#include "GetPumpCodePlug_C.h"
#include <CommCtrl.h>
#include <string>
#ifdef UNICODE
typedef std::wstring tstring;
#else
typedef std::string tstring;
#endif

#pragma comment(lib, "comCtl32.lib")

#define MAX_LOADSTRING 100

// ȫ�ֱ���: 
HINSTANCE hInst;								// ��ǰʵ��
TCHAR szTitle[MAX_LOADSTRING];					// �������ı�
TCHAR szWindowClass[MAX_LOADSTRING];			// ����������
HWND hOutText;

HINSTANCE ghInstance;

// �˴���ģ���а����ĺ�����ǰ������: 
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
BOOL CALLBACK DialogProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam);
void	OnCreate(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);
void AddOutString(tstring msg);
BOOL CALLBACK EnumChildWindowsProc(HWND hChild, LPARAM lparam);

int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPTSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
	ghInstance = hInstance;
	InitCommonControls();
	DialogBox(hInstance, MAKEINTRESOURCE(IDD_DLG_MAIN), NULL, DialogProc);
	return 0;

}




//
//  ����:  WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  Ŀ��:    ���������ڵ���Ϣ��
//
//  WM_COMMAND	- ����Ӧ�ó���˵�
//  WM_PAINT	- ����������
//  WM_DESTROY	- �����˳���Ϣ������
//
//
BOOL CALLBACK DialogProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{


	switch (message)
	{
	case WM_INITDIALOG:
		OnCreate(hDlg, message, wParam, lParam);
		break;
	case WM_CLOSE:
		EndDialog(hDlg, LOWORD(wParam));
		break;
	}
	return 0;
}

void	OnCreate(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	hOutText = GetDlgItem(hDlg, IDC_EDIT_OUT);
	AddOutString(_T("OnCreate"));
	HWND hwnd = FindWindow(0, _T("������Ϣ�汾V1.2"));
	EnumChildWindows(hwnd, EnumChildWindowsProc, 0);
}

BOOL CALLBACK EnumChildWindowsProc(HWND hChild, LPARAM lparam)
{
	TCHAR szBuffer[MAX_PATH];
	if (hChild)
	{
	SendMessage(hChild, WM_GETTEXT, MAX_PATH, (LPARAM)szBuffer);
	AddOutString(szBuffer);
	}
	return TRUE;
}

void AddOutString(tstring msg)
{
	msg += _T("\r\n");
	int ilen = msg.length();
	SendMessage(hOutText, EM_SETSEL, -1, -1);
	SendMessage(hOutText, EM_REPLACESEL, FALSE, (LPARAM)msg.c_str());
	SendMessage(hOutText, EM_SCROLLCARET, 0, 0);
}

// �����ڡ������Ϣ�������
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(lParam);
	switch (message)
	{
	case WM_INITDIALOG:
		return (INT_PTR)TRUE;

	case WM_COMMAND:
		if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
		{
			EndDialog(hDlg, LOWORD(wParam));
			return (INT_PTR)TRUE;
		}
		break;
	}
	return (INT_PTR)FALSE;
}
