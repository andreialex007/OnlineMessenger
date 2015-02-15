function UserMessage(id, userName, avatarUrl, text) {
    var self = new Message(id, userName, avatarUrl, "", text);

    if (userName == "You") {
        self.AvatarUrl("/Account/GetAvatar/");
    }
    
    return self;
}

UserMessage.FromRequest = function (item) {
    return UserMessage(item.Id, item.From.Name, item.AvatarUrl, item.Text);
};

