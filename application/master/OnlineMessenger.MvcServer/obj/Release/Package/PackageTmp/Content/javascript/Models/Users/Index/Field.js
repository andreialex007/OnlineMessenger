function Field(type, name, value) {
    var self = this;
    self.Type = ko.observable(type);
    self.Name = ko.observable(name);
    self.Value = ko.observable(value);
}
