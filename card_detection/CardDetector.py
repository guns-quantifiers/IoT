import cv2
import os
import numpy as np
import Cards
import ServicesProvider
from Game import Game
from dotenv import load_dotenv


font = cv2.FONT_HERSHEY_SIMPLEX

load_dotenv()
api = ServicesProvider.get_api_service()
image_provider = ServicesProvider.get_image_provider()

path = os.path.dirname(os.path.abspath(__file__))
train_ranks = Cards.load_ranks(path + '/Card_Imgs/')

running = True

check = api.health_check()
game = None
if check.error is not None:
    running = False
    print(check.error)
else:
    game = Game(api)


while running:

    image = image_provider.get_image()

    # draw horizontal line to make the difference between croupier's and player's cards more distinct
    img_h, img_w = np.shape(image)[:2]
    mid_h = int(img_h / 2)
    cv2.line(image, (0, mid_h), (img_w, mid_h), (255, 228, 181), 5)

    pre_proc = Cards.preprocess_image(image)
    cnts = Cards.find_cards(pre_proc)
    cards = []

    if len(cnts) != 0:

        for i in range(len(cnts)):
            cards.append(Cards.preprocess_card(cnts[i], image))
            cards[i].best_rank_match, cards[i].rank_diff = Cards.match_card(cards[i], train_ranks)
            image = Cards.draw_results(image, cards[i])

        boxes = [card.contour for card in cards]
        cv2.drawContours(image, boxes, -1, (255, 0, 0), 2)

    cv2.imshow('Visualization', image)

    key = cv2.waitKey(1) & 0xFF
    if key == ord("q"):
        running = False
    elif key == ord("s"):
        game = Game(api)
    elif key == ord("n"):
        game.add_new_deal()
    elif key == ord("u"):
        player_hand, croupier_hand = Cards.get_deal(cards, mid_h)
        game.update_current_deal(player_hand, croupier_hand)
    elif key == ord("e"):
        player_hand, croupier_hand = Cards.get_deal(cards, mid_h)
        game.update_current_deal(player_hand, croupier_hand)
        game.end_current_deal()
    elif key == ord("h"):
        player_hand, croupier_hand = Cards.get_deal(cards, mid_h)
        game.update_current_deal(player_hand, croupier_hand)
        game.get_strategy_for_current_deal()

cv2.destroyAllWindows()
