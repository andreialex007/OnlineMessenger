ko.bindingHandlers.fadeVisible = {
    init: function(element, valueAccessor) {
        var value = valueAccessor();
        $(element).toggle(ko.unwrap(value)); 
    },
    update: function(element, valueAccessor) {
        var value = valueAccessor();
        ko.unwrap(value) ? $(element).show("fast") : $(element).hide("fast");
    }
};

ko.bindingHandlers.dissmissSlowVisible = {
    init: function(element, valueAccessor) {
        var value = valueAccessor();
        $(element).toggle(ko.unwrap(value)); 
    },
    update: function(element, valueAccessor) {
        var value = valueAccessor();
        ko.unwrap(value) ? $(element).show() : $(element).fadeOut(1000);
    }
};



ko.bindingHandlers.selectAnswer = {
    update: function(element, valueAccessor) {
		$(element).click(function(){
			var value = valueAccessor();
			value($(element).text());
		});
    }
};

ko.bindingHandlers.datePicker = {
    init: function(element, valueAccessor) {
		console.log("datePicker");
		jq_1_32(element).DatePicker({
			flat: false,
			format:'d.m.Y',
			date: moment().format("D.MM.YY"),
			current: moment().format("D.MM.YY"),
			calendars: 1,
			starts: 1,
			position: 'right',
			onChange: function(formated, dates){
				$(element).prev().val(formated);
				$('.datepicker').hide();
				$(element).prev().trigger("change");
				valueAccessor()();
			},
		});
    }
};

ko.bindingHandlers.maskedInput = {
    init: function(element, valueAccessor) {
		$(element).mask(valueAccessor());
		console.log("maskedInput");
		console.log(valueAccessor());
    }
};

ko.bindingHandlers.addClass = {
	currentClass : "",
    update: function(element, valueAccessor) {
		var self = this;
        var value = valueAccessor();
		if(typeof value != "string")
			value = value();
		$(element).removeClass(self.currentClass);
			$(element).addClass(value);
		if(value != ''){
			self.currentClass = value;
		}
    }
};


ko.bindingHandlers.scrollToBottom = {
    update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        var unwrapped = ko.unwrap(value) ;
		value(false);
		$(element).scrollTop($(element)[0].scrollHeight);
    }
};

ko.bindingHandlers.fireIfCntlEnter = {
    update: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = valueAccessor();
        var unwrapped = ko.unwrap(value);
		$(element).keyup(function(event){
			if(event.ctrlKey && event.keyCode == 13){
				value();
			}
			return true;
		});
    }
};

ko.bindingHandlers.highLightColumn = {
    init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        $(element).click(function(){
			var el = element;
			var vl = valueAccessor;
			$(".scrolling-table td").removeClass("sorting-cell");
			$(".scrolling-table td:nth-child(" + ($(element).index() + 1) + ")").addClass("sorting-cell");
		});
    }
};




















