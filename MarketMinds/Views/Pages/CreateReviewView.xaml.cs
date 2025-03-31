using Microsoft.UI.Xaml.Controls;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Newtonsoft.Json;

namespace MarketMinds
{
    public sealed partial class CreateReviewView : Window
    {
        public ReviewCreateViewModel ViewModel { get; set; }
        private readonly bool _isEditing;

        public CreateReviewView(ReviewCreateViewModel viewModel, Review existingReview = null)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
            this.Closed += OnWindowClosed;

            if (existingReview != null)
            {
                _isEditing = true;
                ViewModel.Description = existingReview.description;
                ViewModel.Images = existingReview.images;
                ViewModel.Rating = existingReview.rating;
                ViewModel.CurrentReview = existingReview;
            }
        }

        private async void OnUploadImageClick(object sender, RoutedEventArgs e)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var picker = new FileOpenPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                string imgurLink = await UploadToImgur(file);
                if (!string.IsNullOrEmpty(imgurLink))
                {
                    ViewModel.ImagesString = string.IsNullOrEmpty(ViewModel.ImagesString)
                        ? imgurLink
                        : ViewModel.ImagesString + "\n" + imgurLink;
                }
            }
        }

        private void handleSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                Debug.WriteLine("ViewModel is null!");
                return;
            }

            if (ViewModel.ImagesString.Contains("uploading...", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Please wait for image upload to complete.");
                return;
            }

            if (_isEditing)
            {
                ViewModel.UpdateReview();  // Call an update method instead of submitting a new review
            }
            else
            {
                ViewModel.SubmitReview();
            }

            this.Close();
        }

        private async Task<string> UploadToImgur(StorageFile file)
        {
            ViewModel.ImagesString += "\nUploading...";

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                byte[] buffer = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(buffer);
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", App.configuration.GetSection("ImgurSettings:ClientId").Value);

                    var content = new MultipartFormDataContent
            {
                { new ByteArrayContent(buffer), "image" }
            };

                    HttpResponseMessage response = await client.PostAsync("https://api.imgur.com/3/image", content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                    string link = jsonResponse?.data?.link;

                    // Remove "Uploading..." placeholder
                    ViewModel.ImagesString = ViewModel.ImagesString.Replace("\nUploading...", "");

                    return link;
                }
            }
        }

        private void OnWindowClosed(object sender, Microsoft.UI.Xaml.WindowEventArgs e)
        {
            // Clear ViewModel properties when the window is closed
            ViewModel.Description = string.Empty;
            ViewModel.ImagesString = string.Empty;
            ViewModel.Rating = 0;
        }

    }
}