function LoginPage() {
    var self = this;
    self.IsUserIconHidden = ko.observable(false);
    self.IsLoadingIconHidden = ko.observable(true);
    self.IsLoginButtonDisabled = ko.observable(false);
    self.IsHideErrorMessages = ko.observable(false);
    self.UserName = ko.observable("");
    self.Password = ko.observable("");
    self.RememberMe = ko.observable(false);
    self.Errors = ko.observable([]);

    self.LoginClick = function () {
        if (self.IsLoginButtonDisabled()) {
            return;
        }
        if (!self.Validate()) {
            self.SetButtonWaitState();

            $.ajax({
                type: 'POST',
                url: getUrl("/TryLogin"),
                data: { userName: self.UserName(), password: self.Password(), isPersist: self.RememberMe() },
                success: function (data) {
                    if (data.Success) {
                        var newLocation = location.origin + getParameterByName("ReturnUrl");
                        location.href = newLocation;
                    } else {
                        self.Errors(["Логин или пароль неверны!"]);
                        page.IsHideErrorMessages(true);
                    }
                    self.SetButtonDefaultState();
                },
                dataType: 'JSON'
            });
            setTimeout(function () {

            }, 2000);
        }
    };

    self.Validate = function () {
        self.Errors([]);
        var errors = [];
        if (!self.UserName()) {
            errors.push("Имя пользователя не может быть пустым! ");
        }
        if (!self.Password()) {
            errors.push("Пароль не может быть пустым! ");
        }
        self.Errors(errors);
        self.IsHideErrorMessages(errors.length != 0);
        return errors.length > 0;
    };

    self.SetButtonWaitState = function () {
        self.IsLoginButtonDisabled(true);
        self.IsLoadingIconHidden(false);
        self.IsUserIconHidden(true);
    };

    self.SetButtonDefaultState = function () {
        self.IsLoginButtonDisabled(false);
        self.IsLoadingIconHidden(true);
        self.IsUserIconHidden(false);
    };
}
