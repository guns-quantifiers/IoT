import ApiService


class Game:

    def __init__(self, api_service: ApiService):
        self.api_service = api_service
        self.game_token = api_service.start_game()
        self.current_deal_token = api_service.add_deal(self.game_token)

    def update_current_deal(self, player_hand, croupier_hand):
        self.api_service.update_deal(self.current_deal_token, player_hand, croupier_hand)

    def end_current_deal(self):
        self.api_service.end_deal(self.current_deal_token)
        self.current_deal_token = self.api_service.add_deal(self.game_token)

    def get_strategy_for_current_deal(self):
        return self.api_service.get_strategy(self.current_deal_token)
