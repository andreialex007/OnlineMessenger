function SendMail() {
    var self = {};
    self.Email = ko.observable("");
    self.UserName = ko.observable("");
    self.Message = ko.observable("");
    self.Code = ko.observable("");
    self.RefreshImage = ko.observable(false);
    self.Send = function () {
        var obj = {
            Email: self.Email(),
            UserName: self.UserName(),
            Code: self.Code(),
            Message: self.Message()
        };
        postJSONToController("/SendMail", obj, function (data) {
            if (data.Success == false) {
                alert("Input data incorrect or code incorrect");
            } else {
                alert("Your message succesfully sended");
                window.location.reload();
            }
            
        });
    };
    return self;
}