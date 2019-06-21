import requests
import json

API_ADDRESS = 'https://26bdf97d-0d99-4bba-ab8e-924138a2f7be.mock.pstmn.io'


class ApiService:

    def __init__(self, base_url=API_ADDRESS):
        self.base_url = base_url

    def start_game(self):
        url = self.base_url + "/create/game"
        res = requests.post(url)
        data = json.loads(res.text)
        return data['gameToken']

    def add_deal(self, game_token):
        url = self.base_url + "/game/addDeal"
        res = requests.post(url, {'gameToken': game_token})
        data = json.loads(res.text)
        return data['dealToken']

    def update_deal(self, deal_token, player_hand, croupier_hand):
        url = self.base_url + "/deal/update"
        res = requests.post(url, {
            'dealToken': deal_token,
            'playerHand': player_hand,
            'croupierHand': croupier_hand
        })
        data = json.loads(res.text)
        print(data)

    def end_deal(self, deal_token):
        url = self.base_url + "/game/endDeal"
        res = requests.post(url, {'dealToken': deal_token})
        data = json.loads(res.text)
        print(data["Message"])

    def get_strategy(self, deal_token):
        url = self.base_url + "/deal/strategy?dealToken=" + deal_token
        res = requests.get(url)
        data = json.loads(res.text)
        print("Hint")

    def get_deal(self, deal_token):
        url = self.base_url + "/deal?dealToken=" + deal_token
        res = requests.get(url)
        data = json.loads(res.text)
        return data
