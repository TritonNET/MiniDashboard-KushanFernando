using System.ComponentModel;

namespace MiniDashboard.Common.Models
{
    public enum StringFilterType
    {
        [Description("Contains")]
        Contains = 1,

        [Description("Starts with")]
        StartsWith = 2,

        [Description("Ends with")]
        EndsWith = 3,

        [Description("Exact match")]
        Exact = 4
    }
}
