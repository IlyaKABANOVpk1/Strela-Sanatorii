using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Strela_Sanatorii.Utils
{
    public static class ValidationHelper
    {
        // Только буквы русского/латинского алфавита и дефис
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return Regex.IsMatch(name.Trim(), @"^[a-zA-Zа-яА-ЯёЁ\-]+$");
        }

        // Табельный номер: только цифры, 3-10 символов
        public static bool IsValidPersonnelNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return false;
            return Regex.IsMatch(number.Trim(), @"^\d{3,10}$");
        }

        // Телефон: +7 (XXX) XXX-XX-XX
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return true; // Необязательное
            return Regex.IsMatch(phone.Trim(), @"^\+7\s\(\d{3}\)\s\d{3}-\d{2}-\d{2}$");
        }

        public static string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;
            // Убираем всё кроме цифр
            var digits = Regex.Replace(phone, @"\D", "");
            if (digits.Length == 11 && digits.StartsWith("7"))
                digits = digits.Substring(1);
            if (digits.Length == 10)
                return $"+7 ({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 2)}-{digits.Substring(8, 2)}";
            return phone;
        }

        // Положительное число
        public static bool IsPositiveInt(string value, out int result)
        {
            result = 0;
            if (!int.TryParse(value, out int num)) return false;
            result = num;
            return num > 0;
        }
    }
}
