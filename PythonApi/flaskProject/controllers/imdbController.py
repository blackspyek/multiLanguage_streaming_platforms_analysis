from flask import Blueprint, jsonify, request
from models import titleBasics, titleRatings, TitleJoined
from extensions import redis_client, session
from graphHelpers.matchRating import get_average_rating_by_year, get_years_avg
from flask_jwt_extended import jwt_required, get_jwt
imdb = Blueprint('imdb', __name__)
def get_years_func():
    years = redis_client.smembers("years")
    if not years:
        return jsonify({'message': 'No years found'}), 404
    years = [year.decode('utf-8') for year in years]
    return sorted(years)
@imdb.route('/all')
@jwt_required()
def get_movies_ratings_by_year():
    claims = get_jwt()
    if claims['role'] != 'admin':
        return jsonify({'message': 'You are not authorized to access this resource'}), 403
    data = get_years_avg()
    return jsonify(data), 200

@imdb.route('/years')
def get_years():
    return jsonify(get_years_func()), 200


@imdb.route('test')
def test():
    first_titleJoined = session.query(TitleJoined).first()
    title_data = {
        'tconst': first_titleJoined.tconst,
        'primaryTitle': first_titleJoined.primaryTitle,
        'is_adult': first_titleJoined.is_adult,
        'start_year': first_titleJoined.start_year,
        'average_rating': first_titleJoined.average_rating,
        'num_votes': first_titleJoined.num_votes
    }

    return jsonify(title_data), 200


