using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using TailwindTraders.Mobile.Helpers;
using TailwindTraders.Mobile.Models;
using Xamarin.Essentials;

namespace TailwindTraders.Mobile.Services
{
    public class AzureStorageService
    {
        public async Task<string> GetSharedAccessSignature()
        {
            try
            {
                var functionUrl = Preferences.Get(PreferencesConstants.FunctionAppUrlKey, PreferencesConstants.DemoFunctionsUrl);

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

                var sas = await client.GetStringAsync($"{functionUrl}/getsastoken");

                return sas;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return string.Empty;
        }

        public async Task<IEnumerable<WishlistItem>> GetWishlistItems()
        {
            return await Task.FromResult(new List<WishlistItem>());
        }

        public async Task<bool> UploadPhoto(Stream photo, string sharedAccessSignature)
        {
            try
            {
                var creds = new Microsoft.Azure.Storage.Auth.StorageCredentials(sharedAccessSignature);

                var storageAccountName = Preferences.Get(PreferencesConstants.StorageAccountNameKey,
                    PreferencesConstants.DemoStorageName);

                var account = new CloudStorageAccount(creds, storageAccountName, "core.windows.net", true);

                var blobClient = account.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference("wishlist");

                var blockBlob = container.GetBlockBlobReference($"TT-{Guid.NewGuid()}.jpg");

                await blockBlob.UploadFromStreamAsync(photo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }

            return true;
        }
    }
}
