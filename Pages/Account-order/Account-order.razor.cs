using Microsoft.AspNetCore.Components;
using NverCard.Public.Client.Modal.LoginModal;
using NverCard.Public.Client.ServiceProxies.Queries.CustomerOrders.Models;
using NverCard.Public.Client.Services.OrderService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NverCard.Public.Client.Pages.Account_order
{
   public partial class Account_order
   {
      #region Inject
      [Inject]
      IOrderService OrderService { get; set; }

      [Inject]
      private UserSession UserSession { get; set; }

      public string AccountName { get; set; }

      #endregion Inject

      #region Proeprties

      private IEnumerable<CustomerOrderModel> CustomerOrders { get; set; }

      #endregion Properties

      #region Reference 

      /// <summary>
      /// ссылка на модал
      /// </summary>
      private Login modalLogin;

      #endregion Reference

      #region Methods

      protected override async Task OnInitializedAsync()
      {
         await UserSession.LoadSession();

         if (UserSession.IsSessionStarted)
         {
            CustomerOrders = await OrderService.GetCustomerOrders();
            AccountName = UserSession.Name;
         }
         else
            modalLogin?.openModalLogin();
      }
      #endregion Methods
   }
}
