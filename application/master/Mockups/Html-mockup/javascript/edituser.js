// data
var avaliableRoles = ["Admin", "User", "Operator"];
var roles = ["Admin", "User"];

var forms = [
	new EditForm(41,"Carlos","Carlos@mai.ru", "", "", roles),
	new EditForm(22,"Patrick","Patrick@mai.ru", "", "", roles),
	new EditForm(35,"Jennifer","Jennifer@mai.ru", "", "", roles),
	new EditForm(43,"Terry","Terry@mai.ru", "", "", roles),
	new EditForm(59,"Sharon","Sharon@mai.ru", "", "", roles),
];


//view models
function Page(){
	var self = this;
	self.SaveButton = new SaveButton(function(){
		page.SaveButton.IsSaving(true);
		setTimeout(function(){
			var isErrors = false;
			for(var i in page.FormsList()){
				var item = page.FormsList()[i];
				if(!item.ValidateForm()){
					isErrors = true;
				}
			}
		
			page.SaveButton.IsSaving(false);
			page.SaveButton.SetResult(isErrors == true ? "Error" : "Success");
		},2000);
	});
	self.FormsList = ko.observableArray(forms);
	
}

function EditForm(id, userName, email, password, passwordConfirm, roles){
	var self = this;
	self.Id = ko.observable(id);
	self.UserName = ko.observable(userName);
	self.Email = ko.observable(email);
	self.Password = ko.observable(password);
	self.PasswordConfirm = ko.observable(passwordConfirm);
	self.Roles = ko.observableArray(roles);
	self.AvaliableRoles = ko.observableArray(avaliableRoles);
	
	self.IsChangePassword = ko.observable(false);
	self.Errors = ko.observableArray([]);
	self.IsShowErrors = ko.observable(false);
	self.ValidateForm = function(){
		self.Errors([]);
		if(!validateEmail(self.Email())){
			self.Errors.push("Wrong email format");
		}
		if(!self.UserName()){
			self.Errors.push("User name required");
		}
		if(self.IsChangePassword()){
			if(!self.Password() || !self.PasswordConfirm()){
				self.Errors.push("Password and password confurm fields cannot be empty");
			}
			if(self.Password() != self.PasswordConfirm()){
				self.Errors.push("Password and confirmation not equals");
			}
		}
		var validateResult = self.Errors().length == 0;
		self.IsShowErrors(!validateResult);
		return validateResult;
	}
	
}



function SaveButton(clickFunction){
	var self = this;
	self.SavingTexts = {
		Saving: "Saving...",
		Save: "Save items"
	};
	self.SaveText = ko.observable(self.SavingTexts.Save);
	self._isSaving = ko.observable(false);
	self.IsSaving = ko.computed({
		read: function(){
			return self._isSaving();
		},
		write: function(value){
			self._isSaving(value);
			self.SaveText(value ? self.SavingTexts.Saving : self.SavingTexts.Save )
		}
	});
	self.IsSuccessVisible = ko.observable(false);
	self.IsErrorVisible = ko.observable(false);
	self.SetResult = function(result){
		if(result == "Success"){
			self.IsSuccessVisible(true);
		}
		if(result == "Error"){
			self.IsErrorVisible(true);
		}
		setTimeout(function(){
			self.IsSuccessVisible(false);
			self.IsErrorVisible(false);
		},500);
	}
	
	self.IsSaving(false);
	self.OnSave = clickFunction;
}


	

var page = new Page();
ko.applyBindings(page);