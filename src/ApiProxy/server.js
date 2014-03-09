var http = require('http');
var https = require('https');
var url = require('url');

var port = process.env.port || 1337;

var apiUrls = {
	cryptsy: 'http://pubapi.cryptsy.com/api.php?method=singlemarketdata&marketid=',
	poloniex: 'https://poloniex.com/public?command=returnOrderBook&currencyPair=BTC_',
	cryptorush: 'https://cryptorush.in/api.php?get=market&b=btc&key=e6b1d736592f27ff10af41c67a724ac036daed3a&id=17928&m='
};

http.createServer(function (req, res) {
	var requestUrl = url.parse(req.url);
	var pathSegments = requestUrl.pathname.slice(1).split('/');
	var pathSegment = pathSegments.shift();

	if(apiUrls[pathSegment]) {
		handleRequest(req, res, pathSegments, apiUrls[pathSegment]);
	}
}).listen(port);

function handleRequest(req, res, pathSegments, targetUrlBase) {
	var pathSegment = pathSegments.shift();

	if(pathSegment === 'market') {
		var marketId = pathSegments.shift();
		var targetUrl = url.parse(targetUrlBase + marketId);
		console.log('GET', targetUrl);

		res.setHeader("Access-Control-Allow-Origin", "*");

		var requester = targetUrlBase.startsWith('https://') ? https : http;
		var requestOptions = {
			method: 'GET',
			hostname: targetUrl.hostname,
			port: targetUrl.port,
			path: targetUrl.path,
			rejectUnauthorized: false,
		};

		var targetRequest = requester.get(
			requestOptions, 
			function(targetResponse) {
				console.log('Response');
				res.statusCode = targetResponse.statusCode;
				targetResponse.on('data', function(chunk) {
					console.log('Data');
					res.write(chunk);
				})
				.on('end', function() {
					console.log('End');
					res.end();
				});
			})
			.end();
	}
}

if(!String.prototype.startsWith) {
	Object.defineProperty(String.prototype, 'startsWith', {
		enumerable: false,
		configurable: false,
		writable: false,
		value: function (searchString, position) {
			position = position || 0;
			return this.indexOf(searchString, position) === position;
		}
	});
};