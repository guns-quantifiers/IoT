from urllib.request import urlopen
import numpy as np
import cv2


class UrlImageProvider:

    def __init__(self, url):
        self.url = url

    def get_image(self):
        response = urlopen(self.url)
        img_np = np.array(bytearray(response.read()), dtype=np.uint8)
        return cv2.imdecode(img_np, -1)


class DefaultImageProvider:

    def __init__(self, img_path):
        self.img_path = img_path

    def get_image(self):
        return cv2.imread(self.img_path, cv2.IMREAD_COLOR)
