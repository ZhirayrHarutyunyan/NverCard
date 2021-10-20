using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NverCard.Public.Client.Modal.ToastModal;
using NverCard.Public.Client.ServiceProxies.Queries.Products.Models;
using NverCard.Public.Client.Services.FavoritService;
using NverCard.Public.Client.Services.ProductService;
using NverCard.Public.Client.Services.ShoppingCartService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NverCard.Public.Client.Pages.Products
{
    public partial class Products
    {
        #region Const

        private int PageConstSize = 8;

        #endregion Const

        #region Inject

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        IFavoritService FavoritService { get; set; }

        [Inject]
        IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        IProductService ProductService { get; set; }

        [Inject]
        UserSession UserSession { get; set; }


        #endregion Inject

        #region Properties

        /// <summary>
        /// Ընթացիք կատեգորիա կամ պիտակ
        /// </summary>
        private string CurrentSection { get; set; }

        /// <summary>
        /// Min value for slider
        /// </summary>
        private string SliderMinValue { get; set; }

        /// <summary>
        /// Max value for slider
        /// </summary>
        private string SliderMaxValue { get; set; }

        /// <summary>
        /// Slider current min value
        /// </summary>
        private int? CurrentSliderMinValue { get; set; }

        /// <summary>
        /// Slider current max value
        /// </summary>
        private int? CurrentSliderMaxValue { get; set; }

        /// <summary>
        /// Category Id
        /// </summary>
        [Parameter]
        public string SeoName { get; set; }

        /// <summary>
        /// Products on page
        /// </summary>
        private List<ProductShortInfoModel> ProductsList { get; set; } = new List<ProductShortInfoModel>();

        /// <summary>
        /// Favorits
        /// </summary>
        [Parameter]
        public List<Guid> CustomerFavorits { get; set; } = new List<Guid>();

        /// <summary>
        /// Disable favorit button
        /// </summary>
        private bool FavoritButtonIsDisabled { get; set; }

        #region Paging

        /// <summary>
        /// Skip
        /// </summary>
        private int Skip { get; set; }

        private int AllCount { get; set; }

        private int PagesCount { get; set; }

        private int PageSize { get; set; }

        private int CurrentPage { get; set; }

        private List<int> Pages { get; set; } = new List<int>();

        #endregion Paging



        #endregion Properties

        #region Methods

        private async Task GetProducts()
        {
            //await UserSession.LoadSession();
            ProductsList = new List<ProductShortInfoModel>();

            var productInfo = await ProductService.GetProductsPageShortInfoBySeoName(
                new GetProductsPagingModel
                {
                    SeoLink = SeoName,
                    Skip = Skip,
                    Take = PageSize,
                    MinPrice = CurrentSliderMinValue,
                    MaxPrice = CurrentSliderMaxValue
                });

            ProductsList = productInfo.Products.ToList();
            AllCount = productInfo.AllCount;

            if (ProductsList.Count() > 0 && !CurrentSliderMinValue.HasValue && !CurrentSliderMaxValue.HasValue)
            {
                SliderMinValue = productInfo.MinPriceValue.ToString();
                SliderMaxValue = productInfo.MaxPriceValue.ToString();

                CurrentSliderMinValue = productInfo.MinPriceValue;
                CurrentSliderMaxValue = productInfo.MaxPriceValue;
            }

            //pages count
            Pages = new List<int>();
            PagesCount = int.Parse((Math.Ceiling((double)AllCount / PageConstSize).ToString()));

            for (int i = 1; i <= PagesCount; i++)
            {
                Pages.Add(i);
            }
        }

        #region Favorit control

        private async Task FavoritsRebind()
        {
            CustomerFavorits = await FavoritService.GetCustomerFavorits();
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

            FavoritButtonIsDisabled = false;
        }

        #endregion Favorit control

        #region Slider control

        /// <summary>
        /// Slider Max value
        /// </summary>
        private async Task OnSliderMaxValueChange(ChangeEventArgs e)
        {
            CurrentSliderMaxValue = int.Parse(e.Value.ToString());
            await GetProducts();
        }

        /// <summary>
        /// Slider on min value
        /// </summary>
        private async Task OnSliderMinValueChange(ChangeEventArgs e)
        {
            CurrentSliderMinValue = int.Parse(e.Value.ToString());
            await GetProducts();
        }

        #endregion Slider control

        #region Text culture control

        private string GetCultureInfoPriceString(int input)
        {
            return $"{input.ToString("N0", CultureInfo.CreateSpecificCulture("ru-RU"))} ֏";
        }

        #endregion Text culture control

        #region Paging

        private async Task RedirectPage(int page)
        {
            PageSize = PageConstSize;

            if (page >= 1 && page <= AllCount)
            {
                CurrentPage = page;
                Skip = (CurrentPage - 1) * PageSize;

                await GetProducts();
            }
            PageSize = PageConstSize;
        }

        private async Task ShowMore()
        {
            if (CurrentPage + 1 <= PagesCount)
            {
                CurrentPage += 1;
                PageSize += PageConstSize;

                await GetProducts();
            }
        }

        #endregion Paging

        protected override async Task OnParametersSetAsync()
        {
            ProductsList = new List<ProductShortInfoModel>();
            await GetProducts();

            CurrentSection = ProductsList.FirstOrDefault().ProductTags.FirstOrDefault(t => t.SeoLink == SeoName)?.Name ?? ProductsList.FirstOrDefault().Categories.FirstOrDefault(t => t.SeoLink == SeoName)?.CategoryName;

            #region Get favorits

            await FavoritsRebind();

            #endregion Get favorits
        }

        protected override async Task OnInitializedAsync()
        {
            PageSize = PageConstSize;
            Skip = 0;
            CurrentPage = 1;
            SliderMinValue = "";
            SliderMaxValue = "";
            FavoritService.OnChange += StateHasChanged;
            ShoppingCartService.OnChange += StateHasChanged;

            await FavoritService.GetCustomerFavoritsCount();
            await ShoppingCartService.GetShoppingCartItemsCount();
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("rangeFilter.init");
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            FavoritService.OnChange -= StateHasChanged;
            ShoppingCartService.OnChange -= StateHasChanged;
        }

        #endregion Methods
    }
}
