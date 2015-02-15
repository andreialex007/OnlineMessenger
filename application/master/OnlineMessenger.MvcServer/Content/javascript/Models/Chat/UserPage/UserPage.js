function UserPage() {
    var self = {};
    self.ClientName = "You";
    self.UserMessages = ko.observableArray([]);
    self.InitHub = function () {
        var Chat = $.connection.chatHub;
        Chat.client.broadcast = function (data) {
            data = $.parseJSON(data);
            var message = UserMessage.FromRequest(data);
            self.UserMessages.push(message);
            self.ScrollToBottom(true);
        };
        $.connection.hub.start({ jsonp: true });
    };
    self.MessageTextArea = ko.observable("");
    self.SendMessage = function () {
        var text = self.MessageTextArea();
        var newMessage = new UserMessage(0, self.ClientName, "/Content/images/big-user-icon.png", text);
        self.UserMessages.push(newMessage);
        self.ScrollToBottom(true);
        self.SendMessageToServer();
        self.MessageTextArea("");
    };
    self.ScrollToBottom = ko.observable(false);
    self.SendMessageToServer = function () {
        postJSONToController("/AddMessageFromClient", { text: self.MessageTextArea() }, function (data) {
            console.log("posted");
        });
    };

    self.Init = function () {
        self.InitHub();
        postJSONToController("/GetClientMessages", null, function (data) {
            var messages = [];
            for (var i in data) {
                var item = data[i];
                var message = UserMessage.FromRequest(item);
                if (message.UserName() == currentUserName) {
                    message.UserName("You");
                }
                messages.push(message);
            }
            self.UserMessages(messages);
        });
    };
    return self;
}