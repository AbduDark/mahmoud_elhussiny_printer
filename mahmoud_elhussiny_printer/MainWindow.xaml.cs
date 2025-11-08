using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Drawing;

namespace mahmoud_elhussiny_printer
{
    public class AppSettings
    {
        public double Width { get; set; } = 1.57;
        public double Height { get; set; } = 0.98;
        public int FontSize { get; set; } = 2;
        public int BarcodeHeight { get; set; } = 60;
        public int LeftX { get; set; } = 20;
        public int RightX { get; set; } = 200;
        public int TopY { get; set; } = 10;
        public int BottomY { get; set; } = 140;
        public int BarcodeTopY { get; set; } = 30;
        public int BarcodeBottomY { get; set; } = 160;
        public bool PrintBarcode { get; set; } = false;
    }

    public partial class MainWindow : Window
    {
        private const string SettingsFile = "printer_settings.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadPrinters();
            LoadSettings();
        }

        private void LoadPrinters()
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                cmbPrinters.Items.Add(printer);

            if (cmbPrinters.Items.Count > 0)
                cmbPrinters.SelectedIndex = 0;
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    string json = File.ReadAllText(SettingsFile);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    
                    if (settings != null)
                    {
                        txtWidth.Text = settings.Width.ToString();
                        txtHeight.Text = settings.Height.ToString();
                        txtFontSize.Text = settings.FontSize.ToString();
                        txtBarcodeHeight.Text = settings.BarcodeHeight.ToString();
                        txtLeftX.Text = settings.LeftX.ToString();
                        txtRightX.Text = settings.RightX.ToString();
                        txtTopY.Text = settings.TopY.ToString();
                        txtBottomY.Text = settings.BottomY.ToString();
                        txtBarcodeTopY.Text = settings.BarcodeTopY.ToString();
                        txtBarcodeBottomY.Text = settings.BarcodeBottomY.ToString();
                        chkBarcode.IsChecked = settings.PrintBarcode;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الإعدادات: {ex.Message}");
            }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new AppSettings
                {
                    Width = double.TryParse(txtWidth.Text, out double w) ? w : 1.57,
                    Height = double.TryParse(txtHeight.Text, out double h) ? h : 0.98,
                    FontSize = int.TryParse(txtFontSize.Text, out int f) ? f : 2,
                    BarcodeHeight = int.TryParse(txtBarcodeHeight.Text, out int bh) ? bh : 60,
                    LeftX = int.TryParse(txtLeftX.Text, out int lx) ? lx : 20,
                    RightX = int.TryParse(txtRightX.Text, out int rx) ? rx : 200,
                    TopY = int.TryParse(txtTopY.Text, out int ty) ? ty : 10,
                    BottomY = int.TryParse(txtBottomY.Text, out int by) ? by : 140,
                    BarcodeTopY = int.TryParse(txtBarcodeTopY.Text, out int bty) ? bty : 30,
                    BarcodeBottomY = int.TryParse(txtBarcodeBottomY.Text, out int bby) ? bby : 160,
                    PrintBarcode = chkBarcode.IsChecked == true
                };

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الإعدادات: {ex.Message}");
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtFrom.Text, out int from) || !int.TryParse(txtTo.Text, out int to))
            {
                MessageBox.Show("من فضلك أدخل أرقام صحيحة.");
                return;
            }

            if (!double.TryParse(txtWidth.Text, out double width))
                width = 1.57;
            if (!double.TryParse(txtHeight.Text, out double height))
                height = 0.98;
            if (!int.TryParse(txtFontSize.Text, out int font))
                font = 2;
            if (!int.TryParse(txtBarcodeHeight.Text, out int barcodeHeight))
                barcodeHeight = 60;
            
            if (!int.TryParse(txtLeftX.Text, out int leftX))
                leftX = 20;
            if (!int.TryParse(txtRightX.Text, out int rightX))
                rightX = 200;
            if (!int.TryParse(txtTopY.Text, out int topY))
                topY = 10;
            if (!int.TryParse(txtBottomY.Text, out int bottomY))
                bottomY = 140;
            if (!int.TryParse(txtBarcodeTopY.Text, out int barcodeTopY))
                barcodeTopY = 30;
            if (!int.TryParse(txtBarcodeBottomY.Text, out int barcodeBottomY))
                barcodeBottomY = 160;
            
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

                // الصف الأول - رقم 1 (يسار أعلى)
                sb.AppendLine($"TEXT {leftX},{topY},\"3\",0,{font},{font},\"{num1}\"");
                // الصف الأول - رقم 2 (يمين أعلى)
                sb.AppendLine($"TEXT {rightX},{topY},\"3\",0,{font},{font},\"{num2}\"");

                // الصف الثاني - رقم 3 (يسار أسفل)
                sb.AppendLine($"TEXT {leftX},{bottomY},\"3\",0,{font},{font},\"{num3}\"");
                // الصف الثاني - رقم 4 (يمين أسفل)
                sb.AppendLine($"TEXT {rightX},{bottomY},\"3\",0,{font},{font},\"{num4}\"");

                // الباركودات (اختياري)
                if (printBarcode)
                {
                    sb.AppendLine($"BARCODE {leftX},{barcodeTopY},\"128\",{barcodeHeight},1,0,1,2,\"{num1}\"");
                    sb.AppendLine($"BARCODE {rightX},{barcodeTopY},\"128\",{barcodeHeight},1,0,1,2,\"{num2}\"");
                    sb.AppendLine($"BARCODE {leftX},{barcodeBottomY},\"128\",{barcodeHeight},1,0,1,2,\"{num3}\"");
                    sb.AppendLine($"BARCODE {rightX},{barcodeBottomY},\"128\",{barcodeHeight},1,0,1,2,\"{num4}\"");
                }

                sb.AppendLine("PRINT 1,1");

                RawPrinterHelper.SendStringToPrinter(printerName, sb.ToString());
            }

            SaveSettings();
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
