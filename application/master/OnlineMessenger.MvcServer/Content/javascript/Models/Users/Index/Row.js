function Row(cells) {
    var self = this;
    self.Checked = ko.observable(false);
    self.Check = null;
    self.OnCheck = function () {
        if (self.Check != null)
            self.Check(self.Checked());
        return true;
    };
    self.Cells = ko.observableArray(cells);
    self.IsEditVisible = ko.observable(true);
    self.Edit = function () {
        document.location.href = document.location.origin + "/" + controller + "/Edit/?ids=" + self.Cells()[0].Value();
    };
    self.OnDelete = function () {
        if (self.Delete != null)
            self.Delete(self);
        return true;
    };

    self.Delete = null;
}

Row.FromRequest = function (data, table) {
    var index = 0;
    var cells = [];
    for (var i in definitions) {
        var def = definitions[i];
        var fieldValue = data[def.Name];
        var cell = new Cell(fieldValue, table.SortColumns()[index]);
        cells.push(cell);
        index++;
    }
    return new Row(cells);
};