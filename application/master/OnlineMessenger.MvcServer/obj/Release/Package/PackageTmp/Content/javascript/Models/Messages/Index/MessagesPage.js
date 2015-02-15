definitions = [
    new FieldDefinition("Id", "Integer"),
    new FieldDefinition("From", "String"),
    new FieldDefinition("Text", "Contains"),
    new FieldDefinition("Date", "Date")
];

function MessagesPage() {
    var self = new UsersPage();
    self.SearchUrl("/SearchMessages");
    self.DeleteUrl("/DeleteMessages");
    self.Skip = 0;
    self.Take = 50;
    self.GetRow = function (item) {
        return MessageRow.FromRequest(item, self.ResultsArea.Table);
    };

    self.OnScroll = function (isLastRowVisible) {
        self.ResultsArea.IsLoading(isLastRowVisible);
        if (isLastRowVisible) {
            self.ExecuteSearch();
        }
    };

    self.AppendParameterToSend = function (data) {
        data.Take = self.Take;
        data.Skip = self.Skip;
        self.Skip += self.Take;
    };
    self.GetItemsFromRequest = function (data) {
        return data.items;
    };
    self.GetTotalFromRequest = function (data) {
        return data.total;
    };

    self.SetUpSearchConditions = function () {
        self.IsAppend = true;
    };
    
    self.SetDefaultSearchConditions = function () {
        self.Skip = 0;
        self.Take = 50;
    };

    self.IsAppend = true;
    return self;
}


MessageRow.FromRequest = function (data, table) {
    var index = 0;
    var cells = [];
    for (var i in definitions) {
        var def = definitions[i];
        var fieldValue = data[def.Name];
        var cell = new Cell(fieldValue, table.SortColumns()[index]);
        cells.push(cell);
        index++;
    }
    return new MessageRow(cells);
};