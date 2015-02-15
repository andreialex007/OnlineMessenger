function Cell(value, column) {
    var self = this;
    self.Value = ko.observable(value);
    self.Column = column;
}