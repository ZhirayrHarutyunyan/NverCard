using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Nc.Applications.Exceptions;
using NverCard.Public.Client.ServiceProxies.Commands.Authenticators;
using NverCard.Public.Client.ServiceProxies.Commands.Authenticators.Models;
using NverCard.Public.Client.ServiceProxies.Commands.IndividualCustomers;
using NverCard.Public.Client.ServiceProxies.Commands.IndividualCustomers.Models;
using NverCard.Public.Client.Services.FavoritService;
using NverCard.Public.Client.Services.ShoppingCartService;
using NverCard.Public.Client.Shared.Navbar;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace NverCard.Public.Client.Modal.LoginModal
{
    public partial class Login
    {
        #region Inject

        [Inject]
        private CreateIndividualCustomerCommandProxy CreateIndividualCustomerCommandProxy { get; set; }

        [Inject]
        private UserSession UserSession { get; set; }

        [Inject]

        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private CheckValidConfirmTokenCommandProxy CheckValidConfirmTokenCommandProxy { get; set; }

        [Inject]
        private CreateAuthenticatorCommandProxy CreateAuthenticatorCommandProxy { get; set; }

        [Inject]
        IFavoritService FavoritService { get; set; }

        [Inject]
        IShoppingCartService ShoppingCartService { get; set; }

        #endregion Inject

        #region Properties

        [Parameter]
        public EventCallback LoadData { get; set; }


        /// <summary>
        /// phone number error text
        /// </summary>
        private string PhoneNumberErrorText { get; set; }

        /// <summary>
        /// phone number is valid
        /// </summary>
        private bool PhoneNumberIsValid { get; set; }

        /// <summary>
        /// Customer phone number
        /// </summary>
        private string PhoneNumber { get; set; }

        /// <summary>
        /// Inputed confirm code
        /// </summary>
        private string ConfirmCode { get; set; }

        /// <summary>
        /// Valid confirm code
        /// </summary>
        private bool ConfirmCodeIsValid { get; set; }

        /// <summary>
        /// Confirm mode
        /// </summary>
        private bool IsConfirmMode { get; set; } = false;

        /// <summary>
        /// Rules accepted 
        /// </summary>
        private bool IsRulesAccepted { get; set; } = false;

        /// <summary>
        /// Loader for button
        /// </summary>
        private bool ButtonLoading { get; set; }

        /// <summary>
        /// Confirm code timer
        /// </summary>
        private DateTime ConfirmTimer { get; set; }

        /// <summary>
        /// Current date time
        /// </summary>
        private DateTime CurrentDateTime { get; set; }

        /// <summary>
        /// Expired confirm timer
        /// </summary>
        private bool ConfirmTimerExpired { get; set; }

        /// <summary>
        /// Confirm code error message
        /// </summary>
        private string ConfirmCodeErrorMessage { get; set; }
        private bool displayModalLogin = false;

        private bool ConfirmButtonIsDisable => ButtonLoading
            || (!IsRulesAccepted
            || IsConfirmMode ? string.IsNullOrWhiteSpace(ConfirmCode) : string.IsNullOrWhiteSpace(PhoneNumber)
            || string.IsNullOrEmpty(PhoneNumber)
            || PhoneNumber.Length != 13);

        /// <summary>
        /// Օգտատեր
        /// </summary>
        private AuthenticateIndividualCustomerResultModel CustomerInfo { get; set; }

        #endregion Properties

        #region Filed
        private static int confirmCodlength = 6;

        private static Timer aTimer;

        private int counter = 60;

        private NavigationBar navbar;

        #endregion Filed

        #region Methods


        public void StartTimer()
        {
            CurrentDateTime = DateTime.UtcNow.AddHours(4);
            CurrentDateTime = CurrentDateTime.AddSeconds(-CurrentDateTime.Second);
            ConfirmTimer = CurrentDateTime.AddMinutes(1);

            aTimer = new Timer(1000);
            aTimer.Elapsed += CountDownTimer;
            aTimer.Enabled = true;
        }

        public void CountDownTimer(Object source, ElapsedEventArgs e)
        {
            if (ConfirmTimer > CurrentDateTime)
            {
                ConfirmTimer = ConfirmTimer.AddSeconds(-1);
            }
            else
            {
                aTimer.Enabled = false;
                ConfirmTimerExpired = true;
            }
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Send phone number for create customer
        /// </summary>
        private async Task SendPhoneNumber()
        {
            try
            {
                if (PhoneNumberIsValid && IsRulesAccepted)
                {
                    string clearedPhoneNumber = GetCleared(PhoneNumber);

                    ButtonLoading = true;

                    //Create customer
                    CreateIndividualCustomerModel model = new CreateIndividualCustomerModel
                    {
                        UserName = clearedPhoneNumber
                    };
                    await CreateIndividualCustomerCommandProxy.Execute(model);

                    //create authenticator
                    CreateAuthenticatorModel authenticatorModel = new CreateAuthenticatorModel { PhoneNumber = clearedPhoneNumber };
                    await CreateAuthenticatorCommandProxy.Execute(authenticatorModel);

                    IsConfirmMode = true;
                    ButtonLoading = false;
                    ConfirmTimerExpired = false;
                    StartTimer();
                }
            }
            catch (BusinessException exception)
            {
                if (exception.ErrorCode.HasValue && exception.ErrorCode.Value == 600)
                    IsConfirmMode = false;
                ButtonLoading = false;
                ConfirmTimerExpired = false;
            }

        }

        private void InputConfirmCode(ChangeEventArgs args)
        {
            string input = (string)args.Value;
            ConfirmCode = (string)args.Value;
            if (input.Length <= 6)
            {

            }

            ConfirmCodeIsValid = (input.Length == 6 && ConfirmCode.Length == 6);
        }

        /// <summary>
        /// Check phone number is valid
        /// </summary>
        private void CheckIsValidPhoneNumber(ChangeEventArgs args)
        {
            string input = (string)args.Value;

            if ((input.Length <= 13))
            {
                PhoneNumber = (string)args.Value;
                //GetNumberMask(input);
                if (!string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    PhoneNumberIsValid = false;
                }

                if (PhoneNumber.Length == 13)
                {
                    string number = GetCleared(PhoneNumber);
                    PhoneNumberIsValid = Regex.IsMatch(number, @"^\+?(374)([\- ]?)?(\(?((91)|(93)|(94)|(95)|(96)|(97)|(98)|(99)|(33)|(41)|(43)|(44)|(79)|(55)|(66)|(50)|(77)|(49)|(22))\)?[\- ]?)[\d\- ]{6}$", RegexOptions.IgnoreCase);

                    if (!PhoneNumberIsValid)
                        PhoneNumberErrorText = "Հեռախոսահամարը սխալ է կամ գոյություն չունի";
                    else
                        PhoneNumberErrorText = "";
                }
            }
        }

        /// <summary>
        /// Confirm phone number
        /// </summary>
        private async Task ConfirmPhoneNumber()
        {
            if (PhoneNumberIsValid && IsRulesAccepted && !ConfirmTimerExpired && ConfirmCodeIsValid)
                try
                {
                    ButtonLoading = true;
                    AuthenticateIndividualCustomerRequestModel model = new AuthenticateIndividualCustomerRequestModel
                    {
                        ConfirmCode = ConfirmCode,
                        PhoneNumber = GetCleared(PhoneNumber)
                    };
                    CustomerInfo = await CheckValidConfirmTokenCommandProxy.Execute(model);
                    await UserSession.StartSession(CustomerInfo.UserName, CustomerInfo.AccessToken, CustomerInfo.Id);

                    ButtonLoading = false;
                    ConfirmCodeIsValid = UserSession.IsSessionStarted;

                    if (ConfirmCodeIsValid)
                    {

                        CloseModal();
                        await LoadData.InvokeAsync();
                    }

                }
                catch (RpcException exception)
                {
                    ConfirmCodeErrorMessage = exception.Status.Detail;
                    ButtonLoading = false;
                }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task TermOfUseChange()
        {
            IsRulesAccepted = !IsRulesAccepted;
        }

        private void GetNumberMask(string input)
        {
            //if (PhoneNumber.Length == 1 && PhoneNumber!="(")
            //    PhoneNumber = $"({PhoneNumber}";
            //if (PhoneNumber.Length == 3 || (PhoneNumber.Length == 4 && PhoneNumber.EndsWith(" ")))
            //    PhoneNumber = $"{PhoneNumber}) ";
        }

        private string GetCleared(string number)
        {
            number = number.Replace(" ", "");
            number = number.Replace("(", "");
            number = number.Replace(")", "");
            number = number.Replace("-", "");

            if (!number.Contains("374"))
                number = $"374{number}";

            return number;
        }
        public async void openModalLogin()
        {
            this.displayModalLogin = true;
            await JSRuntime.InvokeVoidAsync("addClassToBody");
            StateHasChanged();
        }

        public async void CloseModal()
        {
            this.displayModalLogin = false;
            IsConfirmMode = false;
            IsRulesAccepted = false;
            PhoneNumberIsValid = false;
            ConfirmTimerExpired = false;
            ConfirmCodeIsValid = false;
            CurrentDateTime = new DateTime();
            ConfirmTimer = new DateTime();
            PhoneNumber = "";
            ConfirmCode = "";
            ConfirmCodeErrorMessage = string.Empty;
            await JSRuntime.InvokeVoidAsync("removeClassFromBody");
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            IsConfirmMode = false;
            IsRulesAccepted = false;
            PhoneNumberIsValid = false;
            ConfirmTimerExpired = false;
            ConfirmCodeIsValid = false;
            ButtonLoading = false;
            PhoneNumber = "";
            ConfirmCode = "";
            ConfirmCodeErrorMessage = string.Empty;
            StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            JSRuntime.InvokeVoidAsync("phoneMask.init");
        }


        private async Task Enter(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
                await ConfirmPhoneNumber();
        }

        #endregion Methods
    }
}
