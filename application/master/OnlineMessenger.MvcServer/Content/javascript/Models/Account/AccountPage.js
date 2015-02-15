function AccountPage() {
    var self = {};
    self.SaveButton = new AccountSaveButton(function () {
        var isValid = self.ValidateForm();
        self.SaveButton.IsSaving(true);
        if (isValid == true) {
            var settings = {
                AudioNotificationsEnabled: self.AudioNotificationsEnabled(),
                VisualNotificationsEnabled: self.VisualNotificationsEnabled(),
                Email: self.Email(),
                IsChangePassword: self.IsChangePassword(),
                CurrentPassword: self.CurrentPassword(),
                NewPassword: self.NewPassword(),
                PasswordConfirm: self.PasswordConfirm()
            };

            postJSONToController("/EditUserSettings", settings, function (data) {
                if (data.Success == true) {
                    self.Errors([]);
                    self.IsShowErrors(false);
                    self.SaveButton.SetResult("Success");
                    self.IsChangePassword(false);
                    self.CurrentPassword("");
                    self.NewPassword("");
                    self.PasswordConfirm("");
                } else {
                    var errors = Enumerable.From(data.Errors)
                        .Select(function (x) { return x.Description; })
                        .ToArray();
                    self.Errors(errors);
                    self.IsShowErrors(true);
                    page.SaveButton.SetResult("Error");
                }
                self.SaveButton.IsSaving(false);
            });
        } else {
            self.IsShowErrors(true);
            page.SaveButton.SetResult("Error");
            self.SaveButton.IsSaving(false);
        }
    });


    self.FileObject = null;
    self.OnImageSelected = function (file) {
        self.FileObject = file;
        self.SelectedImageName(file.name);
        self.IsUploadButtonEnabled(true);
    };

    self.RefreshImage = ko.observable(false);
    self.SelectedImageName = ko.observable("");
    self.UploadingImageTextDefault = "UploadImage image";
    self.UploadingImageTextInProgress = "Loading...";
    self.UploadingImageText = ko.observable(self.UploadingImageTextDefault);
    self.IsUploadButtonEnabled = ko.observable(false);
    self.UploadImageClick = function () {
        if (self.IsUploadButtonEnabled() == true) {
            getJSON(getUrl("/UploadAvatar"), self.FileObject, function (data) {
                self.SelectedImageName("");
                self.UploadingImageText(self.UploadingImageTextDefault);
                self.RefreshImage(true);
                notifier.RefreshImage(true);
            });
            self.IsUploadButtonEnabled(false);
            self.UploadingImageText(self.UploadingImageTextInProgress);
        }
    };

    self.AudioNotificationsEnabled = ko.observable(false);
    self.VisualNotificationsEnabled = ko.observable(false);
    self.IsChangePassword = ko.observable(false);
    self.Errors = ko.observableArray([]);
    self.IsShowErrors = ko.observable(false);
    self.Email = ko.observable("");
    self.CurrentPassword = ko.observable("");
    self.NewPassword = ko.observable("");
    self.PasswordConfirm = ko.observable("");

    self.ValidateForm = function () {
        self.Errors([]);
        if (!validateEmail(self.Email())) {
            self.Errors.push("Wrong email format");
        }
        if (self.IsChangePassword()) {
            if (!self.CurrentPassword()) {
                self.Errors.push("Current password cannot be empty");
            }
            if (!self.NewPassword() || !self.PasswordConfirm()) {
                self.Errors.push("Password and password confurm fields cannot be empty");
            }
            if (self.NewPassword() != self.PasswordConfirm()) {
                self.Errors.push("Password and confirmation not equals");
            }
        }
        var validateResult = self.Errors().length == 0;
        return validateResult;
    };
    self.Init = function () {
        postJSONToController("/GetUserSettings", {}, function (data) {
            self.AudioNotificationsEnabled(data.AudioNotificationsEnabled);
            self.VisualNotificationsEnabled(data.VisualNotificationsEnabled);
            self.Email(data.Email);
        });
    };
    return self;
}