function AccountSaveButton(clickFunction) {
    var self = new SaveButton(clickFunction);
    self.SavingTexts = {
        Saving: "Saving...",
        Save: "Save"
    };
    self.SaveText(self.SavingTexts.Save);
    return self;
}