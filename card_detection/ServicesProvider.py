import os
from ImageProvider import UrlImageProvider

ENV_CAMERA_KEY = 'CAMERA_URL'


def get_image_provider():
    url = os.getenv(ENV_CAMERA_KEY)
    return UrlImageProvider(url)