angular
.module('silentArbiterApp', [])
.service('cryptsyService', [
	'$rootScope',
	'$http',
	function($rootScope, $http) {

	}
])
.service('cryptsyService', [
	'$rootScope',
	'$http',
	function($rootScope, $http) {
		// DRK = 155
		// UTC = 163

		var cryptsyMarketDataApiUrl = 'http://localhost:1337/cryptsy/market/163';
		var that = this;

		this.prices = {
			buyPrice: null,
			sellPrice: null,
		};

		this.updatePrices = function() {
			console.log('updating Cryptsy prices');
			$http
				.get(cryptsyMarketDataApiUrl)
				.then(function(result) {
					that.prices.buyPrice = result.data.return.markets.DRK.buyorders[0].price;
					that.prices.sellPrice = result.data.return.markets.DRK.sellorders[0].price;
					$rootScope.$broadcast('cryptsy.update');
				});
		}
	}
])
.service('poloniexService', [
	'$rootScope',
	'$http',
	function($rootScope, $http) {
		// DRK
		// UTC

		var apiUrl = 'http://localhost:1337/poloniex/market/UTC';
		var that = this;

		this.prices = {
			buyPrice: null,
			sellPrice: null,
		};

		this.updatePrices = function() {
			console.log('updating Poloniex prices');
			$http
				.get(apiUrl)
				.then(function(result) {
					that.prices.buyPrice = result.data.asks[0][0];
					that.prices.sellPrice = result.data.bids[0][0];
					$rootScope.$broadcast('poloniex.update');
				});
		}
	}
])
.service('cryptoRushService', [
	'$rootScope',
	'$http',
	function($rootScope, $http) {
		// DRK
		// UTC

		var apiUrl = 'http://localhost:1337/cryptorush/market/UTC';
		var that = this;

		this.prices = {
			buyPrice: null,
			sellPrice: null,
		};

		this.updatePrices = function() {
			console.log('updating Crypto Rush prices');
			$http
				.get(apiUrl)
				.then(function(result) {
					that.prices.buyPrice = result.data.asks[0][0];
					that.prices.sellPrice = result.data.bids[0][0];
					$rootScope.$broadcast('poloniex.update');
				});
		}
	}
])
.controller('priceDeltaList', [
	'$scope',
	'$timeout',
	'cryptsyService',
	'poloniexService',
	function($scope, $timeout, cryptsyService, poloniexService) {
		$scope.$on('cryptsy.update', function(event) {
			console.log('cryptsy change detected');
			$scope.cryptsy = cryptsyService.prices;
		});

		$scope.$on('poloniex.update', function(event) {
			console.log('poloniex change detected');
			$scope.poloniex = poloniexService.prices;
		});

		function updateAllPrices() {
			console.log('updating all prices');
			cryptsyService.updatePrices();
			poloniexService.updatePrices();
		}

		$timeout(updateAllPrices, 60000);
		updateAllPrices();
	}
]);
