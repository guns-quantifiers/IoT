import cv2
import os
import Cards
import numpy as np
from ApiService import ApiService
from Game import Game
from ImageProvider import UrlImageProvider, DefaultImageProvider

image_provider = None
url = 'http://192.168.1.207:8080/shot.jpg'
image_provider = UrlImageProvider(url)
# image_provider = DefaultImageProvider('1.jpg')


font = cv2.FONT_HERSHEY_SIMPLEX

path = os.path.dirname(os.path.abspath(__file__))
train_ranks = Cards.load_ranks(path + '/Card_Imgs/')

running = True

api = ApiService()
game = Game(api)

while running:

    image = image_provider.get_image()
    pre_proc = Cards.preprocess_image(image)
    cnts = Cards.find_cards(pre_proc)

    cards = []
    if len(cnts) != 0:

        # Initialize a new "cards" list to assign the card objects.
        # k indexes the newly made array of cards.

        k = 0

        for i in range(len(cnts)):
            # Create a card object from the contour and append it to the list of cards.
            # preprocess_card function takes the card contour and contour and
            # determines the cards properties (corner points, etc). It generates a
            # flattened 200x300 image of the card, and isolates the card's
            # suit and rank from the image.
            cards.append(Cards.preprocess_card(cnts[i], image))

            # Find the best rank and suit match for the card.
            cards[k].best_rank_match, cards[k].rank_diff = Cards.match_card(cards[k], train_ranks)

            # Draw center point and match result on the image.
            image = Cards.draw_results(image, cards[k])
            k = k + 1

        if len(cards) != 0:
            boxes = [card.contour for card in cards]
            cv2.drawContours(image, boxes, -1, (255, 0, 0), 2)

    img_h, img_w = np.shape(image)[:2]
    mid_h = int(img_h / 2)

    cv2.line(image, (0, mid_h), (img_w, mid_h), (255, 228, 181), 5)
    cv2.imshow('Visualization', image)

    key = cv2.waitKey(1) & 0xFF
    if key == ord("q"):
        running = False
    elif key == ord("s"):
        game = Game(api)
    elif key == ord("u"):
        player_hand, croupier_hand = Cards.get_deal(cards, mid_h)
        game.update_current_deal(player_hand, croupier_hand)
    elif key == ord("e"):
        game.end_current_deal()
    elif key == ord("h"):
        game.get_strategy_for_current_deal()

cv2.destroyAllWindows()
