from flask import Blueprint, jsonify, request
from models import RottenTomatoesMovies, titleBasics
from schemas import RottenTomatoesMoviesSchema, titleBasicsSchema
from extensions import db, redis_client
from flask_jwt_extended import jwt_required, get_jwt

import json
rotten_tomatoes_movies = Blueprint('rotten_tomatoes_movies', __name__)

def get_years_func():
    data = redis_client.get("rotten_tomatoes_movies_years")
    if not data:
        result = db.session.query(
            db.func.year(RottenTomatoesMovies.original_release_date).label("release_year"),
            db.func.avg(RottenTomatoesMovies.tomatometer_rating).label("avg_tomatometer_rating"),
            db.func.avg(RottenTomatoesMovies.audience_rating).label("avg_audience_rating"),
        ).group_by("release_year").all()
        data = []
        for row in result:
            if row.release_year is None:
                continue
            data.append({
                "release_year": row.release_year,
                "avg_tomatometer_rating": float(row.avg_tomatometer_rating/10),
                "avg_audience_rating": float(row.avg_audience_rating/10)
            })
        data.sort(key=lambda x: x['release_year'])
        data_str = json.dumps(data)
        redis_client.set("rotten_tomatoes_movies_years", data_str)
    else:
        print("Data from cache")
        data = json.loads(data.decode('utf-8'))
    return data


@rotten_tomatoes_movies.route('/all')
@jwt_required()
def get_rotten_tomatoes_movies():
    claims = get_jwt()
    if claims['role'] != 'admin':
        return jsonify({'message': 'You are not authorized to access this resource'}), 403

    try:
        data = get_years_func()
        return jsonify(data), 200
    except Exception as e:
        return jsonify({'message': 'An error occurred while processing your request', 'error': str(e)}), 500

