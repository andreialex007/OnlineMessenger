function MessageRow(cells) {
    var self = new Row(cells);
    self.Edit = function () {
        console.log("Edit");
    };
    self.IsEditVisible(false);
    return self;
}