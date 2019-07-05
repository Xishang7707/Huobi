var express = require('express');
var app = express();

app.use('/public', express.static('public'));

// app.get('/*', function (req, res) {
//     res.sendFile(__dirname + req.originalUrl);
// })

app.get('/index.html', function (req, res) {
    res.sendFile(__dirname + "/index.html");
})

app.get('/categories', function (req, res) {
    console.log(req.query)
    res.sendFile(__dirname + "/categories.html");
})

app.get('/search.html', function (req, res) {
    res.sendFile(__dirname + "/search.html");
})

app.get('/doctype.html', function (req, res) {
    res.sendFile(__dirname + "/doctype.html");
})

app.get('/register.html', function (req, res) {
    res.sendFile(__dirname + "/register.html");
})

app.get('/remember.html', function (req, res) {
    res.sendFile(__dirname + "/remember.html");
})

app.get('/request-otc.html', function (req, res) {
    res.sendFile(__dirname + "/request-otc.html");
})

app.get('/request-pro.html', function (req, res) {
    res.sendFile(__dirname + "/request-pro.html");
})

app.get('/zendesk.html', function (req, res) {
    res.sendFile(__dirname + "/zendesk.html");
})

app.get('/requests.html', function (req, res) {
    res.sendFile(__dirname + "/requests.html");
})

app.get('/sections.html', function (req, res) {
    res.sendFile(__dirname + "/sections.html");
})

app.get('/article.html', function (req, res) {
    res.sendFile(__dirname + "/article.html");
})

app.get('/req.html', function (req, res) {
    res.sendFile(__dirname + "/req.html");
})

app.get('/', function(req, res){
	res.sendFile(__dirname+"/index.html");
})

app.listen(3000);
