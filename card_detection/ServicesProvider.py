import os
from ApiService import ApiService
from ImageProvider import UrlImageProvider

ENV_API_KEY = 'API_ADDRESS'
ENV_CAMERA_KEY = 'CAMERA_URL'


def get_image_provider():
    url = os.getenv(ENV_CAMERA_KEY)
    return UrlImageProvider(url)


def get_api_service():
    url = os.getenv(ENV_API_KEY)
    return ApiService(url)
