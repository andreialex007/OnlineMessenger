function RoleEditForm(id, name) {
    var self = this;
    self.Id = ko.observable(id);
    self.Name = ko.observable(name);
    self.IsRemoveVisible = ko.observable(false);

    self.Errors = ko.observableArray([]);
    self.IsShowErrors = ko.observable(false);
    self.ValidateForm = function () {
        self.Errors([]);
        if (!self.Name()) {
            self.Errors.push("Role name is required");
        }

        var validateResult = self.Errors().length == 0;
        self.IsShowErrors(!validateResult);
        return validateResult;
    };
    self.EntityName = ko.observable("Role");


    if (!self.Id()) {
        self.IsRemoveVisible(true);
    }
}