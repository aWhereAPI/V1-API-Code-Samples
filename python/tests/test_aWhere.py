from unittest import TestCase
import tempfile
from datetime import datetime

import mock

from aWhere import Weather


class WeatherTest(TestCase):

    def setUp(self):
        self.client = Weather(authorize=False)

    def test_reading_credential_file(self):
        with tempfile.NamedTemporaryFile() as temp:
            temp.write(b'[Weather]\nconsumer_key = ABCDEFG\nconsumer_secret = 123456\n')
            temp.flush()

            keys = self.client._read_credential(temp.name)

            self.assertEqual(keys['consumer_key'], 'ABCDEFG')
            self.assertEqual(keys['consumer_secret'], '123456')

    def test_key_and_secret_are_hashed(self):
        keys = {'consumer_key': 'ABCDEFG',
                'consumer_secret': '123456'}

        keyhash = self.client._hash_credential(keys)

        self.assertEqual(keyhash, 'QUJDREVGRzoxMjM0NTY=')

    @mock.patch('aWhere.requests.post')
    def test_get_token(self, mock_post):

        mock_response = mock.Mock()
        expected_response = {'access_token': 'xxxx', 'expires_in': 3599}
        mock_response.json.return_value = expected_response
        mock_post.return_value = mock_response

        keys = {'consumer_key': 'ABCDEFG',
                'consumer_secret': '123456'}
        response = self.client._get_token(keys)

        #mock_post.assert_called_once_with('https://api.awhere.com/oauth/token')
        #mock_response.json.assert_called_once()
        self.assertEqual(response, expected_response)

    @mock.patch('aWhere.Weather._get_token')
    def test_authorize(self, mock_get_token):

        mock_token = {'access_token': 'xxxx', 'expires_in': 3599}
        mock_get_token.return_value = mock_token

        self.client.authorize()

        expected_authorization = {'Authorization': 'Bearer {}'.format(mock_token['access_token'])}
        self.assertEqual(self.client.authorization, expected_authorization)

    def test_check_options_with_valid_arguments(self):
        validArgs = {'attribute': [],
                     'endDate': None,
                     'plantDate': None,
                     'temperatureUnits': None,
                     'gddMethod': None,
                     'baseTemp': None,
                     'maxTempCap': None,
                     'minTempCap': None}
        try:
            self.client._check_options(**validArgs)
        except ValueError:
            self.fail("Valid argument failed check.")

    def test_check_options_with_invalid_arguments(self):
        invalidArgs = {'invalid': 'argument'}
        self.assertRaises(ValueError, self.client._check_options, **invalidArgs)

    def test_check_options_with_invalid_attributes(self):
        invalidAttributes = {'attribute': ['invalid']}
        self.assertRaises(ValueError, self.client._check_options, **invalidAttributes)

    def test_reformat_data(self):
        rawdata = [{'dailyAttributes': {'precip': 0.0},'latitude': 0,
                    'date': '1999-12-31T00:00:00', 'longitude': 0, 'requestId': 0}]
        date = datetime.strptime('1999-12-31T00:00:00', '%Y-%m-%dT%H:%M:%S')
        expected_data = {date: {'precip': 0.0}}

        returned_data = self.client._reformat_data(rawdata)

        self.assertEqual(expected_data, returned_data)

    def test_reformat_corrupted_data(self):
        rawdata = [{'invalid': 'data'}]
        self.assertRaises(KeyError, self.client._reformat_data, rawdata)
