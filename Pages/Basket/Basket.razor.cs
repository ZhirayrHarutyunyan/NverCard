using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NverCard.Public.Client.Services.ShoppingCartService;
using System.Collections.Generic;
using NverCard.Public.Client.ServiceProxies.Queries.ShoppingCarts.Models;
using System.Linq;
using System.Globalization;
using NverCard.Public.Client.Modal.LoginModal;
using System;

namespace NverCard.Public.Client.Pages.Basket
{
   public partial class Basket
   {

      #region Inject
      [Inject]
      IShoppingCartService ShoppingCartService { get; set; }

      [Inject]
      UserSession UserSession { get; set; }

      [Inject]
      NavigationManager NavigationManager { get; set; }

      #endregion Inject

      #region Properties

      private List<ShoppingCartItemModel> ShopingCartItems { get; set; }

      private bool CartIsLoading { get; set; }

      #endregion Properties

      #region Reference 

      /// <summary>
      /// ссылка на модал
      /// </summary>
      private Login modalLogin;

      #endregion Reference

      #region Methods
      private void LoginOrContinue()
      {
         if (UserSession.IsSessionStarted)
            NavigationManager.NavigateTo("/checkout");
         else
            modalLogin.openModalLogin();
      }

      private async Task RemoveItemFromShoppingCart(ShoppingCartItemModel model)
      {
         await ShoppingCartService.RemoveItemFromCustomerShoppingCartAsync(model.Id);

         await GetShoppingCartItems();
      }

      /// <summary>
      /// Return cart items
      /// </summary>
      public async Task<List<ShoppingCartItemModel>> GetShoppingCartItems()
      {
         await UserSession.LoadSession();
         ShopingCartItems = (await ShoppingCartService.GetCustomerShoppingCartAsync())?.CartItems.ToList();

         return ShopingCartItems == null ? new List<ShoppingCartItemModel>() : ShopingCartItems;
      }

      /// <summary>
      /// Return cart items count
      /// </summary>
      public async Task<int> GetShoppingCartItemsCount()
      {
         return await ShoppingCartService.GetShoppingCartItemsCount();
      }

      /// <summary>
      /// Return cart total price
      /// </summary>
      protected int GetCartTotalPrice()
      {
         return (ShopingCartItems != null && ShopingCartItems.Count > 0) ? ShopingCartItems.Sum(item => item.Price * item.Quantity) : 0;
      }

      protected override async Task OnInitializedAsync()
      {
         CartIsLoading = true;

         ShoppingCartService.OnChange += StateHasChanged;
         await GetShoppingCartItems();

         CartIsLoading = false;
      }

      #region Cultureinfo

      private string GetCultureInfoPriceString(int input)
      {
         return $"{input.ToString("N0", CultureInfo.CreateSpecificCulture("ru-RU"))} ֏";
      }

      private string GetCultureInfoAmountString(int? input, string nominalName = "")
      {
         var text = string.Format(CultureInfo.CreateSpecificCulture("ru-RU"), "{0:N0}", input) + " " + nominalName;
         //Console.WriteLine(text);
         return text;
      }

      #endregion Culturinfo

      #region Quantity control

      /// <summary>
      /// Decrease
      /// </summary>
      protected async Task DecreaseQuantity(ShoppingCartItemModel cartItemModel)
      {
         if (cartItemModel.Quantity > 1 && cartItemModel.Quantity < 999)
         {
            ShopingCartItems.First(item => item.OfferId == cartItemModel.OfferId).Quantity--;
         }
         else
         {
            ShopingCartItems.First(item => item.OfferId == cartItemModel.OfferId).Quantity = 1;
         }
         StateHasChanged();

         await ShoppingCartService.EditCustomerShoppingCartItem(cartItemModel);
      }

      /// <summary>
      /// Increase
      /// </summary>
      protected async Task IncreaseQuantity(ShoppingCartItemModel cartItemModel)
      {
         if (cartItemModel.Quantity < 999 && cartItemModel.Quantity > 0)
         {
            ShopingCartItems.First(item => item.OfferId == cartItemModel.OfferId).Quantity++;

         }
         else
         {
            ShopingCartItems.First(item => item.OfferId == cartItemModel.OfferId).Quantity = 999;
         }

         StateHasChanged();

         await ShoppingCartService.EditCustomerShoppingCartItem(cartItemModel);
      }

      /// <summary>
      /// Quantity input change
      /// </summary>
      protected async Task QuantityChange(ChangeEventArgs e, ShoppingCartItemModel cartItemModel)
      {
         int quantity;
         if (int.TryParse(e.Value.ToString(), out quantity) && quantity > 0)
         {
            cartItemModel.Quantity = quantity;
            if (cartItemModel.Quantity > 0 && cartItemModel.Quantity < 999)
            {
            }
            else
            {
               ShopingCartItems.First(item => item.OfferId == cartItemModel.OfferId).Quantity = 1;
            }
         }
         else
         {
            ShopingCartItems.First(item => item.OfferId == cartItemModel.OfferId).Quantity = 1;
         }

         StateHasChanged();

         //await ShoppingCartService.EditItemLocalStorageAsync(ShopingCartItems.First(item => item.OfferId == cartItemModel.OfferId));

         await ShoppingCartService.EditCustomerShoppingCartItem(cartItemModel);
      }

      #endregion QuantityControl

      protected int GetShoppingCartItemTotalPrice(ShoppingCartItemModel cartItemModel)
      {
         return cartItemModel.Quantity * cartItemModel.Price;
      }

      public void Dispose()
      {
         ShoppingCartService.OnChange -= StateHasChanged;
      }

      #endregion Methods

   }
}