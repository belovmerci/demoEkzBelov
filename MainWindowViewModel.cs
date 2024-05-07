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

        private ObservableCollection<string> _manufacturers;
        public ObservableCollection<string> Manufacturers
        {
            get { return _manufacturers; }
            set
            {
                _manufacturers = value;
                OnPropertyChanged(nameof(Manufacturers));
            }
        }

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
                if (_sortAscending)
                {
                    SortAscendingText = "Сортировать по цене вверх";
                }
                else
                {
                    SortAscendingText = "Сортировать по цене вниз";
                }
                PerformSearchAndSort();
            }
        }

        private string _sortAscendingText = "Сортировать по цене вверх";
        public string SortAscendingText
        {
            get { return _sortAscendingText; }
            set
            {
                _sortAscendingText = value;
                OnPropertyChanged(nameof(SortAscendingText));
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

        private void MoveToFirstPage()
        {
            CurrentPage = 1;
        }

        private void MoveToPreviousPage()
        {
            CurrentPage--;
        }

        private void MoveToNextPage()
        {
            CurrentPage++;
        }

        private void MoveToLastPage()
        {
            CurrentPage = TotalPages;
        }

        public ICommand AddTestEntriesCommand { get; }
        public ICommand MoveToFirstPageCommand { get; }
        public ICommand MoveToPreviousPageCommand { get; }
        public ICommand MoveToNextPageCommand { get; }
        public ICommand MoveToLastPageCommand { get; }

        public MainWindowViewModel()
        {
            // Initialize commands
            AddTestEntriesCommand = new RelayCommand(AddTestEntries);

            MoveToFirstPageCommand = new RelayCommand(MoveToFirstPage);
            MoveToPreviousPageCommand = new RelayCommand(MoveToPreviousPage);
            MoveToNextPageCommand = new RelayCommand(MoveToNextPage);
            MoveToLastPageCommand = new RelayCommand(MoveToLastPage);

            // Initialize manufacturers
            Manufacturers = new ObservableCollection<string>();
            // Add "Все производители" option
            Manufacturers.Add("Все производители");
            SelectedManufacturer = Manufacturers.FirstOrDefault();

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

            // Update list of manufacturers whenever adding new entries (potential new manufacturers)
            var manufacturers = OriginalProducts.Select(p => p.Manufacturer).Distinct().ToList();
            manufacturers.Insert(0, "Все производители");
            Manufacturers = new ObservableCollection<string>(manufacturers);
            _selectedManufacturer = "Все производители";

            PerformSearchAndSort();
        }

        private void PerformSearchAndSort()
        {
            // Filter products based on search text, manufacturer, and sorting option
            var filteredProducts = OriginalProducts.Where(p =>
                (string.IsNullOrWhiteSpace(SearchText) ||
                p.Name.ToLower().Contains(SearchText.ToLower()) ||
                p.Description.ToLower().Contains(SearchText.ToLower())) &&
                (SelectedManufacturer == "Все производители" || p.Manufacturer == SelectedManufacturer))
                .ToList();

            // Sort filtered products based on the selected sorting option
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

            // Update total entries and total pages
            TotalEntries = filteredProducts.Count;
            TotalPages = (int)Math.Ceiling((double)TotalEntries / _pageSize);

            // Update current page if it exceeds total pages
            if (_currentPage > TotalPages)
            {
                _currentPage = TotalPages;
            }

            // Update the current page of products
            LoadCurrentPageItems();
        }

        private void LoadCurrentPageItems()
        {
            // Anticipating user-induced errors
            if (_currentPage > TotalPages)
            {
                _currentPage = _totalPages;
                OnPropertyChanged(nameof(CurrentPage));
            }
            else if (_currentPage < 1)
            {
                _currentPage = 1;
                OnPropertyChanged(nameof(CurrentPage));
            }

            // Update the current page index
            _currentPageIndex = _currentPage - 1;

            // Based on the current page, make a selection to show
            CurrentProducts.Clear();
            foreach (var product in Products.Skip(_currentPageIndex * _pageSize).Take(_pageSize))
            {
                CurrentProducts.Add(product);
            }
        }

        public string PageInfo => $"Showing {Math.Min((_currentPageIndex * _pageSize) + 1, TotalEntries)}-{Math.Min((_currentPageIndex + 1) * _pageSize, TotalEntries)} of {TotalEntries} entries";
        // OnPropertyChanged raises PropertyChanged event for PageInfo
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Raise PropertyChanged for PageInfo whenever any of its dependent properties change
            if (propertyName == nameof(_currentPageIndex) || propertyName == nameof(_pageSize) || propertyName == nameof(TotalEntries))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageInfo)));
            }
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