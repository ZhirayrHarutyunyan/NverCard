using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Net.ConnectCode.Barcode;

namespace NverCard.Public.Client.Modal.QrcodeModal
{
    public partial class QrcodeModal
    {
        #region Properties

        public bool IsVisible { get; set; }

        public string Barcode { get; set; }

        public string BarcodeText { get; set; }

        public string Height { get; set; } = "200px";
        public string Width { get; set; } = "300px";

        //Telerik.Blazor.Components.TelerikBarcode TelerikBarcodeRef { get; set; }
        #endregion Properties

        #region Parameter

        [Parameter]
        public string QrcodeUrl { get; set; }

        #endregion Parameter

        #region Methods

        public async void Open(string data)
        {
            IsVisible = true;
            QrcodeUrl = data;
            //await GenerateBarcode(data);
            StateHasChanged();
        }

        public async void Close()
        {
            IsVisible = false;
            StateHasChanged();
        }

        //public async Task GenerateBarcode(string data)
        //{
        //    BarcodeFonts bcf = new BarcodeFonts();
        //    bcf.BarcodeType = BarcodeFonts.BarcodeEnum.Code39;
        //    bcf.CheckDigit = BarcodeFonts.YesNoEnum.Yes;
        //    bcf.Data = data;
        //    bcf.encode();
        //    barcode = bcf.EncodedData;
        //    barcode_text = bcf.HumanText;
        //}

        #endregion Methods


    }
}
