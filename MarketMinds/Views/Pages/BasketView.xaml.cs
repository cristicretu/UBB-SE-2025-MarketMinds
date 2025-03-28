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
        private ObservableCollection<BasketItem> _basketItems;
        private User _currentUser;

        public BasketView()
        {
            this.InitializeComponent();

            // Get the current user from the app
            _currentUser = MarketMinds.App.currentUser;

            // Get the BasketViewModel from the app
            _basketViewModel = MarketMinds.App.basketViewModel;

            // Initialize basket items as ObservableCollection for auto-UI updates
            _basketItems = new ObservableCollection<BasketItem>();

            // Set the ListView's data source
            BasketItemsListView.ItemsSource = _basketItems;

            // Load basket data
            LoadBasketData();
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

                // Clear the current basket items
                _basketItems.Clear();

                // Add all items from the view model
                foreach (var item in _basketViewModel.BasketItems)
                {
                    _basketItems.Add(item);
                }

                // Update UI elements
                UpdateUIElements();
            }
            catch (Exception ex)
            {
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
            try
            {
                Button button = sender as Button;
                if (button != null && button.CommandParameter is int itemId)
                {
                    // Find the corresponding basket item
                    var item = _basketItems.FirstOrDefault(i => i.Id == itemId);
                    if (item != null)
                    {
                        // Remove using product ID instead of item ID
                        _basketViewModel.RemoveProductFromBasket(item.Product.Id);
                    }
                    LoadBasketData();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to remove item: {ex.Message}");
            }
        }

        private void HandleIncreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null && button.CommandParameter is int itemId)
                {
                    // Find the corresponding basket item
                    var basketItem = _basketItems.FirstOrDefault(item => item.Id == itemId);
                    if (basketItem != null)
                    {
                        // Update quantity using product ID
                        _basketViewModel.UpdateProductQuantity(basketItem.Product.Id, basketItem.Quantity + 1);

                        // Reload the basket data
                        LoadBasketData();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to increase quantity: {ex.Message}");
            }
        }

        private void HandleDecreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button != null && button.CommandParameter is int itemId)
                {
                    // Find the corresponding basket item
                    var basketItem = _basketItems.FirstOrDefault(item => item.Id == itemId);
                    if (basketItem != null)
                    {
                        if (basketItem.Quantity > 1)
                        {
                            // Update quantity using product ID
                            _basketViewModel.UpdateProductQuantity(basketItem.Product.Id, basketItem.Quantity - 1);
                        }
                        else
                        {
                            // Remove item if quantity would go to 0
                            _basketViewModel.RemoveProductFromBasket(basketItem.Product.Id);
                        }

                        // Reload the basket data
                        LoadBasketData();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to decrease quantity: {ex.Message}");
            }
        }

        private void UpdateQuantityFromTextBox(TextBox textBox)
        {
            try
            {
                if (textBox != null && textBox.Tag is int itemId)
                {
                    if (int.TryParse(textBox.Text, out int newQuantity) && newQuantity >= 0)
                    {
                        // Find the corresponding basket item
                        var basketItem = _basketItems.FirstOrDefault(item => item.Id == itemId);
                        if (basketItem != null)
                        {
                            _basketViewModel.UpdateProductQuantity(basketItem.Product.Id, newQuantity);

                            // Reload the basket data
                            LoadBasketData();
                        }
                    }
                    else
                    {
                        // Invalid input, reset to current quantity
                        var basketItem = _basketItems.FirstOrDefault(item => item.Id == itemId);
                        if (basketItem != null)
                        {
                            textBox.Text = basketItem.Quantity.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to update quantity: {ex.Message}");
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

        private void HandleApplyPromoCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string promoCode = PromoCodeTextBox.Text?.Trim();
            if (!string.IsNullOrEmpty(promoCode))
            {
                try
                {
                    _basketViewModel.ApplyPromoCode(promoCode);

                    // The promo code was applied successfully if it gets here
                    if (_basketViewModel.Discount > 0)
                    {
                        ErrorMessageTextBlock.Text = $"Promo code '{promoCode}' applied successfully!";
                        ErrorMessageTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                        ErrorMessageTextBlock.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ErrorMessageTextBlock.Text = "Promo code applied, but no discount was awarded.";
                        ErrorMessageTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                        ErrorMessageTextBlock.Visibility = Visibility.Visible;
                    }

                    LoadBasketData(); 
                }
                catch (Exception ex)
                {
                    ErrorMessageTextBlock.Text = $"Error applying promo code: {ex.Message}";
                    ErrorMessageTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ErrorMessageTextBlock.Text = "Please enter a promo code.";
                ErrorMessageTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
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
                        LoadBasketData();
                    });
                }
            });
        }

        private void HandleCheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_basketViewModel.CanCheckout())
            {
                _basketViewModel.Checkout();
                // Navigate to checkout page
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
            this.Close();
        }
    }
}