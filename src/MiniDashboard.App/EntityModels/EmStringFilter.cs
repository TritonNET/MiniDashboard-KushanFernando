using MiniDashboard.App.Utils;
using MiniDashboard.Common.Models;

namespace MiniDashboard.App.EntityModels
{
    public class EmStringFilter : EmBase
    {
        public Dictionary<string, StringFilterType> FilterTypes { get; }

        public EmStringFilter()
        {
            FilterTypes = Enum.GetValues(typeof(StringFilterType))
                            .Cast<StringFilterType>()
                            .ToDictionary(
                                e => e.GetDescription(),
                                e => e
                            );

            m_selectedFilterType = StringFilterType.Contains;
            m_filterText = string.Empty;
        }

        private StringFilterType m_selectedFilterType;
        public StringFilterType SelectedFilterType
        {
            get => m_selectedFilterType;
            set => SetProperty(ref m_selectedFilterType, value);
        }

        private string m_filterText;
        public string FilterText
        {
            get => m_filterText;
            set => SetProperty(ref m_filterText, value);
        }
    }
}
