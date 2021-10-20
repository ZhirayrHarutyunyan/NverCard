using Microsoft.AspNetCore.Components;
using NverCard.Public.Client.ServiceProxies.Queries.IndividualCustomers;
using NverCard.Public.Client.ServiceProxies.Queries.IndividualCustomers.Models;
using System.Threading.Tasks;

namespace NverCard.Public.Client.Pages.Account_balance
{
    public partial class Account_balance
    {        
        #region Inject

        [Inject]
        private UserSession UserSession { get; set; }

        [Inject]
        GetCurrentIndividualCustomerQueryProxy GetCurrentIndividualCustomerQueryProxy { get; set; }

        #endregion Inject

        #region Properties

        public ReplenishmentTypes ReplenishmentType { get; set; } = ReplenishmentTypes.Electronic;

        /// <summary>
        /// Current user
        /// </summary>
        private IndividualCustomerModel CurrentUser { get; set; } = new IndividualCustomerModel();


        #endregion Properties


        #region Methods

        private async Task SelectReplenishment(ReplenishmentTypes type)
        {
            ReplenishmentType = type;
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            CurrentUser = await GetCurrentIndividualCustomerQueryProxy.Execute();
        }

        public enum ReplenishmentTypes
        {
            /// <summary>
            /// Քարտով  
            /// </summary>
            Electronic = 1,

            /// <summary>
            /// Կանխիկ  
            /// </summary>
            Universal = 2                        
        }

        #endregion Methods
    }
}
