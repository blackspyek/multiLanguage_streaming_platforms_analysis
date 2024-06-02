from flask import Blueprint, jsonify, request
from graphHelpers.directors import get_directors
from flask_jwt_extended import jwt_required, get_jwt
from extensions import redis_client
import json
directors = Blueprint('directors', __name__)


@directors.route('/all')
@jwt_required()
def get_best_directors():
    claims = get_jwt()
    if claims['role'] != 'admin':
        return jsonify({'message': 'You are not authorized to access this resource'}), 403
    params = request.args
    _type = params.get('type', 'movie')
    CACHING_KEY = f"top_5_directors_{_type}"
    cached_data = redis_client.get(CACHING_KEY)
    if cached_data:
        cached_data = cached_data.decode('utf-8')
        return jsonify(json.loads(cached_data)), 200
    del cached_data
    data = get_directors(_type)
    director_stats = []

    for director_name, stats in data.items():
        try:
            avg_rating = stats["average_rating"]
            movie_count = stats["movies_count"]
            director_stats.append({
                'director_name': director_name,
                'average_rating': avg_rating,
                'movies_count': movie_count
            })
        except KeyError:
            print(f"Warning: Missing data for director '{director_name}'. Skipping...")

    # Calculate the mean rating and the 75th percentile of movie counts
    if not director_stats:
        return jsonify({'message': 'No director data available'}), 200

    mean_rating = sum(d['average_rating'] for d in director_stats) / len(director_stats)
    sorted_movie_counts = sorted(d['movies_count'] for d in director_stats)
    percentile_75_index = int(0.75 * len(sorted_movie_counts))
    m = sorted_movie_counts[percentile_75_index]

    weight_factor = 10
    # Calculate weighted averages
    for director in director_stats:
        v = director['movies_count']
        R = director['average_rating']
        director['weighted_average'] = (R * v + mean_rating * m * weight_factor) / (v + m * weight_factor)

    # Sort by weighted average in descending order
    director_stats.sort(key=lambda x: x['weighted_average'], reverse=True)

    # Select top 5 directors
    top_5_directors = director_stats[:5]

    redis_client.set(CACHING_KEY, json.dumps(top_5_directors))

    return jsonify(top_5_directors), 200
