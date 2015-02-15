function SaveButton(clickFunction) {
    var self = {};
    self.SavingTexts = {
        Saving: "Saving...",
        Save: "Save items"
    };
    self.SaveText = ko.observable(self.SavingTexts.Save);
    self._isSaving = ko.observable(false);
    self.IsSaving = ko.computed({
        read: function () {
            return self._isSaving();
        },
        write: function (value) {
            self._isSaving(value);
            self.SaveText(value ? self.SavingTexts.Saving : self.SavingTexts.Save);
        }
    });
    self.IsSuccessVisible = ko.observable(false);
    self.IsErrorVisible = ko.observable(false);
    self.SetResult = function (result) {
        if (result == "Success") {
            self.IsSuccessVisible(true);
        }
        if (result == "Error") {
            self.IsErrorVisible(true);
        }
        setTimeout(function () {
            self.IsSuccessVisible(false);
            self.IsErrorVisible(false);
        }, 500);
    };
    self.IsSaving(false);
    self.OnSave = clickFunction;

    return self;
}