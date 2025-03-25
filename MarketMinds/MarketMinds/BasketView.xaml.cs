using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using ViewModelLayer.ViewModel;
using DomainLayer.Domain;
using System.Linq;
using System.Collections.Generic;

namespace UiLayer
{
    public sealed partial class BasketView : Window
    {
        private BasketViewModel _basketViewModel;
        private List<BasketItem> _basketItems;
        private User _currentUser; // This should come from a logged-in user session, for now it s just a demo

        public BasketView()
        {
            this.InitializeComponent();

            // Initialize test user for demo run
            _currentUser = new User(1, "TestUser", "test@example.com");

            // Get the BasketViewModel from the application
            InitializeViewModel();

            // Load basket data
            LoadBasketData();
        }

        private void InitializeViewModel()
        {
            _basketViewModel = new BasketViewModel(_currentUser);
        }

        private async void ShowErrorMessage(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void LoadBasketData()
        {
            try
            {
                // Refresh basket data from the view model
                _basketViewModel.LoadBasket();

                // Get the updated basket items
                _basketItems = new List<BasketItem>(_basketViewModel.BasketItems);

                // Set the ListView's data source
                BasketItemsListView.ItemsSource = null;
                BasketItemsListView.ItemsSource = _basketItems;

                // Update UI elements
                UpdateUIElements();
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error loading basket data: {ex.Message}");

                // Show error message to user
                ErrorMessageTextBlock.Text = $"Error loading basket: {ex.Message}";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void UpdateUIElements()
        {
            // Update item count
            int itemCount = _basketItems.Sum(item => item.Quantity);
            ItemCountTextBlock.Text = $"{itemCount} item{(itemCount != 1 ? "s" : "")} in your basket";

            // Update price displays
            SubtotalTextBlock.Text = $"${_basketViewModel.Subtotal:F2}";
            DiscountTextBlock.Text = $"-${_basketViewModel.Discount:F2}";
            TotalTextBlock.Text = $"${_basketViewModel.TotalAmount:F2}";

            // Show empty basket message if there are no items
            if (_basketItems.Count == 0)
            {
                EmptyBasketMessage.Visibility = Visibility.Visible;
                BasketItemsListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyBasketMessage.Visibility = Visibility.Collapsed;
                BasketItemsListView.Visibility = Visibility.Visible;
            }

            // Enable/disable checkout button based on whether checkout is possible
            CheckoutButton.IsEnabled = _basketViewModel.CanCheckout();

            // Show any error messages
            if (!string.IsNullOrEmpty(_basketViewModel.ErrorMessage))
            {
                ErrorMessageTextBlock.Text = _basketViewModel.ErrorMessage;
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void HandleRemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.CommandParameter is int basketItemId)
            {
                // Remove the item through the view model
                _basketViewModel.RemoveItem(basketItemId);

                // Reload basket data to fully refresh the UI
                LoadBasketData();
            }
        }

        private void HandleIncreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.CommandParameter is int basketItemId)
            {
                // Find the current basket item
                var basketItem = _basketItems.FirstOrDefault(item => item.Id == basketItemId);
                if (basketItem != null)
                {
                    // Update quantity through the view model
                    _basketViewModel.UpdateQuantity(basketItemId, basketItem.Quantity + 1);

                    // Force UI update by completely reloading the ListView
                    BasketItemsListView.ItemsSource = null;
                    BasketItemsListView.ItemsSource = _basketItems;

                    // Update other UI elements (totals, etc.)
                    UpdateUIElements();
                }
            }
        }

        private void HandleDecreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.CommandParameter is int basketItemId)
            {
                // Find the current basket item
                var basketItem = _basketItems.FirstOrDefault(item => item.Id == basketItemId);
                if (basketItem != null && basketItem.Quantity > 1)
                {
                    // Update quantity through the view model
                    _basketViewModel.UpdateQuantity(basketItemId, basketItem.Quantity - 1);

                    // Force UI update by completely reloading the ListView
                    BasketItemsListView.ItemsSource = null;
                    BasketItemsListView.ItemsSource = _basketItems;

                    // Update other UI elements (totals, etc.)
                    UpdateUIElements();
                }
                else if (basketItem != null && basketItem.Quantity == 1)
                {
                    // If quantity would go to 0, remove the item
                    _basketViewModel.RemoveItem(basketItemId);
                    LoadBasketData(); // Complete refresh for removals
                }
            }
        }

        private void QuantityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateQuantityFromTextBox(sender as TextBox);
        }

        private void QuantityTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                UpdateQuantityFromTextBox(sender as TextBox);
                e.Handled = true;
            }
        }

        private void UpdateQuantityFromTextBox(TextBox textBox)
        {
            if (textBox != null && textBox.Tag is int basketItemId)
            {
                if (int.TryParse(textBox.Text, out int newQuantity) && newQuantity >= 0)
                {
                    if (newQuantity == 0)
                    {
                        // If quantity is 0, remove the item
                        _basketViewModel.RemoveItem(basketItemId);
                        LoadBasketData(); // Full refresh for item removal
                    }
                    else
                    {
                        // Update quantity
                        _basketViewModel.UpdateQuantity(basketItemId, newQuantity);

                        // Force UI update
                        BasketItemsListView.ItemsSource = null;
                        BasketItemsListView.ItemsSource = _basketItems;
                        UpdateUIElements();
                    }
                }
                else
                {
                    // Invalid input, reset to current quantity
                    var basketItem = _basketItems.FirstOrDefault(item => item.Id == basketItemId);
                    if (basketItem != null)
                    {
                        textBox.Text = basketItem.Quantity.ToString();
                    }
                }
            }
        }

        private void HandleApplyPromoCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string promoCode = PromoCodeTextBox.Text?.Trim();
            if (!string.IsNullOrEmpty(promoCode))
            {
                try
                {
                    _basketViewModel.ApplyPromoCode(promoCode);
                    LoadBasketData(); // Refresh the data
                }
                catch (Exception ex)
                {
                    ErrorMessageTextBlock.Text = $"Error applying promo code: {ex.Message}";
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
            }
        }

        private void HandleClearBasketButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Clear Basket",
                Content = "Are you sure you want to remove all items from your basket?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                XamlRoot = Content.XamlRoot
            };

            _ = dialog.ShowAsync().AsTask().ContinueWith(t =>
            {
                if (t.Result == ContentDialogResult.Primary)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        _basketViewModel.ClearBasket();
                        LoadBasketData(); // Refresh the data
                    });
                }
            });
        }

        private void HandleCheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_basketViewModel.CanCheckout())
            {
                _basketViewModel.Checkout();
                // Navigate to checkout page or process checkout
                ShowCheckoutMessage();
            }
            else
            {
                ErrorMessageTextBlock.Text = "Unable to proceed to checkout. Please check your basket items.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }

        private async void ShowCheckoutMessage()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Checkout",
                Content = "This would navigate to the checkout process. Checkout functionality is not yet implemented.",
                CloseButtonText = "OK",
                XamlRoot = Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void HandleContinueShoppingButton_Click(object sender, RoutedEventArgs e)
        {
            // This would typically navigate back to the product listing page
            this.Close();
        }

        private void handleCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}