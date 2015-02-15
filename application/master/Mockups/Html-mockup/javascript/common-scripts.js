//helper functions
var timeOut;
function delayExecute(func){
	clearTimeout(timeOut);
	timeOut = setTimeout(function(){
		func();
	},300);
}

String.prototype.trim=function(){return this.replace(/^\s+|\s+$/g, '');};

function validateEmail(email) { 
    var re = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;
    return re.test(email);
} 