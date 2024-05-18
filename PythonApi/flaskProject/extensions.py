from flask_sqlalchemy import SQLAlchemy
from flask_jwt_extended import JWTManager
from sqlalchemy import create_engine
from sqlalchemy_utils import database_exists, create_database
from alembic.config import Config

# Database manager
db = SQLAlchemy()
def validate_database():
    alembic_cfg = Config('alembic.ini')
    engine = create_engine(alembic_cfg.get_main_option('sqlalchemy.url'))

    if not database_exists(engine.url):
        create_database(engine.url)
        print('Database created')
# JWT manager
jwt = JWTManager()
