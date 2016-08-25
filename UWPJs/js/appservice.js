(function () {
    "use strict";

    var backgroundTaskInstance = Windows.UI.WebUI.WebUIBackgroundTaskInstance.current;
    var triggerDetails = backgroundTaskInstance.triggerDetails;
    var connection = triggerDetails.appServiceConnection;

    WRC.ThreadBroker.postConnectionArrivedAsync(connection);
})();