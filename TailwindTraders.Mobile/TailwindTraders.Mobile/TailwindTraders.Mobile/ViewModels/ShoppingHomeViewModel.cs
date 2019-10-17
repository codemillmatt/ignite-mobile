using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Toasts;
using TailwindTraders.Mobile.Helpers;
using TailwindTraders.Mobile.Models;
using TailwindTraders.Mobile.Services;
using Xamarin.Forms;
using System.Linq;

namespace TailwindTraders.Mobile.ViewModels
{
    public class ShoppingHomeViewModel : BaseViewModel<ProductCategoryInfo>
    {
        public ObservableCollection<RecommendedProductCategory> RecommendedCategories { get; }

        ObservableCollection<Product> popularProducts;
        public ObservableCollection<Product> PopularProducts { get; set; }

        ObservableCollection<Product> previouslySeenProducts;
        public ObservableCollection<Product> PreviouslySeenProducts { get; set; }

        public ShoppingHomeViewModel()
        {
            NavigateToProductCategoryCommand = new Command<string>(async (code) => await ExecuteNavigateToProductCategoryCommand(code));
            TakePhotoCommand = new Command(async () => await ExecuteTakePhotoCommand());

            RecommendedCategories = new ObservableCollection<RecommendedProductCategory>
            {
                new RecommendedProductCategory { CategoryName = ProductCategoryConstants.SinkCategoryName, ImageName = "recommended_bathrooms", 
                    CategoryAbbreviation = ProductCategoryConstants.SinkCategoryCode, NavigateCommand = NavigateToProductCategoryCommand },
                new RecommendedProductCategory { CategoryName = ProductCategoryConstants.GardeningCategoryName, ImageName = "recommended_plants", 
                    CategoryAbbreviation = ProductCategoryConstants.GardeningCategoryCode, NavigateCommand = NavigateToProductCategoryCommand },
                new RecommendedProductCategory { CategoryName = ProductCategoryConstants.DIYToolsCategoryName, ImageName = "recommended_powertools", 
                    CategoryAbbreviation = ProductCategoryConstants.DIYToolsCategoryCode, NavigateCommand = NavigateToProductCategoryCommand }
            };

            PopularProducts = new ObservableCollection<Product>();
            PreviouslySeenProducts = new ObservableCollection<Product>();
        }

        public ICommand NavigateToProductCategoryCommand { get; }
        public ICommand TakePhotoCommand { get; }

        async Task ExecuteNavigateToProductCategoryCommand(string code)
        {
            var routingUrl = $"{RoutingConstants.ProductCategoryPage}?categoryCode={code}";

            await Shell.Current.GoToAsync(routingUrl, true);
        }
    
        async Task ExecuteTakePhotoCommand()
        {
            bool success = false;
            Stream photoStream;
            var photoService = new PhotoService();

            var result = await Shell.Current.DisplayActionSheet("Smart Shopping", "Cancel", null, "Take Photo", "Select from Camera Roll");

         
            if (result.Equals("Take Photo", StringComparison.OrdinalIgnoreCase))
                photoStream = await photoService.TakePhoto();
            else
                photoStream = await photoService.PickPhoto();

            try
            {
                IsBusy = true;

                var storage = new AzureStorageService();
                var sas = await storage.GetSharedAccessSignature();

                success = await storage.UploadPhoto(photoStream, sas);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            var toast = new NotificationOptions
            {
                Title = success ? "Upload Succeeded" : "Upload Failed",
                Description = success ? "Photo successfully uploaded" : "There was an error while uploading",
                ClearFromHistory = true,
                IsClickable = false
            };

            var notification = DependencyService.Get<IToastNotificator>();

            await notification.Notify(toast);
        }

        public async Task LoadData()
        {
            if (IsInitialized)
                return;

            // Grab some data from the kitchen and hom appliances categories
            var kitchenItems = await DataStore.GetItemAsync(ProductCategoryConstants.KitchenCategoryCode);
            var appliances = await DataStore.GetItemAsync(ProductCategoryConstants.HomeAppliancesCategoryCode);

            var kitchenItemsSubset = kitchenItems.Products.Take(3);
            var appliancesSubset = appliances.Products.Take(3);

            foreach (var item in kitchenItemsSubset)
            {
                PopularProducts.Add(item);
            }

            foreach (var item in appliancesSubset)
            {
                PreviouslySeenProducts.Add(item);
            }
            
            IsInitialized = true;
        }
    }
}
