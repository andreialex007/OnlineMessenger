function ResultsArea() {
    var self = this;
    self.IsLoading = ko.observable(false);
    var fieldNames = Enumerable.From(definitions)
        .Select(function (x) { return x.Name; })
        .ToArray();
    self.Table = new Table(fieldNames);
    
}
