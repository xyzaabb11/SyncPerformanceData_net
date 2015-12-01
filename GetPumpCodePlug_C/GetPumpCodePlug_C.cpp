// GetPumpCodePlug_C.cpp : 定义应用程序的入口点。
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

// 全局变量: 
HINSTANCE hInst;								// 当前实例
TCHAR szTitle[MAX_LOADSTRING];					// 标题栏文本
TCHAR szWindowClass[MAX_LOADSTRING];			// 主窗口类名
HWND hOutText;

HINSTANCE ghInstance;

// 此代码模块中包含的函数的前向声明: 
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
//  函数:  WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  目的:    处理主窗口的消息。
//
//  WM_COMMAND	- 处理应用程序菜单
//  WM_PAINT	- 绘制主窗口
//  WM_DESTROY	- 发送退出消息并返回
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
	HWND hwnd = FindWindow(0, _T("基本信息版本V1.2"));
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

// “关于”框的消息处理程序。
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
