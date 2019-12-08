import cv2
import os
import numpy as np
import Cards
import ServicesProvider
from ConfigWindow import ConfigWindow
from SystemParameters import SystemParameters
from dotenv import load_dotenv
import _thread

font = cv2.FONT_HERSHEY_SIMPLEX

load_dotenv()
image_provider = ServicesProvider.get_image_provider()

path = os.path.dirname(os.path.abspath(__file__))
train_ranks = Cards.load_ranks(path + '/Card_Imgs/')

running = True

terminal_text = ""
params = SystemParameters()
configWindow = ConfigWindow(params)
print(_thread)
_thread.start_new_thread(configWindow.run, ())

while running:

    image = image_provider.get_image()
    pre_proc = Cards.preprocess_image(image, params)
    
    cnts = Cards.find_cards(pre_proc, params, image)
    cards = []
    # cv2.drawContours(image, cnts, -1, (255, 0, 0), 2)
    pre_proc_colored = cv2.cvtColor(pre_proc, cv2.COLOR_GRAY2BGR)
    cv2.drawContours(pre_proc_colored, cnts, -1, (255, 0, 0), 2)
    if len(cnts) != 0:

        for i in range(len(cnts)):
            cards.append(Cards.preprocess_card(cnts[i], image, params))
            cards[i].best_rank_match, cards[i].rank_diff = Cards.match_card(cards[i], train_ranks)
            image = Cards.draw_results(image, cards[i])

        boxes = [card.contour for card in cards]
        
        cv2.drawContours(pre_proc, boxes, -1, (255, 0, 0), 2)

    # draw horizontal line to make the difference between croupier's and player's cards more distinct
    img_h, img_w = np.shape(image)[:2]
    mid_h = int(img_h / 2)
    cv2.line(image, (0, mid_h), (img_w, mid_h), (255, 255, 255), 1)

    player_hand, croupier_hand = Cards.get_deal(cards, mid_h)
    players = Cards.get_cards_value(player_hand)
    croupier = Cards.get_cards_value(croupier_hand)
    cv2.putText(image,
                f"croupier {croupier}", (0 + 10, mid_h - 25),
                font,
                1,
                Cards.card_value_color(croupier),
                1,
                cv2.LINE_AA)
    cv2.putText(image,
                f"player {players}",
                (0 + 10, mid_h + 40),
                font,
                1,
                Cards.card_value_color(players),
                1,
                cv2.LINE_AA)

    key = cv2.waitKey(1) & 0xFF
    if key == ord("q"):
        running = False

    text_size = cv2.getTextSize(terminal_text, font, 1, 2)[0]
    text_x = (img_w - text_size[0]) - 20
    text_y = (img_h + text_size[1]) - 20
    cv2.putText(image, terminal_text, (text_x, 35), font, 1, (255, 255, 255), 1)

    cv2.imshow('Visualization', image)
    # cv2.imshow("IMAGE", pre_proc_colored)
cv2.destroyAllWindows()
