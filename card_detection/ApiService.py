import requests
import json

DEFAULT_HEADERS = {'Content-Type': "application/json", 'Accept': "application/json"}


class ApiResponse:

    def __init__(self, response=None, error=None):
        self.response = response
        self.error = error


class ApiService:

    def __init__(self, base_url):
        self.base_url = base_url

    def health_check(self):
        url = self.base_url + "/health"
        try:
            res = requests.get(url, timeout=4)
            return ApiResponse()
        except:
            return ApiResponse(error="Fatal error: failed to connect to api instance")

    def start_game(self):
        url = self.base_url + "/game/create"
        res = requests.post(url)

        if res.status_code == 200:
            data = json.loads(res.text)
            return ApiResponse(response=data['gameToken'])
        else:
            return ApiResponse(error=f"Error occurred while communication with api, code - {res.status_code}")

    def add_deal(self, game_token):
        url = self.base_url + "/game/addDeal"
        res = requests.post(url,
                            json={'gameToken': game_token},
                            headers=DEFAULT_HEADERS)

        if res.status_code == 200:
            data = json.loads(res.text)
            return ApiResponse(response=data['dealToken'])
        else:
            return ApiResponse(error=f"Error occurred while communication with api, code - {res.status_code}")

    def update_deal(self, deal_token, player_hand, croupier_hand):
        url = self.base_url + "/deal/update"
        res = requests.post(url,
                            json={
                                'dealToken': deal_token,
                                'playerHand': player_hand,
                                'croupierHand': croupier_hand
                            },
                            headers=DEFAULT_HEADERS)
        if res.status_code == 200:
            return ApiResponse()
        else:
            return ApiResponse(error=f"Error occurred while communication with api, code - {res.status_code}")

    def end_deal(self, deal_token):
        url = self.base_url + "/deal/end"
        res = requests.post(url,
                            json={'dealToken': deal_token},
                            headers=DEFAULT_HEADERS)

        if res.status_code == 200:
            data = json.loads(res.text)
            return ApiResponse(response=data["message"])
        else:
            return ApiResponse(error=f"Error occurred while communication with api, code - {res.status_code}")

    def get_strategy(self, deal_token):
        url = self.base_url + "/deal/strategy?dealToken=" + deal_token
        res = requests.get(url)

        if res.status_code == 200:
            data = json.loads(res.text)
            return ApiResponse(response=data)
        else:
            return ApiResponse(error=f"Error occurred while communication with api, code - {res.status_code}")
