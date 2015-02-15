function Message(id, from, avatarUrl, time, text) {
    var self = {};
    self.Id = ko.observable(id);
    self.UserName = ko.observable(from);
    self.AvatarUrl = ko.observable("/Account/GetAvatar/?userName=" + from);
    self.Time = ko.observable(time);
    self.Text = ko.observable(text);
    self.TextWithSmiles = function () {
        var textWithSmiles = self.Text();
        var smiles = new SmilesList();
        for (var s in smiles) {
            var smile = smiles[s];
            var imageTag = "<img width='16' height='16' src='" + smile.Path() + "' /> ";
            textWithSmiles = textWithSmiles.split(smile.Code()).join(imageTag);
        }
        return textWithSmiles;
    };
    return self;
}

Message.FromRequest = function (data) {
    return new Message(data.Id, data.From.Name, "/Content/images/big-user-icon.png", moment(data.Date).format("D.MM.YY h:mm:ss"), data.Text);
};