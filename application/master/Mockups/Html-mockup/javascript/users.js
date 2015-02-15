//data
var now = moment().toString();
var rows = [
	new Row("1", "Joyce", "Ivanov@mail.ru", "Admin, User", "", now),
	new Row("2", "Patti", "Petrov@mail.ru", "Admin, Operator", "", now),
	new Row("3", "Neal", "Neal@mail.ru", "Admin, User", "", now),
	new Row("4", "Kristy", "Kristy@mail.ru", "Admin, User", "", now),
	new Row("5", "Kristen", "Kristen@mail.ru", "Admin", "", now),
	new Row("6", "Adam", "Adam@mail.ru", "Operator", "", now),
	new Row("16", "Scott", "Scott@mail.ru", "User", "", now),
	new Row("25", "Sandra", "Sandra@mail.ru", "Operator", "", now),
	new Row("34", "Jeffrey", "Jeffrey@mail.ru", "Operator", "", now),
	new Row("35", "Margaret", "Margaret@mail.ru", "Operator", "", now),
	new Row("37", "Mildred", "Mildred@mail.ru", "Operator", "", now),
	new Row("37", "Victor", "Victor@mail.ru", "Admin", "", now),
	new Row("38", "Irene", "Irene@mail.ru", "Operator", "", now),
	new Row("39", "Anthony", "Anthony@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("40", "Albert", "Albert@mail.ru", "Operator", "", now),
	new Row("45", "Elizabeth", "Elizabeth@mail.ru", "User", "", now)
];

var searchRows = [
	new Row("7", "Christopher", "Christopher@mail.ru", "Operator", "", now),
	new Row("8", "Ronald", "Ronald@mail.ru", "Operator", "", now),
	new Row("9", "Lisa", "Lisa@mail.ru", "User, Admin", "", now)
];


// viewmodels
function Page(){
	var self = this;
	self.LeftPanel = new LeftPanel();
	self.ResultsArea = new ResultsArea();
}

function LeftPanel(){
	var self = this;
	self.UserIds = ko.observable("");
	self.UserNames = ko.observable("");
	self.CreatedDate = ko.observable("");
	self.Email = ko.observable("");
	self.Roles = ko.observable("");
	self.EditDisabled = ko.observable(true);
	self.DeleteDisabled = ko.observable(true);
	self.New = function(){
		console.log("New");
	}
	self.Edit = function(){
		console.log("Edit");
	}
	self.Del = function(){
		console.log("Del");
	}
	self.OnSearch = function(){
		console.log("onsearch");
		if(self.UserNames() == ""){
			page.ResultsArea.Table.Rows(rows)
		}else{
			page.ResultsArea.Table.Rows(searchRows)
		}
	}
	self.EnableEditDelete = function(){
		var checked = !page.ResultsArea.Table.AtLeastOneChecked();
		self.EditDisabled(checked);
		self.DeleteDisabled(checked);
	}
	self.OnPickDate = function(){
		page.LeftPanel.OnSearch()
	}
}

function ResultsArea(){
	var self = this;
	self.TotalFound = ko.observable(0);
	self.Table = new Table(rows);
}

function Table(rows){
	var self = this;
	self.SortColumns = ko.observableArray([
		new Column("Id"),
		new Column("UserName"),
		new Column("Email"),
		new Column("Roles"),
		new Column("Created"),
		new Column("Messages")
	]);
	self.SelectSortColumn = function(colName){
		var column = Enumerable.From(self.SortColumns()).Single(function(x){ return x.Name() == colName }); //select column by name
		if(column.IsSort() == true)
			column.IsAsc(!column.IsAsc());
		Enumerable.From(self.SortColumns()).ForEach(function(x){ x.IsSort(false) }); // disable sorting in each column
		column.IsSort(true);
	}
	
	self.SortField = ko.observable("");
	self.Checked = ko.observable(false);
	self.OnCheck = function(){
		for(var i in page.ResultsArea.Table.Rows()){
			var row = page.ResultsArea.Table.Rows()[i];
			row.Checked(!self.Checked());
		}
		page.LeftPanel.EnableEditDelete();
		return true;
	}
	self.Rows = ko.observableArray(rows);
	self.IsAllChecked = function(){
		return !Enumerable.From(page.ResultsArea.Table.Rows())
			.Any(function(x){ return !x.Checked() });
	}
	self.AtLeastOneChecked = function(){
		return Enumerable.From(page.ResultsArea.Table.Rows())
			.Any(function(x){ return x.Checked() });
	}
	
}



function Row(id, userName, email, roles, messages, created){
	var self = this;
	self.Checked = ko.observable(false);
	self.OnCheck = function(){
		if(this.Checked() == false)
			page.ResultsArea.Table.Checked(false);
		else{
			if(page.ResultsArea.Table.IsAllChecked())
				page.ResultsArea.Table.Checked(true);
		}
		page.LeftPanel.EnableEditDelete();
		return true;
	}
	self.Id = ko.observable(id);
	self.UserName = ko.observable(userName);
	self.Email = ko.observable(email);
	self.Roles = ko.observable(roles);
	self.Messages = ko.observable(messages);
	self.Created = ko.observable(created);
	self.Edit = function(){
		console.log("Edit");
	}
	self.Delete = function(){
		console.log("Delete");
	}
}

function Column(name){
	var self = this;
	self.Name = ko.observable(name);
	self.IsSort = ko.observable(false);
	self.IsAsc = ko.observable(false);
	self.OnClick = function(){
		page.ResultsArea.Table.SelectSortColumn(self.Name());
	}
}


var page = new Page();
ko.applyBindings(page);


































































