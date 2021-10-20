using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NverCard.Public.Client.ServiceProxies.Queries.Contents;
using NverCard.Public.Client.ServiceProxies.Queries.Contents.Models;
using NverCard.Public.Client.ServiceProxies.Queries.Products.Models;
using NverCard.Public.Client.Services.FavoritService;
using NverCard.Public.Client.Services.ProductService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NverCard.Public.Client.Pages.Home
{
   public partial class Home
   {
      #region Inject

      [Inject]
      IFavoritService FavoritService { get; set; }


      [Inject]
      IProductService ProductService { get; set; }

      [Inject]
      GetSlidersQueryProxy GetSlidersQueryProxy { get; set; }


      [Inject]
      IJSRuntime JSRuntime { get; set; }

      #endregion Inject

      #region Properties

      /// <summary>
      /// Favorits
      /// </summary>
      public List<Guid> CustomerFavorits { get; set; } = new List<Guid>();


      /// <summary>
      /// Disable favorit button
      /// </summary>
      private bool FavoritButtonIsDisabled { get; set; }


      private IEnumerable<SliderModel> Slider { get; set; }

      /// <summary>
      /// Top sales
      /// </summary>
      private IEnumerable<ProductShortInfoModel> TopSalesProducts { get; set; } = new List<ProductShortInfoModel>();

      /// <summary>
      /// New arrivals
      /// </summary>
      private IEnumerable<ProductShortInfoModel> NewArrivalsProducts { get; set; }

      /// <summary>
      /// Best sales products
      /// </summary>
      private IEnumerable<ProductShortInfoModel> BestSalesProducts { get; set; }

      #endregion Properties

      #region Methods

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

         FavoritButtonIsDisabled = false;
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
      protected override async Task OnInitializedAsync()
      {
            //await JSRuntime.InvokeVoidAsync("slickSliderConfigs.init");
            var products = await ProductService.GetProductsByTagSeoLinks(new List<string>() { "top-sales", "new-arrivals", "best-sales" });
            TopSalesProducts = products.Where(p => p.ProductTags.Any(t => t.SeoLink == "top-sales"));
            NewArrivalsProducts = products.Where(p => p.ProductTags.Any(t => t.SeoLink == "new-arrivals"));
            BestSalesProducts = products.Where(p => p.ProductTags.Any(t => t.SeoLink == "best-sales"));

            Slider = await GetSlidersQueryProxy.Execute();
        }

      private string GetCultureInfoPriceString(int input, string currencyName)
      {
         return $"{input.ToString("N0", CultureInfo.CreateSpecificCulture("en-us"))} {currencyName}";
      }

      private string GetCultureInfoAmountString(int? input, string nominalName = "")
      {
         var text = string.Format(CultureInfo.CreateSpecificCulture("en-us"), "{0:0}", input) + " " + nominalName;
         return text;
      }


      protected override async Task OnAfterRenderAsync(bool firstRender)
      {
                await JSRuntime.InvokeVoidAsync("slickSliderConfigs.init");
      }


      #endregion Methods
   }
}
