using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NverCard.Public.Client.ServiceProxies.Dictionaries;
using NverCard.Public.Client.ServiceProxies.Queries.Products;
using NverCard.Public.Client.ServiceProxies.Queries.Products.Models;
using NverCard.Public.Client.ServiceProxies.Queries.ShoppingCarts.Models;
using NverCard.Public.Client.Services.FavoritService;
using NverCard.Public.Client.Services.ShoppingCartService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace NverCard.Public.Client.Pages.Product
{
    public partial class Product
    {
        #region Inject

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        GetProductByProductSeoLinkQueryProxy GetProductByProductSeoLinkQueryProxy { get; set; }

        [Inject]
        GetProductsShortInfoByCategorySeoQueryProxy GetProductsShortInfoByCategorySeoQueryProxy { get; set; }

        [Inject]
        GetProductsShortInfoByProductTagSeoQueryProxy GetProductsShortInfoByProductTagSeoQueryProxy { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        IFavoritService FavoritService { get; set; }

        [Inject]
        UserSession UserSession { get; set; }



        #endregion Ijnect

        #region Properties

        /// <summary>
        /// Favorits
        /// </summary>
        [Parameter]
        public List<Guid> CustomerFavorits { get; set; } = new List<Guid>();


        [Parameter]
        public string ProductSeoLink { get; set; }

        /// <summary>
        /// Current product
        /// </summary>
        private ProductModel CurrentProduct { get; set; }

        /// <summary>
        /// Current product
        /// </summary>
        private List<ProductShortInfoModel> InterestedProductsView { get; set; } = new List<ProductShortInfoModel>();

        /// <summary>
        /// Պռոդուկտի անվանում
        /// </summary>
        private string ProductName { get; set; }

        /// <summary>
        /// Նկարագիր
        /// </summary>
        private static string Description { get; set; }

        /// <summary>
        /// Նկարագիր
        /// </summary>
        private static string DescriptionOfTermsOfUse { get; set; }

        /// <summary>
        /// Որտեղ օգտագործել
        /// </summary>
        private static string ShortDescription { get; set; }

        /// <summary>
        /// Մինիամալ վավերականության ժամկետ
        /// </summary>
        private static string GuaranteedValidityPeriod { get; set; }

        /// <summary>
        /// Արժեք
        /// </summary>
        private string SelectedPriceAmount { get; set; }

        /// <summary>
        /// Նկար
        /// </summary>
        private string ImageFileId { get; set; }

        /// <summary>
        /// Image url
        /// </summary>
        private string CurrentProductImageUrl { get; set; }

        /// <summary>
        /// gift card type
        /// </summary>
        private GiftCardType CardType { get; set; }

        /// <summary>
        /// selected gift card type
        /// </summary>
        private GiftCardType SelectedGiftCardType { get; set; } = GiftCardType.Electronic;

        /// <summary>
        /// Category name
        /// </summary>
        private List<CategorySeoLinkModel> CategoriesSeoLinks { get; set; } = new List<CategorySeoLinkModel>();

        /// <summary>
        /// Նոմինալներ
        /// </summary>
        private List<OfferModel> OffersNominals { get; set; } = new List<OfferModel>();

        /// <summary>
        /// Selected offer
        /// </summary>
        private OfferModel SelectedOffer { get; set; }

        private int Quantity { get; set; } = 1;

        #endregion Properties

        #region Methods

        protected async Task SetProperties(ProductModel product)
        {
            try
            {
                if (product != null)
                {
                    ProductName = CurrentProduct.Name;
                    Description = CurrentProduct.Description;
                    DescriptionOfTermsOfUse = CurrentProduct.DescriptionOfTermsOfUse;
                    ShortDescription = CurrentProduct.ShortDescription;
                    CurrentProductImageUrl = CurrentProduct.ProductImage.ImageUrl;
                    ImageFileId = CurrentProduct.ProductImage.FileId.ToString();
                    GuaranteedValidityPeriod = CurrentProduct.GuaranteedValidityPeriod.Value.ToString();
                    SelectedGiftCardType = CurrentProduct.Offers.Any(o => o.GiftCardType == GiftCardType.Electronic) ? GiftCardType.Electronic : GiftCardType.Plastic;
                    OffersNominals = CurrentProduct.Offers.Where(o => o.GiftCardType == SelectedGiftCardType).Select(o => new OfferModel
                    {
                        Nominal = o.Nominal,
                        Price = o.Price,
                        Id = o.Id,
                        NominalName = o.NominalName,
                        GiftCardType = o.GiftCardType
                    }).OrderBy(n => n.Nominal).ToList();
                    CategoriesSeoLinks = CurrentProduct.CategoriesSeoLinks.Select(c => new CategorySeoLinkModel { CategoryName = c.CategoryName, SeoLink = c.SeoLink, CategoryId = c.CategoryId }).ToList();
                    SelectedOffer = OffersNominals.FirstOrDefault();
                    SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity));

                    foreach (string seoLink in CurrentProduct.CategoriesSeoLinks.Select(l => l.SeoLink))
                    {
                        InterestedProductsView.AddRange((await GetProductsShortInfoByCategorySeoQueryProxy.Execute(seoLink)));
                    }

                    InterestedProductsView = InterestedProductsView.Where(c => c.Id != CurrentProduct.Id).ToList();
                    CustomerFavorits = await FavoritService.GetCustomerFavorits();

                }
                else
                {
                    CurrentProduct = null;
                    ProductName = null;
                    Description = null;
                    DescriptionOfTermsOfUse = null;
                    ShortDescription = null;
                    ImageFileId = null;
                    GuaranteedValidityPeriod = null;
                    OffersNominals = new List<OfferModel>();
                    CategoriesSeoLinks = new List<CategorySeoLinkModel>();
                    SelectedOffer = null;
                    SelectedPriceAmount = null;
                    InterestedProductsView = new List<ProductShortInfoModel>();

                    if (string.IsNullOrEmpty(ProductSeoLink))
                        NavigationManager.NavigateTo("/");
                    else
                    {
                        CurrentProduct = await GetProductByProductSeoLinkQueryProxy.Execute(ProductSeoLink);

                        if (CurrentProduct == null)
                            NavigationManager.NavigateTo("/products");
                    }
                    CustomerFavorits = null;
                }
            }
            catch (Exception)
            {
                NavigationManager.NavigateTo("/products");
            }


        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                await SetProperties(null);
                await SetProperties(CurrentProduct);
            }
            catch (RpcException exception)
            {
                throw;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            JSRuntime.InvokeVoidAsync("slickSliderConfigs.init");
        }

        protected void SelectNominal(int selectedNominal)
        {
            SelectedOffer = OffersNominals.FirstOrDefault(o => o.Nominal == selectedNominal);
            SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity));
        }

        #region Quantity control

        protected void IncreaseQuantity()
        {
            if (Quantity < 999 && Quantity > 0)
            {
                Quantity++;
                SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity));
            }
            else
            {
                Quantity = 999;
                SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity));
            }
        }

        protected void DecreaseQuantity()
        {
            if (Quantity > 1 && Quantity < 999)
            {
                Quantity--;
                SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity));
            }
            else
            {
                Quantity = 1;
                SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity));
            }
        }

        protected void QuantityChange(ChangeEventArgs e)
        {
            int quantity;
            if (int.TryParse(e.Value.ToString(), out quantity) && quantity > 0)
            {
                Quantity = quantity;
                if (Quantity > 0 && Quantity < 999)
                {
                    SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity));
                }
                else
                {
                    Quantity = 1;
                    SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity));
                }
            }
            else
            {
                Quantity = 1;
                SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity), SelectedOffer.NominalName);
            }
        }

        #endregion Quantity control

        #region CultureInfo

        private string GetCultureInfoAmountString(int? input, string nominalName = "")
        {
            var text = string.Format(CultureInfo.CreateSpecificCulture("ru-RU"), "{0:N0}", input) + " " + nominalName;
            //Console.WriteLine(text);
            return text;
        }

        private string GetCultureInfoPriceString(int input)
        {
            return $"{input.ToString("N0", CultureInfo.CreateSpecificCulture("ru-RU"))} ֏";
        }

        #endregion CultureInfo

        #region ShoppingCart control

        private async Task AddItemToShoppingCart()
        {
            await ShoppingCartService.AddItemToCustomerShoppingCartAsync(new ShoppingCartItemModel()
            {
                OfferId = SelectedOffer.Id,
                Quantity = Quantity
            });
        }

        #endregion ShoppingCart control

        #region Favorit control

        private async Task CreateOrRemoveFavorite()
        {

            if (CustomerFavorits != null && CustomerFavorits.Contains(CurrentProduct.Id))
            {
                await FavoritService.RemoveFromFavorit(CurrentProduct.Id);
            }
            else
            {
                await FavoritService.AddToFavorit(CurrentProduct.Id);
            }

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

        private string CheckFavoritStatus()
        {
            if (CustomerFavorits != null && CustomerFavorits.Any())
            {
                return CustomerFavorits.Contains(CurrentProduct.Id) ? "active" : "";
            }
            else
                return "";
        }

        /// <summary>
        /// Create or remove favorit
        /// </summary>
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
        }

        #endregion Favorit control

        /// <summary>
        /// Get delivery type
        /// </summary>
        private string GetCardType(GiftCardType cardType)
        {
            return SelectedGiftCardType == cardType ? "checked" : null;
        }

        /// <summary>
        /// change card type
        /// </summary>
        private void ChangeCardType(GiftCardType cardType)
        {
            SelectedGiftCardType = cardType;
            OffersNominals = CurrentProduct.Offers.Where(o => o.GiftCardType == SelectedGiftCardType).Select(o => new OfferModel
            {
                Nominal = o.Nominal,
                Price = o.Price,
                Id = o.Id,
                NominalName = o.NominalName,
                GiftCardType = o.GiftCardType
            }).OrderBy(n => n.Nominal).ToList();

            SelectedOffer = OffersNominals.FirstOrDefault();
            SelectedPriceAmount = GetCultureInfoAmountString((SelectedOffer.Price * Quantity));

            StateHasChanged();
        }

        #endregion Methods
    }
}
