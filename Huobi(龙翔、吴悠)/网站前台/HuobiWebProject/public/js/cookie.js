// 设置Cookie
function setCookie(c_name, value, expiredays) {
    var exdate = new Date()
    exdate.setDate(exdate.getDate() + expiredays)
    document.cookie = c_name + "=" + escape(value) +
        ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString())
}

// 获取cookie
function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

// 删除cookie
function delCookie() {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie("token");
    if (cval != null)
        document.cookie = "token" + "=" + encodeURIComponent(cval) + ";expires=" + exp.toGMTString();
}