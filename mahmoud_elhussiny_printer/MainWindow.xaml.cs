using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Drawing;

namespace mahmoud_elhussiny_printer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadPrinters();
        }

        private void LoadPrinters()
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                cmbPrinters.Items.Add(printer);

            cmbPrinters.SelectedIndex = 0;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtFrom.Text, out int from) || !int.TryParse(txtTo.Text, out int to))
            {
                MessageBox.Show("من فضلك أدخل أرقام صحيحة.");
                return;
            }

            if (!double.TryParse(txtWidth.Text, out double width))
                width = 3.15;
            if (!double.TryParse(txtHeight.Text, out double height))
                height = 0.98;
            if (!int.TryParse(txtFontSize.Text, out int font))
                font = 2;
            if (!int.TryParse(txtBarcodeHeight.Text, out int barcodeHeight))
                barcodeHeight = 60;
            if (cmbPrinters.SelectedItem == null)
            {
                MessageBox.Show("من فضلك اختر الطابعة أولاً.");
                return;
            }
            string? printerName = cmbPrinters.SelectedItem.ToString();
            if (string.IsNullOrEmpty(printerName))
            {
                MessageBox.Show("من فضلك اختر الطابعة أولاً.");
                return;
            }
            bool printBarcode = chkBarcode.IsChecked == true;

            for (int i = from; i <= to; i += 4)
            {
                int num1 = i;
                int num2 = i + 1 <= to ? i + 1 : i;
                int num3 = i + 2 <= to ? i + 2 : i;
                int num4 = i + 3 <= to ? i + 3 : i;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"SIZE {width},{height}");
                sb.AppendLine("GAP 0.08,0");
                sb.AppendLine("CLS");
                sb.AppendLine("DIRECTION 1");
                sb.AppendLine("REFERENCE 0,0");

                // الملصق الأول - النص الأول (يمين)
                sb.AppendLine($"TEXT 20,10,\"3\",0,{font},{font},\"{num1}\"");
                // الملصق الأول - النص الثاني (شمال)
                sb.AppendLine($"TEXT 200,10,\"3\",0,{font},{font},\"{num2}\"");

                // الملصق الأول - الباركودات (اختياري)
                if (printBarcode)
                {
                    sb.AppendLine($"BARCODE 20,50,\"128\",{barcodeHeight},1,0,2,4,\"{num1}\"");
                    sb.AppendLine($"BARCODE 200,50,\"128\",{barcodeHeight},1,0,2,4,\"{num2}\"");
                }

                sb.AppendLine("PRINT 1,1");

                // الملصق الثاني
                sb.AppendLine("CLS");
                sb.AppendLine($"SIZE {width},{height}");
                sb.AppendLine("GAP 0.08,0");
                sb.AppendLine("DIRECTION 1");
                sb.AppendLine("REFERENCE 0,0");

                // الملصق الثاني - النص الأول (يمين)
                sb.AppendLine($"TEXT 20,10,\"3\",0,{font},{font},\"{num3}\"");
                // الملصق الثاني - النص الثاني (شمال)
                sb.AppendLine($"TEXT 200,10,\"3\",0,{font},{font},\"{num4}\"");

                // الملصق الثاني - الباركودات (اختياري)
                if (printBarcode)
                {
                    sb.AppendLine($"BARCODE 20,50,\"128\",{barcodeHeight},1,0,2,4,\"{num3}\"");
                    sb.AppendLine($"BARCODE 200,50,\"128\",{barcodeHeight},1,0,2,4,\"{num4}\"");
                }

                sb.AppendLine("PRINT 1,1");

                RawPrinterHelper.SendStringToPrinter(printerName, sb.ToString());
            }

            MessageBox.Show("✅ تمت الطباعة بنجاح");
        }
    }

    public class DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)] public string pDocName = string.Empty;
        [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile = string.Empty;
        [MarshalAs(UnmanagedType.LPStr)] public string pDataType = string.Empty;
    }

    public static class RawPrinterHelper
    {
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true)]
        public static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        public static bool SendStringToPrinter(string printerName, string data)
        {
            IntPtr pBytes;
            int dwCount = Encoding.ASCII.GetByteCount(data);
            pBytes = Marshal.StringToCoTaskMemAnsi(data);
            bool success = SendBytesToPrinter(printerName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return success;
        }

        private static bool SendBytesToPrinter(string printerName, IntPtr pBytes, int dwCount)
        {
            IntPtr hPrinter;
            DOCINFOA di = new DOCINFOA { pDocName = "TSPL Document", pDataType = "RAW" };
            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero)) return false;
            StartDocPrinter(hPrinter, 1, di);
            StartPagePrinter(hPrinter);
            WritePrinter(hPrinter, pBytes, dwCount, out _);
            EndPagePrinter(hPrinter);
            EndDocPrinter(hPrinter);
            ClosePrinter(hPrinter);
            return true;
        }
    }
}
