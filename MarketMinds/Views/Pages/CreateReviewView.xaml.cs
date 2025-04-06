using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using ViewModelLayer.ViewModel;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using DomainLayer.Domain;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using MarketMinds.Services;

namespace MarketMinds
{
    public sealed partial class CreateReviewView : Window
    {
        public ReviewCreateViewModel ViewModel { get; set; }
        private readonly bool var_isEditing;
        private readonly ImageUploadService imageUploadService;

        public CreateReviewView(ReviewCreateViewModel viewModel, Review? review = null)
        {
            ViewModel = viewModel;
            imageUploadService = new ImageUploadService();
            var_isEditing = review != null;
            if (var_isEditing)
            {
                ViewModel.CurrentReview = review;
                ViewModel.Description = review.Description;
                ViewModel.Rating = review.Rating;
                ViewModel.Images = review.Images;
            }
            this.InitializeComponent();
            this.Closed += OnWindowClosed;
        }

        private async void HandleAddImage_Click(object sender, RoutedEventArgs e)
        {
            string imgurLink = await imageUploadService.UploadImage(this);
            if (!string.IsNullOrEmpty(imgurLink))
            {
                ViewModel.ImagesString = string.IsNullOrEmpty(ViewModel.ImagesString)
                    ? imgurLink
                    : ViewModel.ImagesString + "\n" + imgurLink;
            }
        }

        private void HandleSubmit_Click(object sender, RoutedEventArgs e)
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

            if (var_isEditing)
            {
                ViewModel.UpdateReview();  // Call an update method instead of submitting a new review
            }
            else
            {
                ViewModel.SubmitReview();
            }

            this.Close();
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