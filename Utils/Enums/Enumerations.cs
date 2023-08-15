using System.Runtime.Serialization;

namespace Utils.Enums
{
    public static class Enumerations
    {
        public enum UserRole
        {
            [EnumMember(Value = "Administrator")]
            Admin,

            [EnumMember(Value = "Manager")]
            Manager,

            [EnumMember(Value = "Employee")]
            Employee,

            [EnumMember(Value = "Guest")]
            Guest
        }

        static string GetEnumName<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            var enumMemberAttribute = typeof(TEnum)
                .GetMember(enumValue.ToString())
                .FirstOrDefault()
                ?.GetCustomAttributes(false)
                .OfType<EnumMemberAttribute>()
                .FirstOrDefault();

            return enumMemberAttribute?.Value ?? enumValue.ToString();
        }
    }
}
