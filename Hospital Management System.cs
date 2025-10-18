using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalManagementSystem
{
    // ===================================================================
    // 1. التجريد (Abstraction) - الواجهة (Interface)
    // ===================================================================

    // الواجهة تحدد السلوكيات الأساسية التي يجب أن يمتلكها أي كيان في المستشفى
    public interface IHospitalEntity
    {
        // عرض ملخص لبيانات الكيان
        string GetSummary();

        // دالة مجردة لحساب راتب الكيان (تختلف طريقة حسابه لكل نوع)
        double CalculateSalary();
    }

    // ===================================================================
    // 6. الفئات والإجراءات الساكنة (Static Classes & Members) - للتحقق من الصحة
    // ===================================================================

    // فئة ساكنة تحتوي على دوال عامة للتحقق من صحة البيانات
    public static class DataValidator
    {
        public static bool IsValidName(string name)
        {
            // التحقق من أن الاسم ليس فارغاً أو مسافات بيضاء
            return !string.IsNullOrWhiteSpace(name);
        }

        public static bool IsNonNegative(int value)
        {
            return value >= 0;
        }
    }

    // ===================================================================
    // 1. التجريد (Abstraction) - الفئة المجردة (Abstract Class)
    // 2. التغليف (Encapsulation)
    // ===================================================================

    // فئة مجردة تطبق الواجهة وتضم الخصائص المشتركة بين جميع الموظفين
    public abstract class StaffMember : IHospitalEntity
    {
        // حقول خاصة (Private fields) - لتطبيق التغليف
        private readonly Guid _staffId; // للقراءة فقط
        private string _name;
        private int _yearsOfExperience;
        protected double _baseSalary; // protected للسماح للفئات المشتقة بالوصول لقيمة الراتب الأساسي

        // خاصية للقراءة فقط للمعرف (StaffId) - تطبيق التغليف
        public Guid StaffId
        {
            get { return _staffId; }
        }

        // خاصية للوصول (Property) لـ Name مع تحكم في البيانات - تطبيق التغليف
        public string Name
        {
            get { return _name; }
            set
            {
                // استخدام دالة ساكنة للتحقق من صحة الاسم
                if (DataValidator.IsValidName(value))
                {
                    _name = value;
                }
                else
                {
                    throw new ArgumentException(”Staff name cannot be empty (Encapsulation violated).”);
                }
            }
        }

        public int YearsOfExperience
        {
            get { return _yearsOfExperience; }
            private set // خاصية للقراءة فقط، لا تُعدل إلا داخلياً
            {
                if (DataValidator.IsNonNegative(value))
                {
                    _yearsOfExperience = value;
                }
                else
                {
                    throw new ArgumentException(”Years of experience must be non-negative.”);
                }
            }
        }

        // دالة مجردة إضافية (Abstract Method) - يجب على كل فئة مشتقة تطبيقها
        public abstract string GetDepartment();

        // باني (Constructor)
        public StaffMember(string name, int experience, double baseSalary)
        {
            _staffId = Guid.NewGuid(); // تعيين المعرّف الفريد
            Name = name;
            YearsOfExperience = experience;
            _baseSalary = baseSalary;
        }

        // تطبيق دالة من الواجهة (IHospitalEntity)
        public string GetSummary()
        {
            return $”ID: {_staffId}, Name: {_name}, Exp: {_yearsOfExperience} years, Dept: {GetDepartment()}”;
        }

        // تطبيق الدالة المجردة من الواجهة (Polymorphism)
        public abstract double CalculateSalary();
    }

    // ===================================================================
    // 3. الوراثة (Inheritance)
    // 4. تعدد الأشكال (Polymorphism)
    // 5. التفويض والأحداث (Delegates & Events)
    // ===================================================================

    public class Doctor : StaffMember
    {
        private string _specialty;

        // تعريف التفويض (Delegate)
        public delegate void StatusChangeHandler(string staffName, string newStatus);
        // تعريف الحدث (Event) بناءً على التفويض
        public event StatusChangeHandler OnStatusChanged;

        public Doctor(string name, int experience, double baseSalary, string specialty)
            : base(name, experience, baseSalary)
        {
            _specialty = specialty;
        }

        // إعادة تعريف دالة CalculateSalary() - تطبيق تعدد الأشكال (Polymorphism)
        public override double CalculateSalary()
        {
            // صيغة مختلفة لحساب الراتب: الراتب الأساسي + علاوة خبرة + علاوة تخصص
            return _baseSalary + (YearsOfExperience * 500) + 2000;
        }

        // إعادة تعريف دالة GetDepartment()
        public override string GetDepartment()
        {
            return $”Medical ({_specialty})”;
        }

        // دالة تطلق الحدث (Event)
        public void SetOnDutyStatus(bool onDuty)
        {
            string status = onDuty ? “On Duty” : “Off Duty”;

            // إطلاق الحدث (إذا كان هناك مشتركين)
            if (OnStatusChanged != null)
            {
                OnStatusChanged(Name, status);
            }
        }
    }

    public class Nurse : StaffMember
    {
        public Nurse(string name, int experience, double baseSalary)
            : base(name, experience, baseSalary)
        {
        }

        // إعادة تعريف دالة CalculateSalary() - تطبيق تعدد الأشكال (Polymorphism)
        public override double CalculateSalary()
        {
            // صيغة مختلفة: الراتب الأساسي + علاوة خبرة فقط
            return _baseSalary + (YearsOfExperience * 300);
        }

        // إعادة تعريف دالة GetDepartment()
        public override string GetDepartment()
        {
            return “Nursing”;
        }
    }

    public class AdminStaff : StaffMember
    {
        public AdminStaff(string name, int experience, double baseSalary)
            : base(name, experience, baseSalary)
        {
        }

        // إعادة تعريف دالة CalculateSalary() - تطبيق تعدد الأشكال (Polymorphism)
        public override double CalculateSalary()
        {
            // صيغة مختلفة: راتب ثابت مع علاوة إدارية
            return _baseSalary + 1000;
        }

        // إعادة تعريف دالة GetDepartment()
        public override string GetDepartment()
        {
            return “Administration”;
        }
    }

    // ===================================================================
    // 6. الأعضاء الساكنة (Static Members) - فئة الإدارة
    // ===================================================================

    // فئة الإدارة المركزية
    public class HospitalRegistry
    {
        private List<StaffMember> _staff = new List<StaffMember>();

        // خاصية ساكنة (Static Property) لحساب العدد الكلي للموظفين
        public static int TotalStaffCount { get; private set; } = 0;

        public void AddStaff(StaffMember member)
        {
            _staff.Add(member);
            TotalStaffCount++; // زيادة العدد عبر الخاصية الساكنة
        }

        // دالة لتطبيق تعدد الأشكال (Polymorphism)
        public List<string> CalculateAllSalaries()
        {
            List<string> salaryReports = new List<string>();
            salaryReports.Add(”--- تقرير الرواتب الشهري ---“);
            double totalCost = 0;

            // التكرار على القائمة الأساسية (StaffMember)
            foreach (var member in _staff)
            {
                // استدعاء دالة CalculateSalary() يختلف حسب نوع الكائن الفعلي
                double salary = member.CalculateSalary();
                totalCost += salary;
                salaryReports.Add($”{member.Name} ({member.GetDepartment()}): الراتب = {salary:N2}”);
            }
            salaryReports.Add($”--- التكلفة الشهرية الإجمالية: {totalCost:N2} ---“);
            return salaryReports;
        }
    }

    // ===================================================================
    // دالة التنفيذ الرئيسية (Program Class)
    // ===================================================================

    class Program
    {
        // دالة لمعالجة الحدث (Event Handler) - توقيعها يطابق التفويض StatusChangeHandler
        public static void LogStatusChange(string staffName, string newStatus)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($”[-- إخطار الحدث --] تم توثيق تغيير حالة: {staffName} أصبحت: {newStatus}”);
            Console.ResetColor();
        }

        static void Main(string[] args)
        {
            //Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine(”=================================================”);
            Console.WriteLine(”  نظام إدارة المستشفى (تطبيق مبادئ OOP متكامل)”);
            Console.WriteLine(”=================================================”);

            HospitalRegistry registry = new HospitalRegistry();

            // 1. إنشاء الكائنات وتطبيق الوراثة والتغليف
            Console.WriteLine(”\n[1] إنشاء وتغليف الكيانات (Inheritance & Encapsulation):”);
            Doctor dr_ahmad = new Doctor(”د. أحمد حسن”, 12, 15000, “قلبية”);
            Nurse nurse_layla = new Nurse(”ليلى عمر”, 5, 5000);
            AdminStaff admin_fadi = new AdminStaff(”فادي ناصر”, 8, 7000);

            // تطبيق التفويض والحدث - اشتراك الدالة في الحدث
            dr_ahmad.OnStatusChanged += LogStatusChange;

            registry.AddStaff(dr_ahmad);
            registry.AddStaff(nurse_layla);
            registry.AddStaff(admin_fadi);

            // استخدام الخاصية الساكنة
            Console.WriteLine($”  -> إجمالي عدد الموظفين (خاصية ساكنة): {HospitalRegistry.TotalStaffCount} موظف.”);
            Console.WriteLine(”------------------------------------------“);

            // 2. تطبيق تعدد الأشكال (Polymorphism)
            Console.WriteLine(”\n[2] توليد تقارير الرواتب (تطبيق تعدد الأشكال):”);
            List<string> reports = registry.CalculateAllSalaries();
            reports.ForEach(Console.WriteLine);
            Console.WriteLine(”------------------------------------------“);

            // 3. اختبار التجريد والتغليف والتحقق الساكن
            Console.WriteLine(”\n[3] اختبار التجريد والتغليف والتحقق الساكن:”);
            Console.WriteLine($”  -> ملخص د. أحمد (تجريد): {dr_ahmad.GetSummary()}”);

            // اختبار التغليف والتحقق الساكن
            try
            {
                Console.WriteLine(”  -> محاولة إدخال اسم غير صالح...”);
                dr_ahmad.Name = “ “; // محاولة تعديل الاسم بقيمة غير صالحة
            }
            catch (ArgumentException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($”  [خطأ التغليف] تم منع العملية: {ex.Message}”);
                Console.ResetColor();
            }
            Console.WriteLine(”------------------------------------------“);

            // 4. اختبار التفويض والأحداث (Delegates & Events)
            Console.WriteLine(”\n[4] اختبار التفويض والأحداث:”);
            Console.WriteLine(”  -> تغيير حالة د. أحمد (سيؤدي لإطلاق حدث مسجل):”);
            dr_ahmad.SetOnDutyStatus(true); // إطلاق الحدث
            dr_ahmad.SetOnDutyStatus(false);
            Console.WriteLine(”=================================================”);
        }
    }
}

