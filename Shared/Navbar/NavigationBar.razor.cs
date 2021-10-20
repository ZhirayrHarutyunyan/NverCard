using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NverCard.Public.Client.Modal.LoginModal;
using NverCard.Public.Client.ServiceProxies.Queries.IndividualCustomers;
using NverCard.Public.Client.ServiceProxies.Queries.IndividualCustomers.Models;
using NverCard.Public.Client.ServiceProxies.Queries.ProductCategories.Models;
using NverCard.Public.Client.ServiceProxies.Queries.ProductTags.Models;
using NverCard.Public.Client.ServiceProxies.Queries.ShoppingCarts.Models;
using NverCard.Public.Client.Services.FavoritService;
using NverCard.Public.Client.Services.ProductCategoryService;
using NverCard.Public.Client.Services.ProductService;
using NverCard.Public.Client.Services.ProductTagService;
using NverCard.Public.Client.Services.ShoppingCartService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NverCard.Public.Client.Shared.Navbar
{
    public partial class NavigationBar
    {
        #region Inject

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        private UserSession UserSession { get; set; }

        [Inject]
        GetCurrentIndividualCustomerQueryProxy GetCurrentIndividualCustomerQueryProxy { get; set; }

        [Inject]
        IFavoritService FavoritService { get; set; }

        [Inject]
        IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        IProductCategoryService ProductCategoryService { get; set; }

        [Inject]
        IProductTagService ProductTagService { get; set; }

        [Inject]
        IProductService ProductService { get; set; }

        #endregion Inject

        #region Reference

        /// <summary>
        /// ссылка на модал
        /// </summary>
        private Login modalLogin;

        #endregion Reference

        #region Properties

        private string ShoppingCartAmount { get; set; }

        private int ShoppingCartItemsCount { get; set; }

        /// <summary>
        /// Shopping cart
        /// </summary>
        private ShoppingCartModel ShoppingCart { get; set; }

        /// <summary>
        /// Current user
        /// </summary>
        private IndividualCustomerModel CurrentUser { get; set; } = new IndividualCustomerModel();

        /// <summary>
        /// Կատեգորիաներ
        /// </summary>
        public List<ProductCategoryModel> ProductCategoriesList { get; set; } = new List<ProductCategoryModel>();

        /// <summary>
        /// Tags
        /// </summary>
        public List<ProductTagModel> ProductTags { get; set; } = new List<ProductTagModel>();

        /// <summary>
        /// Favorite count
        /// </summary>
        [Parameter]
        public int CustomerFavoriteCount { get; set; }

        #endregion Properties

        #region Methods

        public bool ShowBasketDropDown()
        {
            return !NavigationManager.Uri.Contains("basket") && !NavigationManager.Uri.Contains("checkout");
        }

        protected override async Task OnInitializedAsync()
        {

            await UserInfoRebind();

            try
            {
                //await UserInfoRebind();
                ProductCategoriesList = await ProductCategoryService.GetProductCategories();
                ProductTags = await ProductTagService.GetProductTags();
                ShoppingCart = await ShoppingCartService.GetCustomerShoppingCartAsync();
                ShoppingCartAmount = GetCultureInfoAmountString((await ShoppingCartService.GetShoppingCartAmount()), "֏");
                ShoppingCartItemsCount = await ShoppingCartService.GetShoppingCartItemsCount();
                CustomerFavoriteCount = await FavoritService.GetCustomerFavoritsCount();
            }
            catch (RpcException exception)
            {
                //await SignOut();
                throw;
            }

            FavoritService.OnChange += (async () =>
            {
                CustomerFavoriteCount = await FavoritService.GetCustomerFavoritsCount();
                StateHasChanged();
            });

            UserSession.OnChange += StateHasChanged;

            ShoppingCartService.OnChange += (async () =>
            {
                ShoppingCart = await ShoppingCartService.GetCustomerShoppingCartAsync();
                ShoppingCartAmount = GetCultureInfoAmountString((await ShoppingCartService.GetShoppingCartAmount()), "֏");
                ShoppingCartItemsCount = await ShoppingCartService.GetShoppingCartItemsCount();
                StateHasChanged();
            });

            ProductService.OnProductChange += StateHasChanged;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                JSRuntime.InvokeVoidAsync("navbarFixed.init");
            }
        }

        public async Task UserInfoRebind()
        {
            try
            {
                await UserSession.LoadSession();
                if (UserSession.IsSessionStarted)
                {
                    CurrentUser = await GetCurrentIndividualCustomerQueryProxy.Execute();
                }
                else
                {
                    CurrentUser = new IndividualCustomerModel();
                    if (UserSession.AnonymousUserId == null || UserSession.AnonymousUserId == Guid.Empty)
                    {
                        await UserSession.SetUserId(Guid.NewGuid());
                    }
                }
            }
            catch (Exception)
            {
                await SignOut();
            }

            ShoppingCart = await ShoppingCartService.GetCustomerShoppingCartAsync();
            ShoppingCartAmount = GetCultureInfoAmountString((await ShoppingCartService.GetShoppingCartAmount()), "֏");
            ShoppingCartItemsCount = await ShoppingCartService.GetShoppingCartItemsCount();
            CustomerFavoriteCount = await FavoritService.GetCustomerFavoritsCount();
            StateHasChanged();
        }

        private async Task RemoveItemFromShoppingCart(ShoppingCartItemModel model)
        {
            await ShoppingCartService.RemoveItemFromCustomerShoppingCartAsync(model.Id);
        }

        private string GetCultureInfoAmountString(int? input, string nominalName = "")
        {
            var text = string.Format(CultureInfo.CreateSpecificCulture("ru-RU"), "{0:N0}", input) + " " + nominalName;
            return text;
        }

        private async Task SignOut()
        {
            await UserSession.FinishSession();
            await UserInfoRebind();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            FavoritService.OnChange -= StateHasChanged;
            ShoppingCartService.OnChange -= StateHasChanged;
            ProductService.OnProductChange -= StateHasChanged;
            UserSession.OnChange -= StateHasChanged;
        }


        #endregion Methods
    }
}
