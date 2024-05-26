from flask import Blueprint, jsonify, request
from models import RottenTomatoesMovies
from schemas import RottenTomatoesMoviesSchema
from flask_jwt_extended import jwt_required, get_jwt

rotten_tomatoes_movies = Blueprint('rotten_tomatoes_movies', __name__)


@rotten_tomatoes_movies.route('/all')
# @jwt_required()  # Ensure the endpoint is protected by JWT
def get_rotten_tomatoes_movies():
    # Extract the claims from the JWT token
    # claims = get_jwt()
    # Check if the role in the claims is not 'admin'
    # if claims['role'] != 'admin':
    #     return jsonify({'message': 'You are not authorized to access this resource'}), 403

    # Get pagination parameters from the request
    page = request.args.get('page', default=1, type=int)
    per_page = request.args.get('per_page', default=10, type=int)

    # Paginate the rotten_tomatoes_movies entries
    rotten_tomatoes_movies_pagination = RottenTomatoesMovies.query.paginate(
        per_page=per_page,
        page=page,
        error_out=True
    )

    # Get the items from the pagination object
    rotten_tomatoes_movies_entries = rotten_tomatoes_movies_pagination.items

    # Serialize the rotten_tomatoes_movies entries using RottenTomatoesMoviesSchema
    result = RottenTomatoesMoviesSchema().dump(rotten_tomatoes_movies_entries, many=True)

    # Prepare the response with pagination metadata
    response = {
        "rotten_tomatoes_movies": result,
        "total": rotten_tomatoes_movies_pagination.total,
        "page": rotten_tomatoes_movies_pagination.page,
        "per_page": rotten_tomatoes_movies_pagination.per_page,
        "pages": rotten_tomatoes_movies_pagination.pages
    }

    # Return the response as a JSON object
    return jsonify(response), 200