using Grpc.Core;
using Microsoft.AspNetCore.Components;
using NverCard.Public.Client.ServiceProxies.Commands.CustomerOrders;
using NverCard.Public.Client.ServiceProxies.Commands.CustomerOrders.Models;
using NverCard.Public.Client.ServiceProxies.Dictionaries;
using NverCard.Public.Client.ServiceProxies.Queries.IndividualCustomers;
using NverCard.Public.Client.ServiceProxies.Queries.IndividualCustomers.Models;
using NverCard.Public.Client.ServiceProxies.Queries.ShoppingCarts.Models;
using NverCard.Public.Client.Services.ShoppingCartService;
using NverCard.Public.Client.Services.ToastService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NverCard.Public.Client.Pages.Checkout
{
    public partial class Checkout
    {

        #region Constant

        private static string EnvelopeOne = "envelope_type_one";
        private static string EnvelopeTwo = "envelope_type_two";
        private static string EnvelopeThree = "envelope_type_three";
        private static string EnvelopeFour = "envelope_type_four";
        public static int DeliveryPrice = 600;
        public static int EnvelopePrice = 1000;

        #endregion Constant

        #region Inject

        [Inject]
        GetCurrentIndividualCustomerQueryProxy GetCurrentIndividualCustomerQueryProxy { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        UserSession UserSession { get; set; }

        [Inject]
        private CreateCustomerOrderCommandProxy CreateCustomerOrderCommandProxy { get; set; }

        [Inject]
        private IToastService IToastService { get; set; }

        #endregion Inject

        #region Reference 

        /// <summary>
        /// ссылка на модал
        /// </summary>
        private NverCard.Public.Client.Pages.Thanks.Thanks thanks;

        #endregion Reference

        #region Fields



        #endregion Fields

        #region Properties

        /// <summary>
        /// confirm order
        /// </summary>
        private bool ConfirmOrder { get; set; }

        /// <summary>
        /// Order create model
        /// </summary>
        private CreateCustomerOrderModel OrderModel { get; set; }

        /// <summary>
        /// send as gift user name
        /// </summary>
        private string GiftReceivingName { get; set; }

        /// <summary>
        /// send as gift description
        /// </summary>
        private string Congratulation { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        private string Email { get; set; }

        /// <summary>
        /// description
        /// </summary>
        private string Description { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        private string Address { get; set; }

        /// <summary>
        /// Phone number for shipment
        /// </summary>
        private string PhoneNumber { get; set; }

        /// <summary>
        /// Current user
        /// </summary>
        private IndividualCustomerModel CurrentUser { get; set; } = new IndividualCustomerModel();


        /// <summary>
        /// Selected payment type
        /// </summary>
        private PaymentType SelectedPaymentType { get; set; } = PaymentType.Card;

        /// <summary>
        /// Selected envelope
        /// </summary>
        public string SelectedEnvelope { get; set; } = EnvelopeOne;

        /// <summary>
        /// with delivery
        /// </summary>
        private bool WithDelivery { get; set; }

        /// <summary>
        /// Send as a gift
        /// </summary>
        private bool SendAsGift { get; set; }

        /// <summary>
        /// shopping cart items
        /// </summary>
        private List<ShoppingCartItemModel> ShopingCartItems { get; set; }

        private DateTime CurrentDateTime { get; set; } = DateTime.Now;


        private DateTime? SelectedDateTime { get; set; } = null;

        /// <summary>
        /// order create button loading
        /// </summary>
        private bool ButtonLoading { get; set; }

        #endregion Properties

        #region Methods
        private async Task CreateOrder()
        {
            try
            {
                ButtonLoading = true;
                await CreateCustomerOrderCommandProxy.Execute(new CreateCustomerOrderModel
                {
                    AdditionalInformation = Description,
                    Congratulation = Congratulation,
                    CustomerId = CurrentUser.Id,
                    Deliverable = WithDelivery,
                    Email = Email,
                    FullDeliveryAddress = Address,
                    OrderRequestDateTime = SelectedDateTime ?? DateTime.Now,
                    PhoneNumber = PhoneNumber,
                    PaymentType = SelectedPaymentType,
                    SendAsGift = SendAsGift,
                    GiftReceivingName = GiftReceivingName,
                    SelectedEnvelope = SelectedEnvelope,
                    DeliveryDateTime = SelectedDateTime,

                    OrderItems = ShopingCartItems.Select(item => new CreateCustomerOrderItemModel
                    {
                        OfferId = item.OfferId,
                        Quantity = item.Quantity
                    }).ToList()
                });

                ButtonLoading = false;
                ConfirmOrder = true;
                thanks.OpenThanks();
                StateHasChanged();
            }
            catch (RpcException exception)
            {
                await IToastService.Error(exception);
                ButtonLoading = false;
                StateHasChanged();
                throw;
            }

        }

        private void SelectEnvelope(string name)
        {
            SelectedEnvelope = name;
        }

        /// <summary>
        /// Return cart items
        /// </summary>
        public async Task<List<ShoppingCartItemModel>> GetShoppingCartItems()
        {
            if (UserSession.IsSessionStarted)
                ShopingCartItems = (await ShoppingCartService.GetCustomerShoppingCartAsync()).CartItems.ToList();
            else
                ShopingCartItems = new List<ShoppingCartItemModel>();

            StateHasChanged();

            return ShopingCartItems;

        }

        private string GetCultureInfoPriceString(int input)
        {
            return $"{input.ToString("N0", CultureInfo.CreateSpecificCulture("en-us"))} ֏";
        }

        private string GetCultureInfoAmountString(int? input, string nominalName = "")
        {
            var text = string.Format(CultureInfo.CreateSpecificCulture("en-us"), "{0:0}", input) + " " + nominalName;
            //Console.WriteLine(text);
            return text;
        }

        protected override async Task OnInitializedAsync()
        {
            await UserSession.LoadSession();
            try
            {
                if (UserSession.IsSessionStarted)
                {
                    CurrentUser = await GetCurrentIndividualCustomerQueryProxy.Execute();
                    PhoneNumber = CurrentUser.Name;
                }
                else
                {
                    CurrentUser = new IndividualCustomerModel();
                }
            }
            catch (Exception)
            {
                //await SignOut();
                throw;
            }
            await GetShoppingCartItems();

        }

        /// <summary>
        /// Get delivery type
        /// </summary>
        private string GetDeliveryType(bool isWithDelivery)
        {
            return isWithDelivery ? WithDelivery ? "checked" : null : WithDelivery ? null : "checked";
        }

        /// <summary>
        /// Return cart total price
        /// </summary>
        protected int GetCartTotalPrice()
        {
            return (ShopingCartItems != null && ShopingCartItems.Count > 0) ?
                ShopingCartItems.Sum(item => item.Price * item.Quantity) +
                (WithDelivery ? DeliveryPrice : 0) +
                (SelectedEnvelope != EnvelopeOne && SelectedEnvelope != null ? EnvelopePrice : 0) : 0;
        }

        /// <summary>
        /// Check payment tyoe
        /// </summary>
        private string CheckPaymentType(PaymentType paymentType)
        {
            return SelectedPaymentType == paymentType ? "checked" : null;
        }

        /// <summary>
        /// change payment type
        /// </summary>
        private void ChangePaymentType(PaymentType paymentType)
        {
            SelectedPaymentType = paymentType;
            StateHasChanged();
        }

        /// <summary>
        /// Get continue button status
        /// </summary>
        private bool ContinueButtonDisabled()
        {
            if (WithDelivery)
            {

            }
            else
            {

            }

            return ButtonLoading;
        }

        private void ChangeDeliveryType(bool isWithDelivery)
        {
            WithDelivery = isWithDelivery;
            if (!isWithDelivery)
            {
                SelectedPaymentType = PaymentType.Card;
                SelectedDateTime = null;
                SelectedEnvelope = null;
            }
            else
            {
                SelectedEnvelope = EnvelopeOne;
            }

            StateHasChanged();
        }

        private void SendAsChange()
        {
            SendAsGift = !SendAsGift;
        }

        private async Task ChangeDeliveryDate(ChangeEventArgs e)
        {
            DateTime selectedDate;

            if (DateTime.TryParse(e.Value.ToString(), out selectedDate) && selectedDate > CurrentDateTime)
            {
                SelectedDateTime = selectedDate;
            }
            else
            {
                SelectedDateTime = null;
            }

            Console.WriteLine(SelectedDateTime);
            StateHasChanged();
        }

        private async Task SelectedDateChanged(DateTime selectedDate)
        {
            SelectedDateTime = selectedDate;
            StateHasChanged();
        }

        /// <summary>
        /// Navigate to
        /// </summary>
        private async Task NavigateTo(string url)
        {
            NavigationManager.NavigateTo(url);
        }
        #endregion Methods


    }
}
