using MiniDashboard.App.Utils;
using MiniDashboard.Common.Models;
using System.Text;

namespace MiniDashboard.App.EntityModels
{
    public class EmProductFilter : EmBase
    {
        private const int DEFAULT_MAX_PRICE = 10000;

        public EmStringFilter Name { get; }

        public EmProductFilter()
        {
            Name = new EmStringFilter();
            Name.PropertyChanged += Name_PropertyChanged;
        }

        private void Name_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Summary));
        }

        private decimal m_minPrice = 0;
        public decimal MinPrice
        {
            get => m_minPrice;
            set
            {
                SetProperty(ref m_minPrice, value);
                OnPropertyChanged(nameof(Summary));
            }
        }

        private decimal m_maxPrice = DEFAULT_MAX_PRICE;
        public decimal MaxPrice
        {
            get => m_maxPrice;
            set
            {
                SetProperty(ref m_maxPrice, value);
                OnPropertyChanged(nameof(Summary));
            }
        }

        public string Summary { get => GetSummary(); }

        public string GetSummary()
        {
            var sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(Name.FilterText) && MinPrice == 0 && MaxPrice == DEFAULT_MAX_PRICE)
                sb.Append("No filter applied");
            else
            {
                var fitems = new List<string>();
                if (!string.IsNullOrWhiteSpace(Name.FilterText))
                    fitems.Add($"Name {Name.SelectedFilterType.GetDescription()} {Name.FilterText}");

                if (MinPrice != 0)
                    fitems.Add($"Price greater than {MinPrice}");

                if (MaxPrice != DEFAULT_MAX_PRICE)
                    fitems.Add($"Price less than {MaxPrice}");

                sb.Append(string.Join(" and ", fitems));
            }

            return sb.ToString();
        }

        public static explicit operator ProductFilter(EmProductFilter em)
        {
            if (em == null)
                throw new ArgumentNullException(nameof(em));

            var result = new ProductFilter
            {
                MinPrice = em.MinPrice,
                MaxPrice = em.MaxPrice
            };

            if (em.Name != null && !string.IsNullOrWhiteSpace(em.Name.FilterText))
            {
                result.Name = new StringFilter
                {
                    FilterType = em.Name.SelectedFilterType,
                    Value = em.Name.FilterText
                };
            }
            else
            {
                result.Name = null;
            }

            return result;
        }
    }
}
