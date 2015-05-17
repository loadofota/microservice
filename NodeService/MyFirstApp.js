//var http = require('http');

var couchbase = require('couchbase');
var cluster = new couchbase.Cluster();

var bucket = cluster.openBucket('beer-sample');

bucket.get('new_holland_brewing_company-sundog', function(err, result) {
	console.log(result);
});
