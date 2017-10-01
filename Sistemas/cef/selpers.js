var mainBrowser = null;
var LocalPlaya; //= API.getLocalPlayer();

API.onServerEventTrigger.connect(function (name, args) {
    if (name == "UPDATE_CHARACTER") {
        LocalPlaya = args[0];
    }
});