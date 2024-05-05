using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoEkzBelov
{
    public static class Database
    {
        public static ObservableCollection<ProductModel> GetProductsFromDatabase()
        {
            // Simulated fetching data from a database
            ObservableCollection<ProductModel> productModels = new ObservableCollection<ProductModel>();
            for (int i = 0; i < 5; i++)
            {
                productModels.Add(RandomProductModel.GenerateRandomProduct());
            }
            return productModels;

        }
    }
}