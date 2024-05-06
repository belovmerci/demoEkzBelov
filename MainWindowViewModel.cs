using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace demoEkzBelov
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ProductModel> _originalProducts = new ObservableCollection<ProductModel>();
        public ObservableCollection<ProductModel> OriginalProducts
        {
            get { return _originalProducts; }
            set
            {
                _originalProducts = value;
                OnPropertyChanged(nameof(OriginalProducts));
                Products = _originalProducts;
                PerformSearchAndSort();
}
        }

        private ObservableCollection<ProductModel> _products = new ObservableCollection<ProductModel>();
        public ObservableCollection<ProductModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }


        private int _currentPageIndex = 0;
        private int _pageSize = 10;
        private ObservableCollection<ProductModel> _currentProducts = new ObservableCollection<ProductModel>();
        public ObservableCollection<ProductModel> CurrentProducts
        {
            get { return _currentProducts; }
            set
            {
                _currentProducts = value;
                OnPropertyChanged(nameof(CurrentProducts));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                PerformSearchAndSort();
            }
        }

        public ObservableCollection<string> Manufacturers { get; set; }

        private string _selectedManufacturer;
        public string SelectedManufacturer
        {
            get { return _selectedManufacturer; }
            set
            {
                _selectedManufacturer = value;
                PerformSearchAndSort();
            }
        }

        private bool _sortAscending = true;
        public bool SortAscending
        {
            get { return _sortAscending; }
            set
            {
                _sortAscending = value;
                PerformSearchAndSort();
            }
        }

        private int _totalEntries;
        public int TotalEntries
        {
            get { return _totalEntries; }
            private set
            {
                _totalEntries = value;
                OnPropertyChanged(nameof(TotalEntries));
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get { return _totalPages; }
            private set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                LoadCurrentPageItems();
            }
        }

        public ICommand AddTestEntriesCommand { get; }

        public MainWindowViewModel()
        {
            // Initialize commands
            AddTestEntriesCommand = new RelayCommand(AddTestEntries);

            // Add manufacturers
            var manufacturers = OriginalProducts.Select(p => p.Manufacturer).Distinct().ToList();
            manufacturers.Insert(0, "All Manufacturers");
            Manufacturers = new ObservableCollection<string>(manufacturers);
            _selectedManufacturer = "All Manufacturers";

            PerformSearchAndSort();
        }

        private async void AddTestEntries()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    OriginalProducts.Add(RandomProductModel.GenerateRandomProduct());
                }
            });

            PerformSearchAndSort();
        }

        private void PerformSearchAndSort()
        {
            // Filter products based on search text and manufacturer
            var filteredProducts = OriginalProducts.Where(p =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                p.Name.ToLower().Contains(SearchText.ToLower()) ||
                p.Description.ToLower().Contains(SearchText.ToLower())) &&
                (SelectedManufacturer == "All Manufacturers" || p.Manufacturer == SelectedManufacturer))
                .ToList();

            // Sort filtered ascending/descending by price
            if (SortAscending)
            {
                filteredProducts = filteredProducts.OrderBy(p => p.Price).ToList();
            }
            else
            {
                filteredProducts = filteredProducts.OrderByDescending(p => p.Price).ToList();
            }

            // Update total entries and total pages
            TotalEntries = filteredProducts.Count;
            TotalPages = (int)Math.Ceiling((double)TotalEntries / _pageSize);

            Products.Clear();
            foreach (var product in filteredProducts)
            {
                Products.Add(product);
            }

            // Load 1st page of updated items
            CurrentPage = 1;
            // LoadCurrentPageItems();
            // Is called on updating CurrentPage
        }

        private void LoadCurrentPageItems()
        {
            // Anticipating user induced errors
            if (_currentPage > TotalPages)
            {
                _currentPage = _totalPages;
            }
            else if (_currentPage < 1)
            {
                _currentPage = 1;
            }

            // Based of current page make a selection to show
            CurrentProducts.Clear();
            foreach (var product in Products.Skip(_currentPageIndex * _pageSize).Take(_pageSize))
            {
                CurrentProducts.Add(product);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }




    // Converter to convert availability boolean to color
    public class AvailabilityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool available)
            {
                return available ? Brushes.Green : Brushes.Red;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter to convert availability boolean to text
    public class AvailabilityToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool available)
            {
                return available ? "Available" : "Unavailable";
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
