using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace TestMVC.Util
{
    public static class EnumHelper
    {
        public static IEnumerable<string> GetDisplayNames<TEnum> () where TEnum : Enum
        {
            return typeof(TEnum).GetMembers(BindingFlags.Static | BindingFlags.Public).Select(m => m.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? m.ToString()!);
        }
    }
}
