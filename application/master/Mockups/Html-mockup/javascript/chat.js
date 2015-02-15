// data
var users = [
		new ListUser("James", 0),
		new ListUser("Bill", 22),
		new ListUser("Kate", 0),
		new ListUser("Andrew", 0)
];
var operators = [
		new ListUser("Ivanov", 0),
		new ListUser("Petrov", 0),
		new ListUser("Sidorov", 3)
];
var searchResult = [
	new ListUser("User1", 0),
	new ListUser("User2", 0),
	new ListUser("User3", 0)
];

var messages = [
	new Message("Ivanov","images/big-user-icon.png","14.12.14 12:52:17","The attr binding provides a generic way to set the value of any attribute for the associated DOM element. This is useful, for example, when you need to set the title attribute of an element, the src of an img tag, or the href of a link based on values in your view model, with the attribute value being updated automatically whenever the corresponding model property changes."),
	new Message("Petrov","images/big-user-icon-admin.png","14.12.14 12:52:25","To summarise: KO doesn’t compete with jQuery or similar low-level DOM APIs. :-0 KO provides a complementary, high-level way to link a data model to a UI. KO itself doesn’t depend on jQuery, but you can certainly use jQuery at the same time, and indeed that’s often useful if you want things like animated transitions. Developers familiar with Ruby on Rails, ASP.NET MVC, or other MV* technologies may see MVVM as a real-time form of MVC with declarative syntax. In another sense, you can think of KO as a general way to make UIs for editing JSON data… whatever works for you "),
	new Message("Ivanov","images/big-user-icon.png","14.12.14 12:53:33","OK, how do you use it?")
];

var tabs = [
	new ChatTab("Ivanov", [], ko.observable(0)),
	new ChatTab("Petrov", messages, ko.observable(0)),
	new ChatTab("Sidorov", [], ko.observable(0))
];

var smiles = [
	new Smile(":)","images/smiles/victory.png"),
	new Smile(":-)","images/smiles/greedy.png"),
	new Smile(":-|","images/smiles/grimace.png"),
	new Smile(":-0","images/smiles/haha.png"),
	new Smile(":-(","images/smiles/unhappy.png"),
	new Smile(":->","images/smiles/victory.png")
];

var questions = [
	"How can i help you?",
	"What are you doing?",
	"How did you manage it?"
];


// view models
function Page(){
	var self = this;
	self.ListGroups = ko.observable([ 
		new Group("Users",users),
		new Group("Operators",operators)
	]);
	self.ChatUsers = {};
	self.SearchResult = ko.observableArray(searchResult);
	self.ShowSearchPanel = ko.observable(false);
	self.ShowUsersList = ko.observable(true);
	self.SearchText = ko.observable("");
	self.OnSearch = function(){
		delayExecute(function(){
			SearchUsers(page.SearchText());
		});
	}
	self.Tabs = ko.observableArray(tabs);
	self.Smiles = ko.observableArray(smiles);
	self.Questions = ko.observableArray(questions);
	
	//ajax methods
	function SearchUsers(query){
		console.log("ajax call SearchUsers=" + query);
		if(query != ""){
			page.ShowSearchPanel(true);
			page.ShowUsersList(false);
			page.SearchResult = ko.observableArray(searchResult);
		}else{
			page.SearchResult = ko.observableArray([]);
			page.ShowSearchPanel(false);
			page.ShowUsersList(true);
		}	
	}
}

function ChatTab(userName, messages, newMessagesCount){
	var self = this;
	self.UserName = ko.observable(userName);
	self.MessagesList = ko.observableArray(messages);
	self.CurrentMessage = ko.observable("");
	self.IsVisible = ko.observable(false);
	self.NewMessagesCount = newMessagesCount;
	self.ScrollToBottom = ko.observable(false);
	self.Send = function(){
		if(self.CurrentMessage().trim() != ""){
			self.MessagesList.push(new Message("Ivanov","images/big-user-icon.png",moment().format("D.MM.YY h:mm:ss"), self.CurrentMessage())); 
			self.CurrentMessage("");
			self.ScrollToBottom(true);
		}
	}
	
	
	self.AddSmile = function(smileCode){
		self.CurrentMessage(self.CurrentMessage() + " " + smileCode);
	}
	self.SelectAnswer = function(answer){
		self.CurrentMessage(self.CurrentMessage() + " " + answer);
	}
	self.Select = function(){
		for(var t in page.Tabs()){
			var tab = page.Tabs()[t];
			tab.IsVisible(false);
		}
		self.IsVisible(true);
		self.ScrollToBottom(true);
		self.NewMessagesCount(0);
	}
	self.Remove = function(){
		if(self.IsVisible()== true){
			page.Tabs.remove(self);
			if(page.Tabs().length > 0){
				$(page.Tabs()).last()[0].Select();
			}
		}else{
			page.Tabs.remove(self);
		}
	}
	
	
}

function Message(userName, avatarUrl, time, text){
	var self = this;
	self.UserName = ko.observable(userName);
	self.AvatarUrl = ko.observable(avatarUrl);
	self.Time = ko.observable(time);
	self.Text = ko.observable(text);
	self.TextWithSmiles = function(){
		var textWithSmiles = self.Text();
		for(var s in page.Smiles()){
			var smile = page.Smiles()[s];
			var imageTag = "<img width='16' height='16' src='" + smile.Path() + "' /> "
			textWithSmiles = textWithSmiles.split(smile.Code()).join(imageTag);
		}
		return textWithSmiles;
	}
}

function Group(name, users){
	var self = this;
	self.Name = ko.observable(name);
	self.Users = ko.observableArray(users);
	self.IsExpanded = ko.observable(true);
	self.Clicked = function(){
		self.IsExpanded(!self.IsExpanded());
	}
}

function ListUser(name, newMessagesCount){
	var self = this;
	self.Name = ko.observable(name);
	self.NewMessagesCount = ko.observable(newMessagesCount);
	self.Clicked = function(){
		var items = Enumerable.From(page.Tabs())
			.Where(function (x) { return x.UserName() == self.Name() })
			.ToArray()
		if(items.length == 0){
			page.Tabs.push(new ChatTab(self.Name(), [], self.NewMessagesCount));
			$(page.Tabs()).last()[0].Select();
		}else{
			items[0].Select();
		}
	}
}

function Smile(code, path){
	var self = this;
	self.Code = ko.observable(code);
	self.Path = ko.observable(path);
}

var page = new Page();
ko.applyBindings(page);


page.Tabs()[1].Select();
page.Tabs()[2].NewMessagesCount(55);
