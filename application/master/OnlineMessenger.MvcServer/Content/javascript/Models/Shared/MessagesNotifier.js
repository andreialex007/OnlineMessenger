var newMessagesCount = 0;
function MessagesNotifier() {
    var self = {};
    self.cookieName = "NewMessages";
    self.VisualNotificationsEnabled = ko.observable(false);
    self.AudioNotificationsEnabled = ko.observable(false);
    self.RefreshImage = ko.observable(false);
    self.IsShowNewMessages = ko.observable(false);
    self.TopNewMessages = ko.observable(0);
    self.SetNewMessages = function (val) {
        if (!self.VisualNotificationsEnabled())
            return;
        self.IsShowNewMessages(val > 0);
        self.TopNewMessages(val);
    };


    self.InitHub = function (data) {
        var Chat = $.connection.chatHub;
        Chat.client.broadcast = function (message) {
            self.PlayNotification();
            newMessagesCount++;
            setGlobalCookie(self.cookieName, newMessagesCount);
            self.SetNewMessages(newMessagesCount);
        };
        $.connection.hub.start();
    };

    self.PlayNotification = function() {
        if (self.AudioNotificationsEnabled())
            self.playAudio(true);
    };
    self.playAudio = ko.observable(false);

    self.LoadCurrentMessagesCount = function () {
        var rawValue = getGlobalCookie(self.cookieName);
        newMessagesCount = !rawValue ? newMessagesCount : parseInt(rawValue);

    };

    self.RemoveNotifications = function () {
        page.HighLightNewMessagess(newMessagesCount);
        newMessagesCount = 0;
        setGlobalCookie("NewMessages", newMessagesCount);
        self.SetNewMessages(newMessagesCount);
    };
    self.Init = function () {
        console.log("notifier initialized");
        postJSONToControllerExactUrl("/Account/GetMessengerSettings", {}, function (data) {
            self.VisualNotificationsEnabled(data.VisualNotificationsEnabled);
            self.AudioNotificationsEnabled(data.AudioNotificationsEnabled);
        });
    };

    self.Init();
    return self;
}