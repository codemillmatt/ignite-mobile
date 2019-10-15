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

namespace TailwindTraders.Mobile.ViewModels
{
    public class ShoppingHomeViewModel : BaseViewModel<RecommendedProductCategory>
    {
        public ObservableCollection<RecommendedProductCategory> RecommendedCategories { get; }

        public ShoppingHomeViewModel()
        {
            NavigateToProductCategoryCommand = new Command<string>(async (code) => await ExecuteNavigateToProductCategoryCommand(code));
            TakePhotoCommand = new Command(async () => await ExecuteTakePhotoCommand());

            RecommendedCategories = new ObservableCollection<RecommendedProductCategory>
            {
                new RecommendedProductCategory { CategoryName = "Sink", ImageName = "recommended_bathrooms", 
                    CategoryAbbreviation ="sink", NavigateCommand = NavigateToProductCategoryCommand },
                new RecommendedProductCategory { CategoryName = "Gardening", ImageName = "recommended_plants", 
                    CategoryAbbreviation = "gardening", NavigateCommand = NavigateToProductCategoryCommand },
                new RecommendedProductCategory { CategoryName = "DIY Tools", ImageName = "recommended_powertools", 
                    CategoryAbbreviation = "diytools", NavigateCommand = NavigateToProductCategoryCommand }
            };            
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

    }
}
