import time

from flask import Flask, jsonify
from flask_cors import CORS
from extensions import db, jwt, validate_database, create_join_table
from auth import auth
from models import User, TokenBlocklist, RottenTomatoesMovies
from users import users
from rotten_tomatoes_movies import rotten_tomatoes_movies
from controllers.imdbController import imdb
from controllers.mapController import _map
from admins import ADMIN_ACCOUNT_USERNAMES_LIST as admin_usernames
import alembic.config
from sqlalchemy import create_engine, MetaData, Table, insert
from sqlalchemy.exc import IntegrityError
from sqlalchemy.sql import select
import uuid
from concurrent.futures import ThreadPoolExecutor
import functools
import asyncio
import os
import datetime
import csv
import pandas as pd
from matchRating import save_match_rating
async def import_rotten_tomatoes_movies_from_csv(csv_file_path):
    start_time = time.time()  # Start timing
    alembic_cfg = alembic.config.Config('alembic.ini')

    # check if the table exists and is empty
    engine = create_engine(alembic_cfg.get_main_option('sqlalchemy.url'))
    metadata = MetaData()
    movies_table = Table('rotten_tomatoes_movies', metadata, autoload_with=engine)
    with engine.connect() as conn:
        result = conn.execute(select(movies_table)).first()
        if result is None:
            print("The table 'rotten_tomatoes_movies' is empty. Seeding the table.")
        else:
            print("The table 'rotten_tomatoes_movies' has been cleared.")


    def parse_date(date_str):
        try:
            return datetime.datetime.strptime(date_str, '%Y-%m-%d').date() if date_str else None
        except ValueError:
            return None

    if not os.path.isfile(csv_file_path):
        print(f"File not found: {csv_file_path}")
        return

    try:
        with db.session() as query_session:
            existing_movie_ids = set(row[0] for row in query_session.query(RottenTomatoesMovies.id).all())
        with open(csv_file_path, newline='', encoding='iso-8859-1') as csvfile:
            csv_reader = csv.DictReader(csvfile)
            entries_to_add = []
            for idx, row in enumerate(csv_reader):
                rotten_tomatoes_link = row.get('rotten_tomatoes_link')
                if rotten_tomatoes_link:
                    rotten_tomatoes_id = str(uuid.uuid5(uuid.NAMESPACE_DNS, rotten_tomatoes_link))

                    # Check if the entry already exist
                    if rotten_tomatoes_id not in existing_movie_ids:
                        tomatometer_rating = int(row.get('tomatometer_rating')) if row.get('tomatometer_rating') and row.get('tomatometer_rating').isdigit() else None
                        tomatometer_count = int(row.get('tomatometer_count')) if row.get('tomatometer_count') and row.get('tomatometer_count').isdigit() else None
                        audience_rating = int(row.get('audience_rating')) if row.get('audience_rating') and row.get('audience_rating').isdigit() else None
                        audience_count = int(row.get('audience_count')) if row.get('audience_count') and row.get('audience_count').isdigit() else None
                        tomatometer_top_critics_count = int(row.get('tomatometer_top_critics_count')) if row.get('tomatometer_top_critics_count') and row.get('tomatometer_top_critics_count').isdigit() else None
                        tomatometer_fresh_critics_count = int(row.get('tomatometer_fresh_critics_count')) if row.get('tomatometer_fresh_critics_count') and row.get('tomatometer_fresh_critics_count').isdigit() else None
                        tomatometer_rotten_critics_count = int(row.get('tomatometer_rotten_critics_count')) if row.get('tomatometer_rotten_critics_count') and row.get('tomatometer_rotten_critics_count').isdigit() else None

                        original_release_date = parse_date(row.get('original_release_date'))
                        streaming_release_date = parse_date(row.get('streaming_release_date'))

                        directors = row.get('directors')
                        if directors and len(directors) > 255:
                            directors = directors[:255]

                        authors = row.get('authors')
                        if authors and len(authors) > 255:
                            authors = authors[:255]

                        actors = row.get('actors')
                        if actors and len(actors) > 255:
                            actors = actors[:255]

                        entries_to_add.append({
                            'id': rotten_tomatoes_id,
                            'rotten_tomatoes_link': row.get('rotten_tomatoes_link'),
                            'movie_title': row.get('movie_title'),
                            'movie_info': row.get('movie_info'),
                            'critics_consensus': row.get('critics_consensus'),
                            'content_rating': row.get('content_rating'),
                            'genres': row.get('genres'),
                            'directors': directors,
                            'authors': authors,
                            'actors': actors,
                            'original_release_date': original_release_date,
                            'streaming_release_date': streaming_release_date,
                            'runtime': row.get('runtime'),
                            'production_company': row.get('production_company'),
                            'tomatometer_status': row.get('tomatometer_status'),
                            'tomatometer_rating': tomatometer_rating,
                            'tomatometer_count': tomatometer_count,
                            'audience_status': row.get('audience_status'),
                            'audience_rating': audience_rating,
                            'audience_count': audience_count,
                            'tomatometer_top_critics_count': tomatometer_top_critics_count,
                            'tomatometer_fresh_critics_count': tomatometer_fresh_critics_count,
                            'tomatometer_rotten_critics_count': tomatometer_rotten_critics_count
                        })
            with db.session() as insert_session:
                if entries_to_add:
                    try:
                        insertSQL = insert(movies_table).values(entries_to_add)
                        insert_session.execute(insertSQL)
                        insert_session.commit() # Commit the changes to the database

                    except IntegrityError as e:
                        print(f"Error during insert/update: {e}")
                        insert_session.rollback()

    except Exception as e:
        print(f"Error during CSV import: {e}")

    end_time = time.time()  # End timing
    print(f"Time taken for import: {end_time - start_time} seconds")
def process_chunk(args):
    chunk, movies_table, db = args  # Unpack arguments
    entries_to_add = []
    chunk = chunk.where(pd.notnull(chunk), None).to_dict(orient='records')
    alembic_cfg = alembic.config.Config('alembic.ini')
    for row in chunk:
        if not row.get('tconst') or not row.get('primaryTitle'):
            continue
        row['startYear'] = None if row.get('startYear') == '\\N' else row.get('startYear')
        row['isAdult'] = 1 if row.get('isAdult') == '1' else 0
        entries_to_add.append({
            'tconst': row.get('tconst'),
            'originalTitle': row.get('originalTitle'),
            'primaryTitle': row.get('primaryTitle'),
            'is_adult': row.get('isAdult'),
            'start_year': row.get('startYear'),
        })
    engine = create_engine(alembic_cfg.get_main_option('sqlalchemy.url'))
    metadata = MetaData()
    movies_table = Table('title_basics', metadata, autoload_with=engine)
    with engine.connect() as conn:
        with conn.begin():
            try:
                conn.execute(movies_table.insert(), entries_to_add)
            except Exception as e:
                print(f"Error inserting chunk: {e}")
                for entry in entries_to_add:
                    try:
                        conn.execute(movies_table.insert(), [entry])
                    except Exception as entry_error:
                        print(f"Error inserting entry: {entry}, Error: {entry_error}")

async def import_title_basics_from_tsv(tsv_file_path):
    start_time = time.time()  # Start timing
    alembic_cfg = alembic.config.Config('alembic.ini')

    # check if the table exists and is empty
    engine = create_engine(alembic_cfg.get_main_option('sqlalchemy.url'))
    metadata = MetaData()
    movies_table = Table('title_basics', metadata, autoload_with=engine)
    with engine.connect() as conn:
        result = conn.execute(select(1).select_from(movies_table).limit(1))  # Check existence
        if result.scalar() is None:
            print("The table 'movies_table' is empty. Seeding the table.")
        else:
            return

    if not os.path.isfile(tsv_file_path):
        print(f"File not found: {tsv_file_path}")
        return
    try:
        chunksize = 20000
        with pd.read_csv(tsv_file_path, sep='\t', chunksize=chunksize) as reader:
            with ThreadPoolExecutor() as executor:
                executor.map(process_chunk, [(chunk, movies_table, db) for chunk in reader])
    except Exception as e:
        print(f"Error during TSV import: {e}")

    end_time = time.time()  # End timing
    print(f"Time taken for import: {end_time - start_time} seconds")

def process_chunk_ratings(args):
    chunk, movies_table, db = args  # Unpack arguments
    entries_to_add = []
    chunk = chunk.where(pd.notnull(chunk), None).to_dict(orient='records')
    alembic_cfg = alembic.config.Config('alembic.ini')
    for row in chunk:
        if not row.get('tconst') or not row.get('averageRating'):
            continue
        row['numVotes'] = None if row.get('numVotes') == '\\N' else row.get('numVotes')
        entries_to_add.append({
            'tconst': row.get('tconst'),
            'average_rating': row.get('averageRating'),
            'num_votes': row.get('numVotes'),
        })
    engine = create_engine(alembic_cfg.get_main_option('sqlalchemy.url'))
    metadata = MetaData()
    ratings_table = Table('title_ratings', metadata, autoload_with=engine)
    with engine.connect() as conn:
        with conn.begin():
            try:
                conn.execute(ratings_table.insert(), entries_to_add)
            except Exception as e:
                print(f"Error inserting chunk: {e}")
                for entry in entries_to_add:
                    try:
                        conn.execute(ratings_table.insert(), [entry])
                    except Exception as entry_error:
                        print(f"Error inserting entry: {entry}, Error: {entry_error}")
async def import_title_ratings_from_tsv(tsv_file_path):
    start_time = time.time()  # Start timing
    alembic_cfg = alembic.config.Config('alembic.ini')

    # check if the table exists and is empty
    engine = create_engine(alembic_cfg.get_main_option('sqlalchemy.url'))
    metadata = MetaData()
    movies_ratings = Table('title_ratings', metadata, autoload_with=engine)
    with engine.connect() as conn:
        result = conn.execute(select(1).select_from(movies_ratings).limit(1))  # Check existence
        if result.scalar() is None:
            print("The table 'movies_table' is empty. Seeding the table.")
        else:
            return
    if not os.path.isfile(tsv_file_path):
        print(f"File not found: {tsv_file_path}")
        return
    try:
        chunksize = 5000
        with pd.read_csv(tsv_file_path, sep='\t', chunksize=chunksize) as reader:
            with ThreadPoolExecutor() as executor:
                executor.map(process_chunk_ratings, [(chunk, movies_ratings, db) for chunk in reader])
    except Exception as e:
        print(f"Error during TSV import: {e}")

    end_time = time.time()  # End timing
    print(f"Time taken for import: {end_time - start_time} seconds")



from time import sleep
from werkzeug.serving import is_running_from_reloader

def create_app():

    corsOriginsURL = os.getenv('CORS_ORIGINS_URL', 'http://localhost:3000')
    app = Flask(__name__)
    CORS(app, supports_credentials=True, origins=[corsOriginsURL])

    app.config.from_prefixed_env()

    # Initialize the extensions section
    db.init_app(app)
    jwt.init_app(app)


    # Registering the blueprint section
    app.register_blueprint(auth, url_prefix='/auth')
    app.register_blueprint(users, url_prefix='/users')
    app.register_blueprint(rotten_tomatoes_movies, url_prefix='/rotten')
    app.register_blueprint(imdb, url_prefix='/imdb')
    app.register_blueprint(_map, url_prefix='/map')


    with app.app_context():
        if not is_running_from_reloader():
            validate_database()
            alembic.config.main(argv=['upgrade', 'head'])
            asyncio.run(import_rotten_tomatoes_movies_from_csv('data/rotten_tomatoes_movies.csv'))
            asyncio.run(import_title_basics_from_tsv('data/title.basics.tsv'))
            asyncio.run(import_title_ratings_from_tsv('data/title.ratings.tsv'))
            asyncio.run(create_join_table())
            save_match_rating()





    # load user
    @jwt.user_lookup_loader
    def user_lookup_callback(_jwt_header, jwt_data):
        identity = jwt_data['sub']
        return User.query.filter_by(username=identity).one_or_none()

    # additional claims
    """
    This decorator function triggers whenever you create a new access token (e.g., when a user successfully logs in).
    """
    @jwt.additional_claims_loader
    def add_claims_to_access_token(identity):
        if identity in admin_usernames:
            return {
                'identity': identity,
                'role': 'admin'
            }
        else:
            return {
                'identity': identity,
                'role': 'user'
            }

    # JWT error handling section
    @jwt.expired_token_loader
    def expired_token_callback(jwt_header, jwt_data):
        return jsonify({
            'message': 'The token has expired',
            'error': 'token_expired'
        }), 401
    @jwt.invalid_token_loader
    def invalid_token_callback(error):
        return jsonify({
            'message': 'Signature verification failed',
            'error': 'invalid_token'
        }), 401
    @jwt.unauthorized_loader
    def unauthorized_callback(error):
        return jsonify({
            'message': 'Request does not contain an access token',
            'error': 'authorization_required'
        }), 401

    @jwt.token_in_blocklist_loader
    def token_in_blocklist_callback(jwt_header, jwt_data):
        jti = jwt_data['jti']

        token = TokenBlocklist.query.filter_by(jti=jti).one_or_none()

        return token is not None

    return app
if __name__ == '__main__':
    app = create_app()
    app.run()
