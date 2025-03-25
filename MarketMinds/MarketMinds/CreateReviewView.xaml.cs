using Microsoft.UI.Xaml.Controls;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using Microsoft.UI.Xaml;

namespace MarketMinds
{
    public sealed partial class CreateReviewView : Window
    {
        public ReviewCreateViewModel ViewModel { get; set; }

        public CreateReviewView(ReviewCreateViewModel viewModel)
        {
            this.InitializeComponent();
            ViewModel = viewModel;

        }
    }
}
