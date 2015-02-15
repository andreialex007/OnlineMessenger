function RoleEditPage() {
    var self = new EditPage();
    self.AvaliableRoles = ko.observableArray([]);
    self.FormsList = ko.observableArray([]);
    self.EmptyItem = function () {
        return new RoleEditForm(0, "");
    };

    self.FormFromRequest = function (item) {
        return new RoleEditForm(item.Id, item.Name);
    };

    self.SaveOrEditUrl("/SaveOrEditRole");
    self.GetItemsUrl("/GetRolesById");
    self.FormUsersToServerUsers = function () {
        var forms = self.FormsList();
        var items = [];
        
        for (var i in forms) {
            var form = forms[i];
            var obj = {
                Id: form.Id(),
                Name: form.Name()
            };
            items.push(obj);
        }
        return items;
    };
    self.DataLoaded = function (data) {
        var roles = data.Roles;
        if (roles.length == 0) {
            self.FormsList([self.EmptyItem()]);
        } else {
            self.ApplyItemsToList(roles);
        }
    };

    return self;
}