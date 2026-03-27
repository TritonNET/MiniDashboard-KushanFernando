using System.ComponentModel;
using System.Reflection;

namespace MiniDashboard.App.Utils
{
    public static class ExtensionMethods
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field!.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }
    }
}
