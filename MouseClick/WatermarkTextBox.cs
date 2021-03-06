﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseClick
{
    public class WatermarkTextBox : TextBox
    {
        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;

        private string watermarkText;

        public string WatermarkText
        {
            get { return watermarkText; }
            set
            {
                watermarkText = value;
                SetWatermark(watermarkText);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam,[MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private void SetWatermark(string watermarkText)
        {
            SendMessage(Handle, EM_SETCUEBANNER, 0, watermarkText);
        }
    }
}