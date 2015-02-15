function EditPage() {
    var self = {};
    self.AvaliableRoles = ko.observableArray([]);
    self.FormsList = ko.observableArray([]);
    self.SaveOrEditUrl = ko.observable("/SaveOrEditUsers");
    self.GetItemsUrl = ko.observable("/GetUsersByIds");
    self.EmptyItem = function() {
        return new EditForm("", "", "", "", "", "");
    };
    self.RemoveForm = function (item) {
        self.FormsList.remove(item);
    };
    self.AddForm = function () {
        self.FormsList.push(self.EmptyItem());
    };

    self.GoToList = function() {
        document.location.href = document.location.origin + getUrl("/");
    };

    self.SaveItems = function () {

        $.ajax({
            type: 'POST',
            url: getUrl(self.SaveOrEditUrl()),
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(self.FormUsersToServerUsers()),
            success: function (data) {
                self.SaveButton.IsSaving(false);
                if (data.Success == true) {
                    self.SaveButton.SetResult("Success");
                    for (var i in self.FormsList()) {
                        var item = data.Items[i];
                        self.FormsList.replace(self.FormsList()[i], self.FormFromRequest(item));
                    }
                } else {
                    self.SaveButton.SetResult("Error");
                    for (var i in self.FormsList()) {
                        var form = self.FormsList()[i];
                        var errors = Enumerable.From(data.Errors[i].ValidationErrors)
                            .Select(function (x) { return x.Description; })
                            .ToArray();
                        form.Errors(errors);
                        page.FormsList()[i].IsShowErrors(errors.length > 0);
                    }
                }
            },
            dataType: 'JSON'
        });
    };

    self.FormUsersToServerUsers = function () {
        var forms = self.FormsList();
        var users = [];

        for (var i in forms) {

            var form = forms[i];
            var roles = Enumerable.From(self.AvaliableRoles())
                .Where(function (x) { return form.Roles().indexOf(x.Id.toString()) != -1; })
                .ToArray();

            var obj = {
                Id: form.Id(),
                Name: form.UserName(),
                Email: form.Email(),
                Password: form.Password(),
                PasswordConfirm: form.PasswordConfirm(),
                IsChangePassword: form.IsChangePassword(),
                Roles: roles,
            };
            users.push(obj);
        }
        return users;
    };
    self.SaveButton = new SaveButton(function () {
        self.SaveButton.IsSaving(true);
        var isErrors = false;
        for (var i in self.FormsList()) {
            var item = self.FormsList()[i];
            if (!item.ValidateForm()) {
                isErrors = true;
            }
        }

        if (isErrors == true) {
            self.SaveButton.IsSaving(false);
            self.SaveButton.SetResult("Error");
        } else {
            self.SaveItems();
        }
    });

    self.DataLoaded = function(data) {
        var users = data.Users;
        self.AvaliableRoles(data.Roles);
        if (users.length == 0) {
            self.FormsList([self.EmptyItem()]);
        } else {
            self.ApplyItemsToList(users);
        }
    };

    self.ApplyItemsToList = function(items) {
        var forms = [];
        for (var i in items) {
            var item = items[i];
            var form = self.FormFromRequest(item);
            forms.push(form);
        }
        self.FormsList(forms);
    };

    self.Init = function () {
        var ids = cleanArray(getParameterByName("ids").split(","));

        $.ajax({
            type: 'POST',
            url: getUrl(self.GetItemsUrl()),
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(ids),
            success: self.DataLoaded,
            dataType: 'JSON'
        });
    };
    

    self.FormFromRequest = function(item) {
        return new EditForm(item.Id, item.Name, item.Email, "", "", Enumerable.From(item.Roles).Select(function(x) { return x.Id.toString(); }).ToArray());
    };
    return self;
}

