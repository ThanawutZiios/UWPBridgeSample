(function () {
    'use strict';
    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            args.setPromise(WinJS.UI.processAll().then(function () {
                startapp();
                ensureListeningToBroker();
            }));
        }
    };

    app.start();

    function openBridgeError() {
        document.getElementById("lblStatus").innerText = "Open Bridge Fail";
    }

    function startwin32() {
        Windows.ApplicationModel.FullTrustProcessLauncher.launchFullTrustProcessForCurrentAppAsync().done(() => {
        }, openBridgeError);
    }

    function processCommand(command) {
        var connection = WRC.Bridge.getConnection();
        if (connection) {
            var inputs = new Windows.Foundation.Collections.ValueSet();
            inputs.insert("Command", command);
            connection.sendMessageAsync(inputs).done(
                function (result) {
                    new Windows.UI.Popups.MessageDialog("Command : " + command + "\n\n" + result.message.data).
                        showAsync();
                }
            );
        } else {

            new Windows.UI.Popups.MessageDialog("Connection is not open").
                showAsync();
        }
    }
    function getPrinter() {
        processCommand("getprinters");
    }
    function getCpu() {
        processCommand("getcpu");
    }
    function getMemory() {
        processCommand("getmemory");
    }
    function openIE() {
        processCommand("openie");
    }

    function startapp() {
        document.getElementById("btnStart").addEventListener("click", startwin32);
        document.getElementById("btnGetPrinter").addEventListener("click", getPrinter);
        document.getElementById("btnGetCPU").addEventListener("click", getCpu);
        document.getElementById("btnGetMemory").addEventListener("click", getMemory);
        document.getElementById("btnOpenIE").addEventListener("click", openIE);
    }

    // ThreadBroker
    var broker = WRC.ThreadBroker;
    var listeningToBroker = false;
    function appServiceRequestReceived(data) {
        document.getElementById("lblStatus").innerText = "Request Received, body = " + data.request.message.X1;
    }
    function ensureListeningToBroker() {
        if (!listeningToBroker) {
            broker.addEventListener("connectionarrived", function (data) {
                var connection = data.detail[0];
                var theBridge = WRC.Bridge;
                theBridge.setConnection(connection);

                connection.addEventListener("requestreceived", appServiceRequestReceived);
                document.getElementById("lblStatus").innerText = "Connection Arrived.";
            });

            broker.addEventListener("connectiondone", function (data) {
                document.getElementById("lblStatus").innerText = "Connection Done.";
            });

            listeningToBroker = true;
        }
    }
}())