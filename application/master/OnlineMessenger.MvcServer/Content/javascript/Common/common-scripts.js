//helper functions
var timeOut;
function delayExecute(func) {
    clearTimeout(timeOut);
    timeOut = setTimeout(function () {
        func();
    }, 100);
}

String.prototype.trim = function () { return this.replace(/^\s+|\s+$/g, ''); };

function validateEmail(email) {
    var re = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/;
    return re.test(email);
}

function cleanArray(arr) {
    return arr.filter(function (n) { return n; });
}

var pagePath = "";
function getCookie(name) {
    name = pagePath + name;
    return getGlobalCookie(name);
}

function getGlobalCookie(name) {
    var value = $.cookie(name);
    return !value ? "" : value;
}

function setGlobalCookie(name, value) {
    if (!value)
        value = "";
    $.cookie(name, value, { path: "/" });
}

function setCookie(name, value) {
    name = pagePath + name;
    setGlobalCookie(name, value);
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

var urlPrefix = "";
function postJSONToController(url, data, success) {
    var options = buildJSONOptions(url, data, success);
    $.ajax(options);
}

function postJSONToControllerExactUrl(url, data, success) {
    var options = buildJSONOptions(url, data, success);
    options.url = url;
    $.ajax(options);
}

function postJSONToControllerJSONP(url, data, success) {

    $.ajax(buildJSONPOptions(url, data, success));
}

function buildJSONPOptions(url, data, success) {
    var options = buildJSONOptions(url, data, success);
    options.xhrFields = {
        withCredentials: true
    };
    options.crossDomain = true;
    options.dataType = "jsonp";
    return options;
}

function buildJSONOptions(url, data, success) {
    var options = {
        type: 'POST',
        url: urlPrefix + getUrl(url),
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data),
        success: function (dt) {
            success(dt);
        },
        dataType: 'JSON'
    };
    return options;
}


function isScrolledIntoView(elem) {
    var docViewTop = $(window).scrollTop();
    var docViewBottom = docViewTop + $(window).height();

    var elemTop = $(elem).offset().top;
    var elemBottom = elemTop + $(elem).height();

    return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
}

var getJSON = function (url, file, successHandler) {
    var xhr = typeof XMLHttpRequest != 'undefined'
      ? new XMLHttpRequest()
      : new ActiveXObject('Microsoft.XMLHTTP');
    var fd = new FormData();
    fd.append("file", file);
    xhr.open('POST', url, true);
    xhr.onreadystatechange = function () {
        var status;
        var data;
        if (xhr.readyState == 4) { // `DONE`
            status = xhr.status;
            if (status == 200) {
                data = JSON.parse(xhr.responseText);
                successHandler && successHandler(data);
            }
        }
    };
    xhr.send(fd);
};