import ApiService


class Game:

    def __init__(self, api_service: ApiService):
        self.api_service = api_service
        self.game_token = api_service.start_game().response
        self.current_deal_token = api_service.add_deal(self.game_token).response

    def update_current_deal(self, player_hand, croupier_hand):
        api_response = self.api_service.update_deal(self.current_deal_token, player_hand, croupier_hand)
        if api_response.error is None:
            print(f"Successfully updated deal with id = {self.current_deal_token[0:3]}")
            print("Croupier hand: ", croupier_hand)
            print("Player hand: ", player_hand)
        else:
            print(api_response.error)

    def add_new_deal(self):
        api_response = self.api_service.add_deal(self.game_token)
        if api_response.error is None:
            self.current_deal_token = api_response.response
            print(f"Successfully added new deal with id = {self.current_deal_token[0:3]}")
        else:
            print(api_response.error)

    def end_current_deal(self):
        api_response = self.api_service.end_deal(self.current_deal_token)
        if api_response.error is None:
            print(f"Successfully ended deal with id = {self.current_deal_token[0:3]}")
            self.add_new_deal()
        else:
            print(api_response.error)

    def get_strategy_for_current_deal(self):
        api_response = self.api_service.get_strategy(self.current_deal_token)
        if api_response.error is None:
            print(f"Successfully acquired strategy: ", api_response.response)
            return api_response.response
        else:
            print(api_response.error)
            return None
