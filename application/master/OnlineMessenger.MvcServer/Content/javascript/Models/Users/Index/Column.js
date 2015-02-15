function Column(name) {
    var self = this;
    self.Name = ko.observable(name);
    self.IsSort = ko.observable(false);
    self.IsAsc = ko.observable(false);
    self.onSelect = null;
    self.OnClick = function () {
        if (self.onSelect != null)
            self.onSelect(self.Name());
    };
}
