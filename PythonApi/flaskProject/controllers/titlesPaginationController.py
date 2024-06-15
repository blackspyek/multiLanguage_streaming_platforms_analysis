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
API_URL = os.getenv('API_URL', 'http://localhost:5192')

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
        if data is None or len(data) == 0:
            return None
        redis_client.set('titles', json.dumps(data))
        return data
    except requests.exceptions.HTTPError as err:
        print(err)
@pagination.route('/avgcategory')
def calculate_platform_category_ratings():
    params = request.args
    categoryname = params.get('category')
    if redis_client.get('category_ratings') is None:
        if redis_client.get('titles_with_ratings') is None:
            (res, status) = get_year_with_titles()
            if status == 404:
                return jsonify({'message': 'No data found'}), 404
        data = json.loads(redis_client.get('titles_with_ratings'))
        platform_category_ratings = {}
        for item in data:
            for platform_info in item["titlePlatform"]:
                platform_name = platform_info["platform"]["name"]
                if platform_name not in platform_category_ratings.keys():
                    platform_category_ratings[platform_name] = {}
                for category_info in item["titleCategory"]:
                    category_name = category_info["category"]["name"]
                    if category_name not in platform_category_ratings[platform_name].keys():
                        platform_category_ratings[platform_name][category_name] = []
                    if item["over_all"] is not None:
                        platform_category_ratings[platform_name][category_name].append(item["over_all"])

        for platform, categories in platform_category_ratings.items():
            for category, ratings in categories.items():
                try:
                    platform_category_ratings[platform][category] = round(sum(ratings) / len(ratings), 2)
                except ZeroDivisionError:
                    print(f'Error calculating average for {category} on {platform} with ratings {ratings}')

        redis_client.set('category_ratings', json.dumps(platform_category_ratings))
    else:
        platform_category_ratings = json.loads(redis_client.get('category_ratings'))

    platform_categories = {}
    AllPlatforms = {}
    for platform, categories in platform_category_ratings.items():
        for category, rating in categories.items():
            if categoryname is not None and categoryname != '' and category == categoryname:
                platform_categories[platform] = rating
                if category not in AllPlatforms.keys():
                    AllPlatforms[category] = []
                AllPlatforms[category].append(rating)

    for category, ratings in AllPlatforms.items():
        try:
            AllPlatforms = round(sum(ratings) / len(ratings), 2)
        except ZeroDivisionError:
            print(f'Error calculating average for {category} with ratings {ratings}')
    platform_categories['AllPlatforms'] = AllPlatforms
    return jsonify(platform_categories), 200
@pagination.route('/categories')
@jwt_required()
def get_Categories():
    if redis_client.get('categories') is not None:
        categories = redis_client.get('categories')
        categories = categories.decode('utf-8')
        return jsonify(json.loads(categories)), 200
    try:
        response = requests.get(f'{API_URL}/api/category')
        response.raise_for_status()
        data = response.json()
        redis_client.set('categories', json.dumps(data))
        return jsonify(data), 200
    except requests.exceptions.HTTPError as err:
        print(err)
        return jsonify({'message': 'No data found'}), 404
def get_titles_with_ratings():
    data = get_titles()
    if data is None:
        redis_client.delete('updating')
        return None
    redis_client.delete('titles_with_ratings')
    redis_client.set('updating', "True")
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
    redis_client.delete('updating')
    return json.dumps(data)
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
    lastmodDate = requests.get(f'{API_URL}/api/titles/lastmod').json()

    lastSaved = redis_client.get('titles_with_ratings_lastmod')
    lastSaved = lastSaved.decode('utf-8') if lastSaved is not None else None
    TestFlag = os.getenv('TEST', "1")
    if redis_client.get('titles_with_ratings') is None or (TestFlag == "0" and lastSaved != lastmodDate):
        redis_client.set('titles_with_ratings_lastmod', lastmodDate)
        data = get_titles_with_ratings()
        if data is None:
            return jsonify({'message': 'No data found'}), 404
    else:
        data = json.loads(redis_client.get('titles_with_ratings'))

    while redis_client.get('updating') == "True":
        pass
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


import xml.etree.ElementTree as ET

@pagination.route('/download/xml')
@jwt_required()
def download_titles_xml():
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

    # Create XML structure
    root = ET.Element("Titles")
    for title in data:
        title_element = ET.SubElement(root, "Title")
        for key, value in title.items():
            if isinstance(value, list):
                list_element = ET.SubElement(title_element, key)
                for item in value:
                    item_element = ET.SubElement(list_element, "Item")
                    for k, v in item.items():
                        sub_item_element = ET.SubElement(item_element, k)
                        sub_item_element.text = str(v)
            else:
                element = ET.SubElement(title_element, key)
                element.text = str(value)

    # Generate the XML string
    xml_data = ET.tostring(root, encoding='utf-8')

    response = make_response(xml_data)
    response.headers['Content-Disposition'] = 'attachment; filename=titles.xml'
    response.headers['Content-Type'] = 'application/xml'
    return response



