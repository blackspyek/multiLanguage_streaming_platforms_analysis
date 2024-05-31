import os

from flask import Blueprint, jsonify, request
from models import TitleJoined
from extensions import redis_client, session
import requests
from flask_jwt_extended import jwt_required, get_jwt
from sqlalchemy import or_
_map = Blueprint('_map', __name__)


def get_year_with_titles_func():
    API_URL = os.getenv('API_URL', 'http://localhost:5192')
    try:
        response = requests.get(f'{API_URL}/api/country/all/movies')
        response.raise_for_status()
        data = response.json()
        return data
    except requests.exceptions.HTTPError as err:
        print(err)

@_map.route('/all')
@jwt_required()
def get_year_with_titles():
    claims = get_jwt()
    if claims['role'] != 'admin':
        return jsonify({'message': 'You are not authorized to access this resource'}), 403

    data = get_year_with_titles_func()
    if not data:
        return jsonify({'message': 'No data found'}), 404
    country_avg_rating = {}
    titles_that_dont_exist = []
    for country, titles in data.items():
        total_rating = 0
        count = 0
        for title in titles:
            title_variations = [
                title['titleName'],
                title['titleName'].replace(" ", "-"),
                title['titleName'].replace("-", " "),
                title['titleName'].replace(",", ":"),

            ]
            query = session.query(TitleJoined).filter(
                TitleJoined.start_year.in_([title['release_Year'] - 1, title['release_Year'],title['release_Year'] + 1]),
                or_(
                    (TitleJoined.primaryTitle.in_(title_variations)),
                    (TitleJoined.originalTitle.in_(title_variations))
                )
            ).first()
            if query:
                total_rating += query.average_rating
                count += 1
            else:
                titles_that_dont_exist.append("Title: " + title['titleName'] + " Year: " + str(title['release_Year']))
        if count > 0:
            country_avg_rating[country] = total_rating / count
    country_avg_rating['titles_that_dont_exist'] = titles_that_dont_exist
    return jsonify(country_avg_rating), 200





