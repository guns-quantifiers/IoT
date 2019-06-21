import numpy as np
import cv2

RANKS = ['Ace', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven',
         'Eight', 'Nine', 'Ten', 'Jack', 'Queen', 'King']

# Adaptive threshold levels
BKG_THRESH = 60
CARD_THRESH = 30

# Width and height of card corner, where rank and suit are
CORNER_WIDTH = 32
CORNER_HEIGHT = 84

# Dimensions of rank train images
RANK_WIDTH = 70
RANK_HEIGHT = 125

RANK_DIFF_MAX = 2000

CARD_MAX_AREA = 120000
CARD_MIN_AREA = 25000

font = cv2.FONT_HERSHEY_SIMPLEX


class QueryCard:
    """Structure to store information about query cards in the camera image."""

    def __init__(self):
        self.contour = []
        self.width, self.height = 0, 0
        self.corner_pts = []
        self.center = []
        self.warp = []  # 200x300, flattened, grayed, blurred image
        self.rank_img = []  # Thresholded, sized image of card's rank
        self.best_rank_match = "Unknown"
        self.rank_diff = 0


class TrainRanks:
    """Structure to store information about train rank images."""

    def __init__(self):
        self.img = []  # Thresholded, sized rank image loaded from hard drive
        self.name = "Placeholder"


def load_ranks(filepath):
    """Loads rank images from directory specified by filepath. Stores
    them in a list of Train_ranks objects."""

    train_ranks = []
    i = 0

    for Rank in RANKS:
        train_ranks.append(TrainRanks())
        train_ranks[i].name = Rank
        filename = Rank + '.jpg'
        train_ranks[i].img = cv2.imread(filepath + filename, cv2.IMREAD_GRAYSCALE)
        i = i + 1

    return train_ranks


def preprocess_image(image):
    """Returns a grayed, blurred, and adaptively thresholded camera image."""

    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    blur = cv2.GaussianBlur(gray, (5, 5), 0)

    # The best threshold level depends on the ambient lighting conditions.
    # For bright lighting, a high threshold must be used to isolate the cards
    # from the background. For dim lighting, a low threshold must be used.
    # To make the card detector independent of lighting conditions, the
    # following adaptive threshold method is used.
    #
    # A background pixel in the center top of the image is sampled to determine
    # its intensity. The adaptive threshold is set at 50 (THRESH_ADDER) higher
    # than that. This allows the threshold to adapt to the lighting conditions.
    img_w, img_h = np.shape(image)[:2]
    bkg_level = gray[int(img_h / 100)][int(img_w / 2)]
    thresh_level = bkg_level + BKG_THRESH

    retval, thresh = cv2.threshold(blur, thresh_level, 255, cv2.THRESH_BINARY)

    return thresh


def find_cards(thresh_image):
    """Finds all card-sized contours in a thresholded camera image.
    Returns a list of card contours sorted
    from largest to smallest."""

    dummy, cnts, hier = cv2.findContours(thresh_image, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)

    if len(cnts) == 0:
        return [], []

    index_sort = sorted(range(len(cnts)), key=lambda i: cv2.contourArea(cnts[i]), reverse=True)

    cnts_sort = [cnts[i] for i in index_sort]
    hier_sort = [hier[0][i] for i in index_sort]

    return [contour for k, contour in enumerate(cnts_sort) if is_card(contour, hier_sort[k][3])]


def is_card(contour, hier):

    # Determine which of the contours are cards by applying the
    # following criteria:
    # 1) Have no parents,
    # 2) Smaller area than the maximum card size,
    # 3) Bigger area than the minimum card size,
    # 4) Have four corners

    if hier != -1:
        return False

    size = cv2.contourArea(contour)
    if (size > CARD_MAX_AREA) or (size < CARD_MIN_AREA):
        return False

    peri = cv2.arcLength(contour, True)
    approx = cv2.approxPolyDP(contour, 0.01 * peri, True)
    if len(approx) != 4:
        return False

    return True


def preprocess_card(contour, image):
    """Uses contour to find information about the query card.
       Isolates rank image from the card."""

    q_card = QueryCard()
    q_card.contour = contour

    # Find perimeter of card and use it to approximate corner points
    peri = cv2.arcLength(contour, True)
    approx = cv2.approxPolyDP(contour, 0.01 * peri, True)
    pts = np.float32(approx)
    q_card.corner_pts = pts

    # Find width and height of card's bounding rectangle
    x, y, w, h = cv2.boundingRect(contour)
    q_card.width, q_card.height = w, h

    # Find center point of card by taking x and y average of the four corners.
    average = np.sum(pts, axis=0) / len(pts)
    cent_x = int(average[0][0])
    cent_y = int(average[0][1])
    q_card.center = [cent_x, cent_y]

    # Warp card into 200x300 flattened image using perspective transform
    q_card.warp = flattener(image, pts, w, h)

    # Grab corner of warped card image and do a 4x zoom
    qcorner = q_card.warp[0:CORNER_HEIGHT, 0:CORNER_WIDTH]
    qcorner_zoom = cv2.resize(qcorner, (0, 0), fx=4, fy=4)

    # Sample known white pixel intensity to determine good threshold level
    white_level = qcorner_zoom[15, int((CORNER_WIDTH * 4) / 2)]
    thresh_level = white_level - CARD_THRESH
    if thresh_level <= 0:
        thresh_level = 1
    retval, query_thresh = cv2.threshold(qcorner_zoom, thresh_level, 255, cv2.THRESH_BINARY_INV)

    # Get rank
    qrank = query_thresh[20:185, 0:128]

    # Find rank contour and bounding rectangle, isolate and find largest contour
    dummy, qrank_cnts, hier = cv2.findContours(qrank, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    qrank_cnts = sorted(qrank_cnts, key=cv2.contourArea, reverse=True)

    # Find bounding rectangle for largest contour, use it to resize query rank
    # image to match dimensions of the train rank image
    if len(qrank_cnts) != 0:
        x1, y1, w1, h1 = cv2.boundingRect(qrank_cnts[0])
        qrank_roi = qrank[y1:y1 + h1, x1:x1 + w1]
        qrank_sized = cv2.resize(qrank_roi, (RANK_WIDTH, RANK_HEIGHT), 0, 0)
        q_card.rank_img = qrank_sized

    return q_card


def match_card(q_card, train_ranks):
    """Finds best rank and suit matches for the query card. Differences
    the query card rank and suit images with the train rank and suit images.
    The best match is the rank or suit image that has the least difference."""

    best_rank_match_diff = 10000
    best_rank_match_name = "Unknown"

    # If no contours were found in query card in preprocess_card function,
    # the img size is zero, so skip the differencing process
    # (card will be left as Unknown)
    if len(q_card.rank_img) != 0:

        # Difference the query card rank image from each of the train rank images,
        # and store the result with the least difference
        for Trank in train_ranks:

            diff_img = cv2.absdiff(q_card.rank_img, Trank.img)
            rank_diff = int(np.sum(diff_img) / 255)

            if rank_diff < best_rank_match_diff:
                best_rank_match_diff = rank_diff
                best_rank_name = Trank.name

    if best_rank_match_diff < RANK_DIFF_MAX:
        best_rank_match_name = best_rank_name

    # Return the identity of the card and the quality of the suit and rank match
    return best_rank_match_name, best_rank_match_diff


def draw_results(image, q_card):
    """Draw the card name, center point, and contour on the camera image."""

    x = q_card.center[0]
    y = q_card.center[1]
    cv2.circle(image, (x, y), 5, (255, 0, 0), -1)

    rank_name = q_card.best_rank_match

    # Draw card name twice, so letters have black outline
    cv2.putText(image, rank_name, (x - 40, y - 10), font, 1, (0, 0, 0), 3, cv2.LINE_AA)
    cv2.putText(image, rank_name, (x - 40, y - 10), font, 1, (50, 200, 200), 2, cv2.LINE_AA)

    return image


def flattener(image, pts, w, h):
    """Flattens an image of a card into a top-down 200x300 perspective.
    Returns the flattened, re-sized, grayed image.
    See www.pyimagesearch.com/2014/08/25/4-point-opencv-getperspective-transform-example/"""
    temp_rect = np.zeros((4, 2), dtype="float32")

    s = np.sum(pts, axis=2)

    tl = pts[np.argmin(s)]
    br = pts[np.argmax(s)]

    diff = np.diff(pts, axis=-1)
    tr = pts[np.argmin(diff)]
    bl = pts[np.argmax(diff)]

    # Need to create an array listing points in order of
    # [top left, top right, bottom right, bottom left]
    # before doing the perspective transform

    if w <= 0.8 * h:  # If card is vertically oriented
        temp_rect[0] = tl
        temp_rect[1] = tr
        temp_rect[2] = br
        temp_rect[3] = bl

    if w >= 1.2 * h:  # If card is horizontally oriented
        temp_rect[0] = bl
        temp_rect[1] = tl
        temp_rect[2] = tr
        temp_rect[3] = br

    # If the card is 'diamond' oriented, a different algorithm
    # has to be used to identify which point is top left, top right
    # bottom left, and bottom right.

    if 0.8 * h < w < 1.2 * h:  # If card is diamond oriented
        # If furthest left point is higher than furthest right point,
        # card is tilted to the left.
        if pts[1][0][1] <= pts[3][0][1]:
            # If card is titled to the left, approxPolyDP returns points
            # in this order: top right, top left, bottom left, bottom right
            temp_rect[0] = pts[1][0]  # Top left
            temp_rect[1] = pts[0][0]  # Top right
            temp_rect[2] = pts[3][0]  # Bottom right
            temp_rect[3] = pts[2][0]  # Bottom left

        # If furthest left point is lower than furthest right point,
        # card is tilted to the right
        if pts[1][0][1] > pts[3][0][1]:
            # If card is titled to the right, approxPolyDP returns points
            # in this order: top left, bottom left, bottom right, top right
            temp_rect[0] = pts[0][0]  # Top left
            temp_rect[1] = pts[3][0]  # Top right
            temp_rect[2] = pts[2][0]  # Bottom right
            temp_rect[3] = pts[1][0]  # Bottom left

    max_width = 200
    max_height = 300

    # Create destination array, calculate perspective transform matrix,
    # and warp card image
    dst = np.array([[0, 0], [max_width - 1, 0], [max_width - 1, max_height - 1], [0, max_height - 1]], np.float32)
    m = cv2.getPerspectiveTransform(temp_rect, dst)
    warp = cv2.warpPerspective(image, m, (max_width, max_height))
    warp = cv2.cvtColor(warp, cv2.COLOR_BGR2GRAY)

    return warp


def get_deal(cards, h_limit):
    player_hand = []
    croupier_hand = []

    for card in cards:
        if card.center[1] < h_limit:
            croupier_hand.append(card.best_rank_match)
        else:
            player_hand.append(card.best_rank_match)

    print(f"Player hand: {player_hand}")
    print(f"Croupier hand: {croupier_hand}")
    return player_hand, croupier_hand
