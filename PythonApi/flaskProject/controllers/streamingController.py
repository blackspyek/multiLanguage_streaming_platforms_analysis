from flask import Blueprint, jsonify, request
from models import titleBasics, titleRatings, TitleJoined
from extensions import redis_client, session
from matchRating import get_average_rating_by_year, get_years_avg
from flask_jwt_extended import jwt_required, get_jwt
import requests
import os
streaming = Blueprint('streaming', __name__)


@streaming.route('/allPlatforms')
def get_platforms():
    platforms = get_platforms_func()
    if not platforms:
        return jsonify({'message': 'No data found'}), 404
    return jsonify(platforms), 200

def get_platforms_func():
    API_URL = os.getenv('API_URL', 'http://localhost:5192')
    #API_URL = os.getenv('API_URL', 'http://streamingapi:5192')
    try:
        response = requests.get(f'{API_URL}/api/platform')
        response.raise_for_status()
        data = response.json()
        return data
    except requests.exceptions.HTTPError as err:
        print(err)