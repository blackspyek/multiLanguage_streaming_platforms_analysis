import json
import os

from flask import Blueprint, jsonify, request, make_response
from models import TitleJoined, RottenTomatoesMovies
from extensions import redis_client, session
import requests
from flask_jwt_extended import jwt_required, get_jwt
from sqlalchemy import or_, extract
pagination = Blueprint('pagination', __name__)

SORT_BY = ['over_all', 'imdb_rating', 'tomatometer', 'audience_rating']
def get_titles():
    API_URL = os.getenv('API_URL', 'http://localhost:5192')
    params = {}
    if redis_client.get('titles') is not None:
        titles = redis_client.get('titles')
        titles = titles.decode('utf-8')
        return json.loads(titles)
    try:
        response = requests.get(f'{API_URL}/api/titles', params=params)
        response.raise_for_status()
        data = response.json()
        redis_client.set('titles', json.dumps(data))
        return data
    except requests.exceptions.HTTPError as err:
        print(err)
@pagination.route('/categories')
@jwt_required()
def get_Categories():
    if redis_client.get('categories') is not None:
        categories = redis_client.get('categories')
        categories = categories.decode('utf-8')
        return jsonify(json.loads(categories)), 200
    try:
        response = requests.get(f'http://localhost:5192/api/category')
        response.raise_for_status()
        data = response.json()
        redis_client.set('categories', json.dumps(data))
        return jsonify(data), 200
    except requests.exceptions.HTTPError as err:
        print(err)
        return jsonify({'message': 'No data found'}), 404


@pagination.route('/all')
@jwt_required()
def get_year_with_titles():
    params = request.args
    startYear = params.get('startYear')
    endYear = params.get('endYear')
    pageNumber = params.get('pageNumber')
    pageSize = params.get('pageSize')
    platformNames = params.get('platformNames')
    sortBy = params.get('sortBy')
    sort = params.get('sort')
    _type = params.get('type')
    selectedGenres = params.get('genres')
    lastmodDateURL = "http://localhost:5192/api/titles/lastmod"
    lastmodDate = requests.get(lastmodDateURL).json()
    lastSaved = redis_client.get('titles_with_ratings_lastmod')
    lastSaved = lastSaved.decode('utf-8') if lastSaved is not None else None
    if redis_client.get('titles_with_ratings') is None or lastSaved != lastmodDate:
        redis_client.set('titles_with_ratings_lastmod', lastmodDate)
        data = get_titles()
        for title in data:
            title_variations = [
                title['titleName'],
                title['titleName'].replace(" ", "-"),
                title['titleName'].replace("-", " "),
                title['titleName'].replace(",", ":"),
            ]
            year_variations = [
                title['release_Year'] - 1,
                title['release_Year'] - 2,
                title['release_Year'] - 3,
                title['release_Year'] - 4,
                title['release_Year'] - 5,
                title['release_Year'],
                title['release_Year'] + 1,
                title['release_Year'] + 2,
                title['release_Year'] + 3,
                title['release_Year'] + 4,
                title['release_Year'] + 5
            ]
            query = session.query(TitleJoined).filter(
                TitleJoined.start_year.in_(year_variations),
                or_(
                    (TitleJoined.primaryTitle.in_(title_variations)),
                    (TitleJoined.originalTitle.in_(title_variations))
                )
            ).first()
            over_all = (0, 0)

            if query:
                title['imdb_rating'] = query.average_rating
                over_all = (over_all[0] + query.average_rating, over_all[1] + 1)
            else:
                title['imdb_rating'] = 0
            rotten_tomatoes_query = session.query(RottenTomatoesMovies).filter(
                RottenTomatoesMovies.movie_title.in_(title_variations),
                extract('year', RottenTomatoesMovies.original_release_date).in_(year_variations)
            ).first()

            if rotten_tomatoes_query:
                if rotten_tomatoes_query.tomatometer_rating is None:
                    title['tomatometer'] = 0
                else:
                    title['tomatometer'] = rotten_tomatoes_query.tomatometer_rating / 10

                if rotten_tomatoes_query.audience_rating is None:
                    title['audience_rating'] = 0
                else:
                    title['audience_rating'] = rotten_tomatoes_query.audience_rating / 10
                if title['audience_rating'] > 0 or title['tomatometer'] > 0:
                    over_all = (over_all[0] + title['tomatometer'] + title['audience_rating'], over_all[1] + 2)
            else:
                title['tomatometer'] = None
                title['audience_rating'] = None
            if over_all[1] > 0:
                title['over_all'] = round(over_all[0] / over_all[1], 2)
            else:
                title['over_all'] = None
        # save to redis
        redis_client.set('titles_with_ratings', json.dumps(data))
    else:
        data = json.loads(redis_client.get('titles_with_ratings'))
    if selectedGenres is not None and selectedGenres != '':
        genres = selectedGenres.split(',')
        genres = [int(genre) for genre in genres]
        data = [
            title for title in data if any(
                category["category"]["id"] in genres for category in title["titleCategory"]
            )
        ]

    if _type is not None and _type != '':
        data = [title for title in data if title['type'] == _type]
    try:
        if startYear is not None and startYear != '' and endYear is not None and endYear != '':
            data = [title for title in data if
                    title['release_Year'] >= int(startYear) and title['release_Year'] <= int(endYear)]
    except:
        return jsonify({'message': f'{startYear} or {endYear} is not a number'}), 400

    if sortBy is not None and sortBy != '' and sortBy in SORT_BY:
        def sort_key(x):
            return (x.get(sortBy) is None, x.get(sortBy) if sort == "asc" else -x.get(sortBy) if isinstance(x.get(sortBy), (int, float)) else x.get(sortBy))
        data = sorted(data, key=sort_key)

    if pageNumber is None or pageNumber == '':
        pageNumber = 1
    if pageSize is None or pageSize == '':
        pageSize = 10
    totalPages = len(data) // int(pageSize) if len(data) % int(pageSize) == 0 else len(data) // int(pageSize) + 1
    data = data[(int(pageNumber) - 1) * int(pageSize):int(pageNumber) * int(pageSize)]
    res = {
        'items': data,
        'totalElements': len(data),
        'totalPages': totalPages,
        'pageNumber': pageNumber,
    }
    return jsonify(res), 200

@pagination.route('/download')
@jwt_required()
def download_titles():
    params = request.args
    startYear = params.get('startYear')
    endYear = params.get('endYear')
    sortBy = params.get('sortBy')
    sort = params.get('sort')
    _type = params.get('type')
    selectedGenres = params.get('genres')
    if redis_client.get('titles_with_ratings') is None:
        return jsonify({'message': 'No data found'}), 404
    else:
        data = json.loads(redis_client.get('titles_with_ratings'))

    if selectedGenres is not None and selectedGenres != '':
        genres = selectedGenres.split(',')
        genres = [int(genre) for genre in genres]
        data = [
            title for title in data if any(
                category["category"]["id"] in genres for category in title["titleCategory"]
            )
        ]

    if _type is not None and _type != '':
        data = [title for title in data if title['type'] == _type]
    try:
        if startYear is not None and startYear != '' and endYear is not None and endYear != '':
            data = [title for title in data if
                    title['release_Year'] >= int(startYear) and title['release_Year'] <= int(endYear)]
    except:
        return jsonify({'message': f'{startYear} or {endYear} is not a number'}), 400

    if sortBy is not None and sortBy != '' and sortBy in SORT_BY:
        def sort_key(x):
            return (x.get(sortBy) is None, x.get(sortBy) if sort == "asc" else -x.get(sortBy) if isinstance(x.get(sortBy), (int, float)) else x.get(sortBy))
        data = sorted(data, key=sort_key)

    response = make_response(jsonify(data))
    response.headers['Content-Disposition'] = 'attachment; filename=titles.json'
    response.headers['Content-Type'] = 'application/json'
    return response






