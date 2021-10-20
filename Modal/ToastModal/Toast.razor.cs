using Microsoft.AspNetCore.Components;
using NverCard.Public.Client.Services.ToastService;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace NverCard.Public.Client.Modal.ToastModal
{
    public partial class Toast
    {
        #region Inject

        [Inject]
        IToastService ToastService { get; set; }

        #endregion Inject

        #region Paramter

        [Parameter]
        public string Message { get; set; }

        #endregion Parameter

        #region Properties

        public bool PopupToast { get; set; }

        public DateTime PopupDateTime { get; set; }

        private ToastLevel Level { get; set; }

        #endregion Properties

        #region Fields

        private CountdownTimer countdownTimer;
        private int progress = 100;              
        private static Timer aTimer = new Timer(1000);

        #endregion Fields

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            ToastService.OnShow += ShowToast;
        }

        public void StartTimer()
        {
            PopupDateTime = DateTime.Now;
            PopupDateTime = PopupDateTime.AddSeconds(2);

            //aTimer = new Timer(1000);
            if (PopupDateTime > DateTime.Now)
                aTimer.Elapsed += CountDownTimer;
            aTimer.Enabled = true;
        }

        public void CountDownTimer(Object source, ElapsedEventArgs e)
        {
            if (PopupDateTime < DateTime.Now)
            {
                aTimer.Enabled = false;
                aTimer.Stop();
                PopupToast = false;
            }
            InvokeAsync(StateHasChanged);
        }

        public void RemoveToast()
        {
            InvokeAsync(() =>
            {
                PopupToast = false;
                StateHasChanged();
            });
        }

        private void ShowToast(ToastLevel level, string message, bool show)
        {
            RemoveToast();
            InvokeAsync(() =>
            {
                Message = message;
                Level = level;
                PopupToast = show;
                StartTimer();

                StateHasChanged();
            });
        }

        private void CloseToast()
        {
            InvokeAsync(() =>
            {
                Message = "";
                PopupToast = false;
                PopupDateTime = DateTime.Now;
                StateHasChanged();
            });
        }

        //private async void CalculateProgress(int percentComplete)
        //{
        //    progress = 100 - percentComplete;
        //    await InvokeAsync(StateHasChanged);
        //}

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ToastService.OnClose -= CloseToast;
            countdownTimer.Dispose();
            countdownTimer = null;
        }

        public string GetToastLevelStyle()
        {
            return Level.ToString().ToLower();
        }

        #endregion Methods
    }
}
