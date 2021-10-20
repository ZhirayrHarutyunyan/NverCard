using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace NverCard.Public.Client.Pages.Thanks
{
   public partial class Thanks
   {
      #region Inject

      [Inject]
      private NavigationManager NavigationManager { get; set; }

      [Inject]
      IJSRuntime JSRuntime { get; set; }

      #endregion Inject

      #region Properties

      [Parameter]
      public bool Visible { get; set; }

      #endregion Properties

      #region Methods

      public void OpenThanks()
      {
         Visible = true;
         StateHasChanged();
      }

      protected override void OnAfterRender(bool firstRender)
      {
         JSRuntime.InvokeVoidAsync("slickSliderConfigs.init");
      }

      #endregion Methods
   }
}
