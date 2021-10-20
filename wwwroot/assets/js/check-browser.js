function checkBrowser() {
   var BrowserDetect = {
      init: function () {
         this.browser = this.searchString(this.dataBrowser) || "Other";
         this.version = this.searchVersion(navigator.userAgent) || this.searchVersion(navigator
            .appVersion) || "Unknown";
      },
      searchString: function (data) {
         for (var i = 0; i < data.length; i++) {
            var dataString = data[i].string;
            this.versionSearchString = data[i].subString;

            if (dataString.indexOf(data[i].subString) !== -1) {
               return data[i].identity;
            }
         }
      },
      searchVersion: function (dataString) {
         var index = dataString.indexOf(this.versionSearchString);
         if (index === -1) {
            return;
         }

         var rv = dataString.indexOf("rv:");
         if (this.versionSearchString === "Trident" && rv !== -1) {
            return parseFloat(dataString.substring(rv + 3));
         } else {
            return parseFloat(dataString.substring(index + this.versionSearchString.length + 1));
         }
      },

      dataBrowser: [{
            string: navigator.userAgent,
            subString: "Edge",
            identity: "MS Edge"
         },
         {
            string: navigator.userAgent,
            subString: "MSIE",
            identity: "Explorer"
         },
         {
            string: navigator.userAgent,
            subString: "Trident",
            identity: "Explorer"
         },
         {
            string: navigator.userAgent,
            subString: "Firefox",
            identity: "Firefox"
         },
         {
            string: navigator.userAgent,
            subString: "Opera",
            identity: "Opera"
         },
         {
            string: navigator.userAgent,
            subString: "OPR",
            identity: "Opera"
         },

         {
            string: navigator.userAgent,
            subString: "Chrome",
            identity: "Chrome"
         },
         {
            string: navigator.userAgent,
            subString: "Safari",
            identity: "Safari"
         }
      ]
   };

   BrowserDetect.init();
   var browser = BrowserDetect.browser;
   var version = BrowserDetect.version;
   var isOld = browser == "Edge" && version < 84 ||
      browser == "Explorer"
   browser == "Firefox" && version < 81 ||
      browser == "Opera" && version < 84 ||
      browser == "Chrome" && version < 85 ||
      browser == "Safari" && version < 13;
   var href = document.getElementsByTagName('base')[0].href + "outdated-browsers.html";
   if (isOld) {
      window.location.href(href);
   }
}