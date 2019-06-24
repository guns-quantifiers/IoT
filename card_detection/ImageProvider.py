from urllib.request import urlopen
import numpy as np
import cv2

CONNECTION_ERROR_MESSAGE = "Bad connection"
FONT = cv2.FONT_HERSHEY_SIMPLEX


class UrlImageProvider:

    def __init__(self, url):
        self.url = url

    def get_image(self):
        try:
            response = urlopen(self.url, timeout=5)
            img_np = np.array(bytearray(response.read()), dtype=np.uint8)
            return cv2.imdecode(img_np, -1)
        except:
            default_image = np.zeros((600, 1000, 3), np.uint8)
            text_size = cv2.getTextSize(CONNECTION_ERROR_MESSAGE, FONT, 1, 2)[0]
            text_x = (default_image.shape[1] - text_size[0]) // 2
            text_y = (default_image.shape[0] + text_size[1]) // 2
            cv2.putText(default_image, CONNECTION_ERROR_MESSAGE, (text_x, text_y - 20), FONT, 1, (255, 255, 255), 2)
            return default_image


class DefaultImageProvider:

    def __init__(self, img_path):
        self.img_path = img_path

    def get_image(self):
        return cv2.imread(self.img_path, cv2.IMREAD_COLOR)
