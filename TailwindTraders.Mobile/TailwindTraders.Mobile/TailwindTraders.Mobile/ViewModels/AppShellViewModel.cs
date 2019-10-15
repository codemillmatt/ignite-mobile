using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TailwindTraders.Mobile.Helpers;
using Xamarin.Forms;

namespace TailwindTraders.Mobile.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        public ICommand NavigateToProductCategoryCommand { get; }

        public AppShellViewModel()
        {
            NavigateToProductCategoryCommand = new Command<string>(async (code) => await ExecuteNavigateToProductCategoryCommand(code));
        }

        private async Task ExecuteNavigateToProductCategoryCommand(string code)
        {
            var routeWithData = $"{RoutingConstants.ProductCategoryPage}?categoryCode={code}";

            await Shell.Current.GoToAsync(routeWithData, true);
        }
    }    
}
