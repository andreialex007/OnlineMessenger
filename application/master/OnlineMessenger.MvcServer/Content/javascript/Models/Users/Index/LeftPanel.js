function LeftPanel() {
    var self = this;
    self.Fields = ko.observableArray([]);
    for (var d in definitions) {
        var definition = definitions[d];
        self.Fields.push(new Field(definition.Type, definition.Name, getCookie(definition.Name)));
    }

    self.EditDisabled = ko.observable(true);
    self.DeleteDisabled = ko.observable(true);
    self.New = function () {
        document.location.href = document.location.origin + "/" + controller + "/Edit";
    };
    self.Edit = function () {
        document.location.href = document.location.origin + "/" + controller + "/Edit/?ids=" + getSelectedIdsList();
    };
    self.Delete = null;
    self.OnDelete = function () {
        if (self.Delete != null) {
            var ids = getSelectedIdsList();
            self.Delete(ids);
        }
    };
    self.EnableEditDelete = null;
    self.OnPickDate = null;

    function getSelectedIdsList() {
        return Enumerable.From(page.ResultsArea.Table.Rows())
            .Where(function (x) { return x.Checked(); })
            .Select(function (x) { return x.Cells()[0].Value(); })
            .ToArray();
    }

    self.RemoveRowsByIds = function (userIds) {
        for (var i in userIds) {
            page.ResultsArea.Table.RemoveRowById(userIds[i]);
        }
    };


}