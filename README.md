بما أن هذا مشروع جامعي، يجب أن يكون ملف الـ **README** احترافياً، تقنياً، ويحتوي على "إخلاء مسؤولية أخلاقي" لضمان عدم استخدامه بشكل خاطئ.

إليك محتوى ملف `README.md` جاهز للتنسيق:

---

# Keyboard Input Monitor (Educational Security Project)

## 📌 Project Overview

This project is a **System Input Monitor** (Keylogger) developed in **C#** using the **.NET Framework**. It was created for educational purposes to demonstrate how low-level system hooks work within the Windows operating system and how data can be exfiltrated via secure protocols (SMTP).

## 🚀 Features

* **Low-Level Keyboard Hook:** Intercepts system-wide keyboard events using `user32.dll`.
* **Stealth Mode:** Operates as a background process with no visible GUI/Console window.
* **Persistence:** Automatically registers itself in the Windows Registry to start on system boot.
* **Multi-Language Support:** Utilizes `ToUnicodeEx` to correctly capture inputs in multiple languages, including **Arabic** and **English**, based on the active keyboard layout.
* **Data Formatting:** Intelligent parsing of control keys like `Space`, `Enter`, and `Backspace` to produce human-readable logs.
* **Automated Exfiltration:** Sends captured data to a predefined email address via **SMTP** every 50 keystrokes.

## 🛠️ Technical Implementation

* **WinAPI Integration:** Uses P/Invoke to access `SetWindowsHookEx`, `GetKeyboardLayout`, and `ToUnicodeEx`.
* **Registry Manipulation:** Modifies `HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run` for persistence.
* **Event-Driven:** Implements a message loop using `Application.Run()` to stay active without consuming excessive CPU resources.

## 📋 Prerequisites

* Windows OS (Tested on Windows 10/11)
* .NET Framework 4.7.2 or higher
* Visual Studio 2019/2022
* **Reference Required:** `System.Windows.Forms` must be added to the project references.

## ⚙️ Configuration

Before building the project, update the following variables in `Program.cs`:

```csharp
private static string myEmail = "your-email@gmail.com";
private static string myAppPassword = "your-app-password";

```

*Note: You must use a Google "App Password" if using Gmail.*

## ⚠️ Ethical & Legal Disclaimer

**For Educational Purposes Only.**
This software is intended for academic research and authorized security testing. The author is not responsible for any misuse or damage caused by this program. Unauthorized monitoring of computer systems is illegal and unethical.

---
