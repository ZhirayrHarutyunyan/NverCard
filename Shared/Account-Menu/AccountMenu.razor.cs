using Microsoft.AspNetCore.Components;
using NverCard.Public.Client.ServiceProxies.Queries.IndividualCustomers;
using NverCard.Public.Client.ServiceProxies.Queries.IndividualCustomers.Models;
using NverCard.Public.Client.Services.FavoritService;
using NverCard.Public.Client.Services.ShoppingCartService;
using NverCard.Public.Client.Shared.Navbar;
using System;
using System.Threading.Tasks;

namespace NverCard.Public.Client.Shared.Account_Menu
{
    public partial class AccountMenu
    {

        #region Inject

        [Inject]
        IFavoritService FavoritService { get; set; }

        [Inject]
        IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        private UserSession UserSession { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        GetCurrentIndividualCustomerQueryProxy GetCurrentIndividualCustomerQueryProxy { get; set; }

        #endregion Inject

        #region Properties
                
        public string UserName { get; set; }

        #endregion Properties

        #region Parameters

        [Parameter]
        public bool IsAccountCards { get; set; } = false;

        [Parameter]
        public bool IsAccountBalance { get; set; } = false;

        [Parameter]
        public bool IsAccountOrders { get; set; } = false;

        #endregion Parameters

        #region Methods

        private async Task SignOut()
        {
            await UserSession.FinishSession();

            NavigationManager.NavigateTo("/products");
        }

        protected override async Task OnInitializedAsync()
        {
            FavoritService.OnChange += StateHasChanged;
            ShoppingCartService.OnChange += StateHasChanged;

            await UserSession.LoadSession();
            UserName = UserSession.Name;
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
