using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ViewModelLayer.ViewModel;
using BusinessLogicLayer.Services;

namespace UiLayer
{
    public sealed partial class BasketView : Window
    {
        public BasketView()
        {
            InitializeComponent();
        }

        private void handleCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
