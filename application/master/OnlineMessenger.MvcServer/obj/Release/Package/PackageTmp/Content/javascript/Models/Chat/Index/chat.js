var questions = [
    "Чем я вам могу помочь?",
    "Что вы делаете?",
    "Как вам это удалось?"
];

// view models
function Page() {
    var self = this;
    self.ListGroups = ko.observable([]);
    self.Users = ko.observableArray([]);
    self.RemoveUser = function (item) {
        self.Users.remove(item);
        var tab = page.GetTabByUserName(item.Name());
        if (tab != null) {
            self.Tabs.remove(tab);
        }

        page.ShowHideWelcomeMessage();

        postJSONToController("/RemoveOperatorFromVisible", { clientName: item.Name() }, function (data) {

        });
    };


    self.ShowHideWelcomeMessage = function () {
        page.IsVisibleWelcomeMessage(page.Tabs().length == 0);
    };
    self.FoundItemClicked = function (item) {
        if (!Enumerable.From(self.Users()).Any(function (x) { return x.Name() == item.Name(); })) {
            self.Users.push(item);
        }
        self.SearchText("");
        self.ShowSearchPanel(false);
        self.ShowUsersList(true);
        postJSONToController("/AddOperatorToVisible", { clientName: item.Name() }, function (data) {

        });
    };

    self.SortByOnline = function (x, y) {
        return (x.IsOnline() === y.IsOnline()) ? 0 : x.IsOnline() ? -1 : 1;
    };


    self.Operators = ko.observableArray([]);
    self.IsUsersExpanded = ko.observable(false);
    self.IsOperatorsExpanded = ko.observable(false);
    self.OperatorsClicked = function () {
        self.IsOperatorsExpanded(!self.IsOperatorsExpanded());
    };
    self.UsersClicked = function () {
        self.IsUsersExpanded(!self.IsUsersExpanded());
    };

    self.IsVisibleWelcomeMessage = ko.observable(true);
    self.ChatUsers = {};
    self.SearchResult = ko.observableArray([]);
    self.ShowSearchPanel = ko.observable(false);
    self.ShowUsersList = ko.observable(true);
    self.SearchText = ko.observable("");
    self.OnSearch = function () {
        delayExecute(function () {
            searchUsers(page.SearchText());
        });
    };
    self.Tabs = ko.observableArray([]);
    self.Smiles = ko.observableArray(new SmilesList());
    self.Questions = ko.observableArray(questions);

    function searchUsers(query) {
        if (query != "") {
            postJSONToController("/SearchUsersByName", { name: query }, function (data) {
                var foundUsers = [];
                for (var i in data) {
                    foundUsers.push(ListUser.FromRequest(data[i]));
                }
                page.ShowSearchPanel(true);
                page.ShowUsersList(false);
                page.SearchResult(foundUsers);
            });
        } else {
            page.SearchResult([]);
            page.ShowSearchPanel(false);
            page.ShowUsersList(true);
        }
    }

    self.ConvertUsersFromRequest = function (inputArray) {
        return Enumerable.From(inputArray)
            .Select(function (x) { return ListUser.FromRequest(x); })
            .ToArray();
    };

    self.LoadGroups = function () {
        postJSONToController("/allgroups", null, function (data) {
            var users = self.ConvertUsersFromRequest(data.Users);
            var operators = self.ConvertUsersFromRequest(data.Operators);
            self.Operators(operators);
            self.Users(users);
            for (var i in self.Users()) {
                self.Users()[i].IsClient(true);
            }
            page.Operators.sort(page.SortByOnline);
        });
    };

    self.LoadGroups();
    self.Chat = null;

    self.GetUserFromGroups = function (userName) {
        var user = null;
        var groupsArr = [self.Operators(), self.Users()];
        for (var i in groupsArr) {
            var found = self.GetUserInGroupListByName(groupsArr[i], userName);
            if (found != null) {
                user = found;
            }
        }
        return user;
    };

    self.IsMessageSendedToMe = function (message) {
        return Enumerable.From(message.To).Any(function (x) { return x.Name == currentUserName; });
    };

    self.HighLightNewMessagess = function (messagesCount) {
        if (messagesCount == 0)
            return;
        postJSONToController("/GetNewMessages", { messagesCount: messagesCount }, function (data) {
            for (var i in data) {
                self.ProcessNewMessage(data[i]);
            }
        });
    };

    self.HighLightUserAndTab = function (message, name) {
        var user = self.GetUserFromGroups(name);
        if (user == null) {
            postJSONToController("/AddOperatorToVisible", { clientName: name }, function (data) {
                self.Users.push(new ListUser(name, 1, false, true, true));
            });
            return;
        }

        user.NewMessagesCount(user.NewMessagesCount() + 1);

        var tab = self.GetTabByUserName(name);
        if (tab) {
            if (tab.IsVisible()) {
                tab.NewMessagesCount(0);
            }
            tab.MessagesList.push(Message.FromRequest(message));
            tab.ScrollToBottom(true);
        }
    };

    self.ProcessNewMessage = function (message) {
        if (message.From.Name == currentUserName)
            return;

        if (self.IsMessageSendedToMe(message)) {
            self.HighLightUserAndTab(message, message.From.Name);
        }
        else {
            for (var i in message.To) {
                var to = message.To[i];
                self.HighLightUserAndTab(message, to.Name);
            }
        }
    };

    self.InitHub = function () {
        var Chat = $.connection.chatHub;
        Chat.client.broadcast = function (message) {
            notifier.PlayNotification();
            message = $.parseJSON(message);
            self.ProcessNewMessage(message);
        };
        Chat.client.userConnected = function (serverUser) {
            serverUser = $.parseJSON(serverUser);
            var user = self.GetUserFromGroups(serverUser.Name);
            if (user == null) {
                self.addClientTolist(serverUser);
                return;
            }
            user.IsOnline(true);
            page.Operators.sort(page.SortByOnline);
        };
        Chat.client.userDisconnected = function (serverUser) {
            serverUser = $.parseJSON(serverUser);
            var user = self.GetUserFromGroups(serverUser.Name);
            if (user == null)
                return;
            user.IsOnline(false);
            page.Operators.sort(page.SortByOnline);
        };

        $.connection.hub.start();
    };

    self.addClientTolist = function (client) {
        var listUser = ListUser.FromRequest(client);
        listUser.IsNewUserVisible(true);
        listUser.IsOnline(true);
        self.Users.push(listUser);
        self.Users.sort(page.SortByOnline);
    };

    self.GetTabByUserName = function (userName) {
        return Enumerable.From(page.Tabs())
            .SingleOrDefault(null, function (x) { return x.UserName() == userName; });
    };

    self.GetUserInGroupListByName = function (group, userName) {
        return Enumerable.From(group)
             .SingleOrDefault(null, function (x) { return x.Name() == userName; });
    };
}

function ChatTab(listUser, messages, newMessagesCount) {
    var self = this;
    self.ListUser = listUser;
    self.UserName = listUser.Name;
    self.MessagesList = ko.observableArray(messages);
    self.CurrentMessage = ko.observable("");
    self.IsVisible = ko.observable(false);
    self.NewMessagesCount = newMessagesCount;
    self.ScrollToBottom = ko.observable(false);
    self.Send = function () {
        if (self.CurrentMessage().trim() != "") {
            postJSONToController("/Send", { userName: self.UserName(), text: self.CurrentMessage() }, function (data) {
                self.MessagesList.push(new Message(0, currentUserName, "/Content/images/big-user-icon.png", moment().format("D.MM.YY h:mm:ss"), self.CurrentMessage()));
                self.CurrentMessage("");
                notifier.PlayNotification();
                self.ScrollToBottom(true);
            });
        }
    };


    self.AddSmile = function (smileCode) {
        self.CurrentMessage(self.CurrentMessage() + " " + smileCode);
    };
    self.SelectAnswer = function (answer) {
        self.CurrentMessage(self.CurrentMessage() + " " + answer);
    };
    self.Select = function () {
        for (var t in page.Tabs()) {
            var tab = page.Tabs()[t];
            tab.IsVisible(false);
        }
        self.IsVisible(true);
        self.ScrollToBottom(true);
        self.NewMessagesCount(0);


    };
    self.Remove = function () {
        if (self.IsVisible() == true) {
            page.Tabs.remove(self);
            if (page.Tabs().length > 0) {
                $(page.Tabs()).last()[0].Select();
            }
        } else {
            page.Tabs.remove(self);
        }
        page.ShowHideWelcomeMessage();
    };

    self.LoadMessages = function () {
        postJSONToController("/GetMessagesOfUsers", { userName: self.UserName() }, function (data) {
            for (var i in data) {
                var item = data[i];
                self.MessagesList.push(Message.FromRequest(item));
                self.ScrollToBottom(true);
            }
        });
    };
    self.LoadMessages();
}

function ListUser(name, newMessagesCount, isNewUser, isOnline, isClient) {
    var self = this;
    self.Name = ko.observable(name);
    self.IsNewUserVisible = ko.observable(isNewUser);
    self.NewMessagesCount = ko.observable(newMessagesCount);
    self.IsOnline = ko.observable(isOnline);
    self.IsClient = ko.observable(isClient);
    self.Clicked = function () {
        self.IsNewUserVisible(false);
        var userTab = Enumerable.From(page.Tabs()).SingleOrDefault(null, function (x) { return x.UserName() == self.Name(); });
        if (!userTab) {
            page.Tabs.push(new ChatTab(self, [], self.NewMessagesCount));
            $(page.Tabs()).last()[0].Select();
        } else {
            userTab.Select();
        }

        page.ShowHideWelcomeMessage();
    };
}
ListUser.FromRequest = function (data) {
    return new ListUser(data.Name, 0, false, !data.IsConnected ? false : true, !Enumerable.From(data.Roles).Any());
};


