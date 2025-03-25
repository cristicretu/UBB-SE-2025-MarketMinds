using Microsoft.UI.Xaml.Controls;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;

namespace MarketMinds
{
    public sealed partial class CreateReviewView : Window
    {
        public ReviewCreateViewModel ViewModel { get; set; }

        public CreateReviewView(ReviewCreateViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }

        private void handleSubmit_Click(Object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                Debug.WriteLine("ViewModel is null!");
                return;
            }

            ViewModel.SubmitReview();
            this.Close();
        }
    }
}
