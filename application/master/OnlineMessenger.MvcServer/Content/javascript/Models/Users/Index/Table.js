function Table(columnNames) {
    var self = this;

    self.SortColumns = ko.observableArray([]);
    for (var i in columnNames) {
        var column = new Column(columnNames[i]);
        column.onSelect = function (colName) {
            self.OnColumnSelect(colName);
        };
        self.SortColumns.push(column);
    }
    self.GetSortColumn = function () {
        return Enumerable.From(self.SortColumns()).Single(function (x) { return x.IsSort() == true; });
    };
    self.IsLoadingVisible = ko.observable(false);
    
    self.ColumnSelect = null;
    self.OnColumnSelect = function (colName) {

        var column = Enumerable.
            From(self.SortColumns())
            .Single(function (x) { return x.Name() == colName; });

        if (column.IsSort() == true)
            column.IsAsc(!column.IsAsc());

        Enumerable.From(self.SortColumns()).
            ForEach(function (x) { x.IsSort(false); });

        column.IsSort(true);

        if (self.ColumnSelect != null)
            self.ColumnSelect();
    };

    self.Check = null;
    self.Rows = ko.observableArray([]);
    self.Checked = ko.observable(false);
    self.OnCheck = function () {
        for (var i in self.Rows()) {
            var row = self.Rows()[i];
            row.Checked(!self.Checked());
        }
        if (self.Check != null)
            self.Check(self.Checked());
        return true;
    };

    self.IsAllChecked = function () {
        return !Enumerable.From(self.Rows())
            .Any(function (x) { return !x.Checked(); });
    };
    self.AtLeastOneChecked = function () {
        return Enumerable.From(self.Rows())
            .Any(function (x) { return x.Checked(); });
    };

    self.GetRowById = function (id) {
        return Enumerable.From(self.Rows())
        .Single(function (x) { return x.Cells()[0].Value() == id; });
    };

    self.RemoveRowById = function (id) {
        var row = self.GetRowById(id);
        self.Rows.remove(row);
    };
}
