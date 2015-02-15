

pagePath = controller;
var definitions = [
    new FieldDefinition("Id", "Integer"),
    new FieldDefinition("Name", "String"),
    new FieldDefinition("Email", "String"),
    new FieldDefinition("CreatedDate", "Date"),
    new FieldDefinition("Roles", "String"),
    new FieldDefinition("MessagesCount", "Count")
];

/*  viewmodels */
function UsersPage() {
    var self = {};
    self.LeftPanel = new LeftPanel();
    self.ResultsArea = new ResultsArea();
    self.SearchUrl = ko.observable("/SearchUsers");
    self.DeleteUrl = ko.observable("/DeleteUsers");

    self.OnSearch = function () {
        delayExecute(
            function () {
                self.SetDefaultSearchConditions();
                self.IsAppend = false;
                self.ExecuteSearch();
            });
    };


    self.IsAppend = false;
    self.ResultsArea.Table.ColumnSelect = function () {
        self.SetDefaultSearchConditions();
        self.IsAppend = false;
        self.ExecuteSearch();
    };
    self.ResultsArea.Table.Check = function () {
        self.LeftPanel.EnableEditDelete();
    };

    self.LeftPanel.EnableEditDelete = function () {
        var checked = !self.ResultsArea.Table.AtLeastOneChecked();
        self.LeftPanel.EditDisabled(checked);
        self.LeftPanel.DeleteDisabled(checked);
    };

    self.LeftPanel.OnPickDate = function () {
        self.OnSearch();
    };

    self.SetDefaultSearchConditions = function () {

    };

    self.OnScroll = function (isLastRowVisible) {

    };

    self.GetRow = function (item) {
        return Row.FromRequest(item, self.ResultsArea.Table);
    };

    self.LoadItems = function (fields, orderBy, isAsc) {
        self.ResultsArea.IsLoading(true);
        var toSend = {};
        for (var f in fields) {
            var field = fields[f];
            if (field.Type() == "Date") {
                toSend[field.Name()] = field.Value();
            }
            if (field.Type() == "Contains") {
                toSend[field.Name()] = field.Value();
            }
            if (field.Type() == "String" || field.Type() == "Integer") {
                toSend[field.Name()] = cleanArray(field.Value().split(','));
            }
        }

        toSend.orderBy = orderBy;
        toSend.isAsc = isAsc;
        self.AppendParameterToSend(toSend);
        $.ajax({
            type: 'POST',
            url: getUrl(self.SearchUrl()),
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(toSend),
            success: function (response) {
                var data = self.GetItemsFromRequest(response);
                self.Total(self.GetTotalFromRequest(response));
                var rows = [];
                for (var i in data) {
                    var row = self.GetRow(data[i]);
                    row.Delete = self.DeleteRow;

                    row.Check = function (checked) {
                        if (checked == false)
                            self.ResultsArea.Table.Checked(false);
                        else {
                            if (self.ResultsArea.Table.IsAllChecked())
                                self.ResultsArea.Table.Checked(true);
                        }
                        self.LeftPanel.EnableEditDelete();
                    };
                    rows.push(row);
                }

                if (self.IsAppend) {
                    var currentArray = self.ResultsArea.Table.Rows();
                    var resultArray = currentArray.concat(rows);
                    self.ResultsArea.Table.Rows(resultArray);
                } else {
                    self.ResultsArea.Table.Rows(rows);
                }
                self.ResultsArea.IsLoading(false);
                self.SetUpSearchConditions();
            },
            dataType: 'JSON'
        });
    };

    self.SetUpSearchConditions = function () {
        self.IsAppend = false;
    };
    self.Total = ko.observable(0);
    self.GetItemsFromRequest = function (data) {
        return data;
    };
    self.GetTotalFromRequest = function (data) {
        return data.length;
    };

    self.AppendParameterToSend = function (data) {

    };

    self.DeleteRow = function (row) {
        if (confirm("Do you really want to delete current item?")) {
            var ids = [row.Cells()[0].Value()];
            $.ajax({
                type: 'POST',
                url: getUrl(self.DeleteUrl()),
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ ids: ids }),
                success: function (data) {
                    if (data.Success) {
                        page.ResultsArea.Table.Rows.remove(row);
                        page.LeftPanel.EditDisabled(true);
                        page.LeftPanel.DeleteDisabled(true);
                    }
                },
                dataType: 'JSON'
            });
        }
    };

    self.ExecuteSearch = function () {
        var sortColumn = self.ResultsArea.Table.GetSortColumn();

        for (var i in self.LeftPanel.Fields()) {
            var field = self.LeftPanel.Fields()[i];
            setCookie(field.Name(), field.Value());
        }

        setCookie('Sort.Name', sortColumn.Name());
        setCookie('Sort.IsAsc', sortColumn.IsAsc());

        self.LoadItems(
            self.LeftPanel.Fields(),
            sortColumn.Name(),
            sortColumn.IsAsc()
        );
    };

    self.LeftPanel.Delete = function (ids) {
        if (confirm("Do you really want to delete selected items?")) {
            $.ajax({
                type: 'POST',
                url: getUrl(self.DeleteUrl()),
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ ids: ids }),
                success: function (data) {
                    if (data.Success) {
                        self.LeftPanel.RemoveRowsByIds(ids);
                        page.LeftPanel.EditDisabled(true);
                        page.LeftPanel.DeleteDisabled(true);
                    }
                },
                dataType: 'JSON'
            });
        }
    };


    self.Init = function () {
        initSortCookie();
    };

    function initSortCookie() {
        var sortName = getCookie("Sort.Name");
        if (sortName == "")
            sortName = self.ResultsArea.Table.SortColumns()[0].Name();
        var isAsc = getCookie("Sort.IsAsc");
        if (isAsc == "")
            isAsc = false;
        self.ResultsArea.Table.OnColumnSelect(sortName);
        var column = self.ResultsArea.Table.GetSortColumn();
        column.IsAsc(isAsc);
    }

    return self;

}






























































