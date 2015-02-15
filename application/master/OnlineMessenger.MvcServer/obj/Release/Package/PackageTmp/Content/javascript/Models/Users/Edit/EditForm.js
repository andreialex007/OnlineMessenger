function EditForm(id, userName, email, password, passwordConfirm, roles) {
    var self = this;
    self.Id = ko.observable(id);
    self.EntityName = ko.observable("User");
    self.UserName = ko.observable(userName);
    self.Email = ko.observable(email);
    self.Password = ko.observable(password);
    self.PasswordConfirm = ko.observable(passwordConfirm);
    self.Roles = ko.observableArray(roles);
    self.IsChangePassword = ko.observable(false);
    self.IsRemoveVisible = ko.observable(false);
    self.IsChangePasswordCheckBoxVisible = ko.observable(true);

    self.Errors = ko.observableArray([]);
    self.IsShowErrors = ko.observable(false);
    self.ValidateForm = function () {
        self.Errors([]);
        if (!validateEmail(self.Email())) {
            self.Errors.push("Wrong email format");
        }
        if (!self.UserName()) {
            self.Errors.push("User name required");
        }
        if (self.IsChangePassword()) {
            if (!self.Password() || !self.PasswordConfirm()) {
                self.Errors.push("Password and password confurm fields cannot be empty");
            }
            if (self.Password() != self.PasswordConfirm()) {
                self.Errors.push("Password and confirmation not equals");
            }
        }
        var validateResult = self.Errors().length == 0;
        self.IsShowErrors(!validateResult);
        return validateResult;
    };

    if (!self.Id()) {
        self.IsChangePasswordCheckBoxVisible(false);
        self.IsChangePassword(true);
        self.IsRemoveVisible(true);
    }
    
}
