# برنامج طباعة XPrinter XP-370B

## نظرة عامة
هذا تطبيق Windows WPF مكتوب بلغة C# للتحكم في طابعة حرارية من نوع XPrinter XP-370B. يتيح البرنامج:
- طباعة أرقام متسلسلة على ملصقات
- التحكم في حجم الورق والنص
- إضافة باركود اختياري لكل رقم
- طباعة رقمين على كل ملصق (يمين وشمال)

## البنية التقنية
- **اللغة**: C# .NET 8.0
- **الإطار**: WPF (Windows Presentation Foundation)
- **المكتبات**: 
  - System.Drawing.Common للتعامل مع الطابعات
  - Windows Print Spooler API (winspool.Drv) للطباعة الخام

## هيكل المشروع
```
mahmoud_elhussiny_printer/
├── App.xaml                      # تكوين التطبيق
├── App.xaml.cs                   # كود التطبيق الأساسي
├── MainWindow.xaml               # واجهة المستخدم الرسومية
├── MainWindow.xaml.cs            # منطق التطبيق
├── AssemblyInfo.cs               # معلومات التجميع
└── mahmoud_elhussiny_printer.csproj  # ملف المشروع
```

## التشغيل في Replit
نظراً لأن هذا تطبيق Windows، يتم تشغيله على Linux باستخدام:
- **Wine64**: طبقة توافق لتشغيل تطبيقات Windows على Linux
- **VNC**: لعرض واجهة المستخدم الرسومية في المتصفح

## بناء المشروع
```bash
cd mahmoud_elhussiny_printer
dotnet build -c Release
```

## التشغيل
```bash
wine64 ./mahmoud_elhussiny_printer/bin/Release/net8.0-windows/mahmoud_elhussiny_printer.exe
```

## ملاحظات مهمة
- **الطباعة الفعلية**: لن تعمل في بيئة Replit لأنه لا توجد طابعة فعلية متصلة
- **واجهة المستخدم**: يمكن رؤية الواجهة واختبار الوظائف، لكن الطباعة ستفشل عند عدم وجود طابعة
- **اللغة**: الواجهة بالعربية مع اتجاه النص من اليمين لليسار (RTL)

## التعديلات المطلوبة للعمل على Replit
تم إضافة الإعدادات التالية لملف `.csproj`:
```xml
<EnableWindowsTargeting>true</EnableWindowsTargeting>
<PackageReference Include="System.Drawing.Common" Version="8.0.0" />
```

## التاريخ
- **2025-01-08**: استيراد المشروع وإعداده للعمل في Replit
- **الإصلاحات**: 
  - إصلاح هيكل الكود (نقل RawPrinterHelper إلى class منفصل)
  - إضافة System.Drawing.Common package
  - إصلاح تحذيرات nullable reference
