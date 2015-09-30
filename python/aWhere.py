"""Provides Weather and WeatherStation, aWhere API client classes."""

import configparser
from os import path
from base64 import b64encode
from datetime import datetime, timedelta

import requests

__author__ = "John Cobb"

class Weather(object):

    def __init__(self, authorize=True):
        self.authorization = None
        self.expiration = None
        if authorize: self.authorize()
        self.data = None

    def _read_credential(self, keyfile):
        """Read private API keys from a configuration file.

        The configuration file should be in the following format:
            [Weather]
            consumer_key = ****
            consumer_secret = ****

        Args:
            keyfile (str): the path to a configuration file.
        Returns:
            a dict containing API keys 'consumer_key' and 'consumer_secret'.
        Raise:
            KeyError: if the configuration file does not contain API keys.
        """
        config = configparser.ConfigParser()
        config.read(keyfile)
        keys = dict(config.items('Weather'))
        if 'consumer_key' and 'consumer_secret' in keys:
            return keys
        else:
            raise KeyError('API keys not found in configuration file: {}'.format(keyfile))

    @staticmethod
    def _hash_credential(keys):
        """Return the Base64-encoded "{key}:{secret}" combination.

        Args:
            keys (dict): 'consumer_key' and 'consumer_secret' API keys.
        Returns:
            encoded key combination.
        """
        combination = '{consumer_key}:{consumer_secret}'.format(**keys).encode()
        return b64encode(combination).decode('utf8')

    def _get_token(self, keys):
        """Request an API access token.

        Args:
            keys (dict): 'consumer_key' and 'consumer_secret' API keys.
        Returns:
            a dict containing 'access_token' and 'expires_in'.
        Raise:
            ValueError: if API call does not return a token.
        """
        credential = self._hash_credential(keys)
        response = requests.post('https://api.awhere.com/oauth/token',
                                 data='grant_type=client_credentials',
                                 headers={'Content-Type': 'application/x-www-form-urlencoded',
                                          'Authorization': 'Basic {}'.format(credential)}).json()
        if 'access_token' and 'expires_in' in response.keys():
            return response
        else:
            raise ValueError(response)

    def authorize(self, key=None, secret=None):
        """Authenticate API usage using 'key' and 'secret' or the default .keys configuration file.

        Args (optional):
            key (str):      'Consumer Key' from http://developer.awhere.com/user/me/apps
            secret (str):   'Consumer Secret' from http://developer.awhere.com/user/me/apps
        """
        if key and secret:
            keys = {'consumer_key': key,
                    'consumer_secret': secret}
        else:
            keys = self._read_credential(path.join(path.dirname(__file__), '.keys'))
        token = self._get_token(keys)
        self.authorization = {'Authorization': 'Bearer {}'.format(token['access_token'])}
        self.expiration = datetime.now() + timedelta(seconds=token['expires_in'])

    @staticmethod
    def _check_options(**kwargs):
        """Check keyword arguments against API optional parameters and requestable weather attributes.

        Args:
            kwargs (dict): the keyword arguments passed to the request method.
        Return:
            kwargs (dict): a validated set of keyword arguments.
        Raises:
            ValueError: if a keyword is an invalid optional parameter.
            TypeError: if the 'attribute' parameter is not a list.
            ValueError: if the 'attribute' parameter contains an invalid requestable weather attribute.
        """
        for option in kwargs.keys():
            if option not in ['attribute', 'endDate', 'plantDate', 'temperatureUnits',
                              'gddMethod', 'baseTemp', 'maxTempCap', 'minTempCap']:
                raise ValueError('"{}" is not a valid optional parameter'.format(option))
            if option == 'attribute':
                if type(kwargs['attribute']) != list:
                    raise TypeError("attribute parameter must be passed as a list")
                valid_attributes = ['minTemperature', 'maxTemperature', 'precip', 'accPrecip', 'accPrecipPriorYear',
                                    'accPrecip3YearAverage', 'accPrecipLongTermAverage', 'solar', 'minHumidity',
                                    'maxHumidity', 'mornWind', 'maxWind', 'gdd', 'accGdd', 'accGddPriorYear',
                                    'accGdd3YearAverage', 'accGddLongTermAverage', 'pet', 'accPet', 'ppet']
                for attribute in kwargs['attribute']:
                    if attribute not in valid_attributes:
                        raise ValueError('"{}" is not a valid requestable weather attribute'.format(attribute))
        return kwargs

    def request(self, latitude, longitude, startDate=None, **kwargs):
        """Query the aWhere Weather API.

        Args:
            latitude:   decimal-formatted number
            longitude:  decimal-formatted number
            startDate (optional): ISO-8601 formatted date, defaults to today.
            kwargs:     for valid keyword arguments see API documentation
                        http://developer.awhere.com/api/reference/observations-agronomics
        Return:
            True if successful.
        """
        if not startDate:
            startDate = datetime.today().strftime('%Y-%m-%d')
        params = {'latitude': latitude,
                  'longitude': longitude,
                  'startDate': startDate}
        params.update(self._check_options(**kwargs))
        url = 'https://api.awhere.com/v1/weather'
        response = requests.get(url, params, headers=self.authorization)
        return self._handle_response(response)

    def _handle_response(self, response):
        """Handle request response.

        Args:
            response: a requests.response object returned from API request.
        Returns:
            True if successful.
        Raises:
            ValueError: if unsuccessful.
        """
        if response.status_code == 200:
            self.data = self._reformat_data(response.json())
            return True
        else:
            raise ValueError(response.json())

    def _reformat_data(self, rawdata):
        """Reformat original data into a date keyed dictionary of daily weather attributes.

        Args:
            rawdata:    json-formatted response from API
        Returns:
            A date keyed dictionary of daily weather attributes.
        Raises:
            KeyError: if rawdata does not contain 'date' and 'dailyAttributes' values.
        """
        data = {}
        try:
            for record in rawdata:
                date = datetime.strptime(record['date'], '%Y-%m-%dT%H:%M:%S')
                data[date] = record['dailyAttributes']
        except KeyError:
            raise KeyError("original data dictionary is not properly formatted")
        return data


class WeatherStation(Weather):

    def __init__(self, name, latitude, longitude):
        super().__init__()
        self.name = name
        self.latitude = latitude
        self.longitude = longitude

    def request(self, startDate=None, **kwargs):
        super().request(self.latitude, self.longitude, startDate, **kwargs)


