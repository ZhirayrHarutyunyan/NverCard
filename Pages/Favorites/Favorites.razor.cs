using Microsoft.AspNetCore.Components;
using NverCard.Public.Client.ServiceProxies.Queries.Favorits;
using NverCard.Public.Client.ServiceProxies.Queries.Products;
using NverCard.Public.Client.ServiceProxies.Queries.Products.Models;
using NverCard.Public.Client.Services.FavoritService;
using NverCard.Public.Client.Services.ProductService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NverCard.Public.Client.Pages.Favorites
{
    public partial class Favorites
    {
        #region Inject

        [Inject]
        IFavoritService FavoritService { get; set; }

        [Inject]
        IProductService ProductService { get; set; }

        [Inject]
        UserSession UserSession { get; set; }

        [Inject]
        GetCustomerFavoritProductsShortInfoQueryProxy GetCustomerFavoritProductsShortInfoQueryProxy { get; set; }

        [Inject]
        GetAvailableProductsShortInfoByIdsQueryProxy GetAvailableProductsShortInfoByIdsQueryProxy { get; set; }

        #endregion Inject

        #region Properties

        /// <summary>
        /// Selected category
        /// </summary>
        public List<Guid> CustomerFavoritsView { get; set; } = new List<Guid>();

        /// <summary>
        /// Selected category
        /// </summary>
        public List<Guid> CustomerFavorits { get; set; } = new List<Guid>();

        private List<ProductShortInfoModel> FavoritProducts { get; set; }

        private bool PageIsLoadnig { get; set; }

        #endregion Properties

        #region Methods

        private async Task Rebind()
        {
            //await UserSession.LoadSession();

            CustomerFavorits = await FavoritService.GetCustomerFavorits();
            if (CustomerFavorits.Count() > 0)
                FavoritProducts = await ProductService.GetProductsShortInfoByIds(CustomerFavorits);
            else
                FavoritProducts = new List<ProductShortInfoModel>();

            StateHasChanged();
        }


        private string GetCultureInfoAmountString(double input)
        {
            return $"{input.ToString("N0", CultureInfo.CreateSpecificCulture("en-us"))} ֏";
        }

        protected override async Task OnInitializedAsync()
        {
            PageIsLoadnig = true;
            await Rebind();

            PageIsLoadnig = false;
        }

        private string CheckFavoritStatus(ProductShortInfoModel product)
        {
            if (CustomerFavorits != null && CustomerFavorits.Any())
            {
                return CustomerFavorits.Contains(product.Id) ? "active" : "";
            }
            else
                return "";
        }

        private async Task CreateOrRemoveFavorite(ProductShortInfoModel product)
        {
            if (CustomerFavorits != null && CustomerFavorits.Contains(product.Id))
            {
                await FavoritService.RemoveFromFavorit(product.Id);
            }
            else
            {
                await FavoritService.AddToFavorit(product.Id);
            }

            CustomerFavorits = await FavoritService.GetCustomerFavorits();
            await Rebind();
        }
        #endregion Methods
    }
}
