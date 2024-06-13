from flask import Blueprint, jsonify, request
from graphHelpers.bestRated import get_bestRated
from flask_jwt_extended import jwt_required, get_jwt
from extensions import redis_client
import json
bestRated = Blueprint('bestRated', __name__)


@bestRated.route('/best')
@jwt_required()
def get_best_rated():
    claims = get_jwt()
    if claims['role'] != 'admin':
        return jsonify({'message': 'You are not authorized to access this resource'}), 403
    params = request.args
    _type = params.get('type', 'movie')
    CACHING_KEY = f"top_10_movies_{_type}"
    cached_data = redis_client.get(CACHING_KEY)
    if cached_data:
        cached_data = cached_data.decode('utf-8')
        return jsonify(json.loads(cached_data)), 200
    del cached_data
    data = get_bestRated(_type)
    movie_stats = []

    for movie_name, stats in data.items():
        try:
            avg_rating = stats["average_rating"]
            votes_count = stats["votes_count"]
            title_type = stats["title_type"]
            if votes_count < 1000 and title_type != "movie":
                continue
            movie_stats.append({
                'movie_name': movie_name,
                'average_rating': avg_rating,
                'votes_count': votes_count
            })
        except KeyError:
            print(f"Warning: Missing data for title '{movie_name}'. Skipping...")


    if not movie_stats:
        return jsonify({'message': 'No movie data available'}), 200

    
    movie_stats.sort(key=lambda x: x['average_rating'], reverse=True)

    top_10_movies = movie_stats[:10]

    redis_client.set(CACHING_KEY, json.dumps(top_10_movies))

    return jsonify(top_10_movies), 200
