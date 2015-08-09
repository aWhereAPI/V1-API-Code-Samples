<?php

// Add your consumer key and secret here
$consumer_key 		= "";
$consumer_secret 	= "";

// Constants
$host = "https://api.awhere.com";
$oauth_token = GetOAuthToken(); 

// Usage Samples
$queryStringUrl = BuildUrlWithExampleQueryString();
print "Sample GET Response (Celsius):\n";
print_r(GetWeatherJson($queryStringUrl));

$queryStringUrl = BuildUrlWithExampleQueryString(true);
print "Sample GET Response (Fahrenheit):\n";
print_r(GetWeatherJson($queryStringUrl));

// Sample GET request to the Weather API, using the PHP cURL extension
//
// Returns an array of WeatherResponse objects either in json or PHP object form
function GetWeatherJson($queryStringUrl, $json = true)
{
	global $oauth_token;

	$ch = curl_init($queryStringUrl);
	
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_HTTPHEADER, array(
												"Content-Type: application/json",
												"Accept: application/json",
												"Authorization: Bearer $oauth_token"
											));
	curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);
	curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
	
	return $json ? CurlExecute($ch) : json_decode(CurlExecute($ch));
}

// A utility function to encapsulate curl error handling
function CurlExecute($ch)
{
	// These next two lines disable the SSL authority checks in your cURL environment. 
	// Sometimes, development environments are not equipped with all the SSL Certificate Authority Chains
	// and cannot verify the authenticity of an SSL Cert. If you get SSL errors from cURL, 
	// You can uncomment the next two lines. However, when you go to production you should not use these flags. 

	//curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false); 
	//curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false); 

	$curlOutput = curl_exec($ch);
	if($curlOutput === false)
	{
		$curlOutput = "";
		die(curl_error($ch));
	}
	else
	{
		$info = curl_getinfo($ch);
		
		if($info['http_code'] != 200)
		{
			$curlOutput = "";
			die("HTTP Error: ".$info['http_code']." for ".$info['url']);
		}
	}	
	curl_close($ch);
	
	return $curlOutput;
}

// Builds a URL with an example query string for demonstration of GET request.
function BuildUrlWithExampleQueryString($useFahrenheit = false)
{	
	global $host; 
	
	$dateString = date("Y-m-d", time());
	$url = $host."/v1/weather"; 
	
	$url = "$url?latitude=30.25&longitude=-97.75&startDate=$dateString&attribute=maxTemperature&attribute=precip";
	if($useFahrenheit){
		$url .= "&temperatureUnits=fahrenheit";
	}
	
	return $url;
}

// Uses the Token API to retrieve an access token. 
// Note: the token will expire after an hour. This API returns an 'expires_in' property
//       with the number of seconds until it expires, but that is not captured in this example. 
//       API calls with an expired token also return 401 Unauthorized HTTP error. 
function GetOAuthToken(){ 
	global $host,$consumer_key,$consumer_secret;  
	
	$ch = curl_init($host."/oauth/token");
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
	curl_setopt($ch, CURLOPT_POSTFIELDS, "grant_type=client_credentials");
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_HTTPHEADER, array(
												"Content-Type: application/x-www-form-urlencoded",
												"Authorization: Basic ".base64_encode($consumer_key.":".$consumer_secret)
											));

	$result = CurlExecute($ch); 
	$result = json_decode($result);
	return $result->access_token;
}

?>
