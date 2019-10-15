using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.ViewModels
{
    [QueryProperty("ProductName", "productName")]
    [QueryProperty("Price", "price")]
    [QueryProperty("ImageUrl", "imageUrl")]
    [QueryProperty("BrandUrl", "brandUrl")]
    [QueryProperty("CategoryCode", "categoryCode")]
    [QueryProperty("QuantityRemaining", "quantityRemaining")]
    public class ProductDetailViewModel : BaseViewModel
    {
        string productName;
        public string ProductName { get => productName;
            set
            {
                SetProperty(ref productName, Uri.UnescapeDataString(value));
            }
        }

        string price;
        public string Price { get => price; set => SetProperty(ref price, value); }

        string imageUrl;
        public string ImageUrl { get => imageUrl;
            set
            {
                SetProperty(ref imageUrl, Uri.UnescapeDataString(value));
            }
        }

        string brandName;
        public string BrandName { get => brandName; set => SetProperty(ref brandName, value); }

        string categoryCode;
        public string CategoryCode { get => categoryCode; set => SetProperty(ref categoryCode, value); }

        string quantityRemaining;
        public string QuantityRemaining { get => quantityRemaining; set => SetProperty(ref quantityRemaining, value); }
    }
}