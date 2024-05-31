import os

from flask_sqlalchemy import SQLAlchemy
from flask_jwt_extended import JWTManager
from sqlalchemy import create_engine, inspect, text
from sqlalchemy_utils import database_exists, create_database
from alembic.config import Config
import redis
# Database manager
db = SQLAlchemy()
session = db.session
def validate_database():
    alembic_cfg = Config('alembic.ini')
    # wait for the database to be ready
    engine = create_engine(db.engine.url)
    if not database_exists(engine.url):
        create_database(engine.url)


# JWT manager
jwt = JWTManager()

REDIS_HOST = os.getenv('REDIS_HOST', 'localhost')
REDIS_PORT = os.getenv('REDIS_PORT', 6379)
REDIS_DB = os.getenv('REDIS_DB', 0)

# Redis manager
redis_client = redis.Redis(host=REDIS_HOST, port=REDIS_PORT, db=REDIS_DB)


async def create_join_table():
    print("Creating join table")
    engine = create_engine(db.engine.url)

    # Check if table exists before creating
    is_exists = inspect(engine).has_table('title_joined')
    if is_exists:
        return

    metadata = db.metadata

    # Modified SQL statement to create a regular table
    create_table = """
        CREATE TABLE title_joined AS
        SELECT
            tb.tconst,
            tb.originalTitle,
            tb.primaryTitle,
            tb.is_adult,
            tb.start_year,
            tr.average_rating,
            tr.num_votes
        FROM
            title_basics tb
        INNER JOIN
            title_ratings tr ON tb.tconst = tr.tconst
    """
    create_index1 = """
        CREATE INDEX idx_titlejoined_primary_title_start_year ON title_joined (primaryTitle(255), start_year);
    """

    create_index2 = """
        CREATE INDEX idx_titlejoined_original_title_start_year ON title_joined (originalTitle(255), start_year);
    """
    with engine.connect() as connection:
        connection.execute(text(create_table))
        connection.execute(text(create_index1))
        connection.execute(text(create_index2))






    print("Join table created")



