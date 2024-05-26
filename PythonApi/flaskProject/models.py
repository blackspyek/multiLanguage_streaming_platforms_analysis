from datetime import datetime
from sqlite3 import Date
from extensions import db
from uuid import uuid4
from werkzeug.security import generate_password_hash, check_password_hash


class User(db.Model):
    __tablename__ = 'users'
    id = db.Column(db.String(255), primary_key=True, default=lambda: str(uuid4()))
    username = db.Column(db.String(255), unique=True, nullable=False)
    email = db.Column(db.String(255), unique=True, nullable=False)
    password = db.Column(db.Text, nullable=False)

    def __repr__(self):
        return f'<User {self.username}>: {self.email}'

    def generate_password(self, password):
        self.password = generate_password_hash(password)

    def check_password(self, password):
        return check_password_hash(self.password, password)

    @classmethod
    def get_user_by_username(cls, username):
        return User.query.filter_by(username=username).first()

    def save(self):
        db.session.add(self)
        db.session.commit()

    def delete(self):
        db.session.delete(self)
        db.session.commit()


class TokenBlocklist(db.Model):
    __tablename__ = 'token_blocklist'
    id = db.Column(db.Integer, primary_key=True)
    jti = db.Column(db.String(120), nullable=False)
    create_at = db.Column(db.DateTime, nullable=False, default=datetime.utcnow)

    def __repr__(self):
        return f'<Token {self.jti}>'

    def save(self):
        db.session.add(self)
        db.session.commit()


# netflix
class Netflix(db.Model):
    __tablename__ = 'netflix'
    id = db.Column(db.String(255), primary_key=True, default=lambda: str(uuid4()))
    show_id = db.Column(db.String(255), nullable=False)
    type = db.Column(db.String(50), nullable=False)
    title = db.Column(db.String(255), nullable=False)
    director = db.Column(db.String(255), nullable=True)
    cast = db.Column(db.Text, nullable=True)
    country = db.Column(db.String(255), nullable=True)
    date_added = db.Column(db.String(255), nullable=True)
    release_year = db.Column(db.Integer, nullable=True)
    rating = db.Column(db.String(10), nullable=True)
    duration = db.Column(db.String(50), nullable=True)
    listed_in = db.Column(db.String(255), nullable=True)
    description = db.Column(db.Text, nullable=True)

    def __repr__(self):
        return f'<Netflix {self.title}>'

    def save(self):
        db.session.add(self)
        db.session.commit()


# rotten_tomatoes_movies
class RottenTomatoesMovies(db.Model):
    __tablename__ = 'rotten_tomatoes_movies'
    id = db.Column(db.String(255), primary_key=True, default=lambda: str(uuid4()))
    rotten_tomatoes_link = db.Column(db.String(255), nullable=False)
    movie_title = db.Column(db.String(255), nullable=True)
    movie_info = db.Column(db.Text, nullable=True)
    critics_consensus = db.Column(db.Text, nullable=True)
    content_rating = db.Column(db.String(50), nullable=True)
    genres = db.Column(db.String(255), nullable=True)
    directors = db.Column(db.String(255), nullable=True)
    authors = db.Column(db.String(255), nullable=True)
    actors = db.Column(db.Text, nullable=True)
    original_release_date = db.Column(db.Date, nullable=True)
    streaming_release_date = db.Column(db.Date, nullable=True)
    runtime = db.Column(db.String(50), nullable=True)
    production_company = db.Column(db.String(255), nullable=True)
    tomatometer_status = db.Column(db.String(50), nullable=True)
    tomatometer_rating = db.Column(db.Integer, nullable=True)
    tomatometer_count = db.Column(db.Integer, nullable=True)
    audience_status = db.Column(db.String(50), nullable=True)
    audience_rating = db.Column(db.Integer, nullable=True)
    audience_count = db.Column(db.Integer, nullable=True)
    tomatometer_top_critics_count = db.Column(db.Integer, nullable=True)
    tomatometer_fresh_critics_count = db.Column(db.Integer, nullable=True)
    tomatometer_rotten_critics_count = db.Column(db.Integer, nullable=True)

    def __repr__(self):
        return f'<RottenTomatoesMovies {self.movie_title}>'

    def save(self):
        db.session.add(self)
        db.session.commit()


# imdb_movies
class titleAkas(db.Model):
    __tablename__ = 'title_akas'
    id = db.Column(db.String(255), primary_key=True, default=lambda: str(uuid4()))
    title_id = db.Column(db.String(255), nullable=False)
    ordering = db.Column(db.Integer, nullable=True)
    title = db.Column(db.String(255), nullable=True)
    region = db.Column(db.String(50), nullable=True)
    language = db.Column(db.String(50), nullable=True)
    types = db.Column(db.String(50), nullable=True)
    attributes = db.Column(db.String(50), nullable=True)
    is_original_title = db.Column(db.Integer, nullable=True)

    def __repr__(self):
        return f'<titleAkas {self.title}>'

    def save(self):
        db.session.add(self)
        db.session.commit()


class titleBasics(db.Model):
    __tablename__ = 'title_basics'
    tconst = db.Column(db.String(255), nullable=False, primary_key=True)
    original_title = db.Column(db.String(255), nullable=True)
    is_adult = db.Column(db.Boolean, nullable=True)
    start_year = db.Column(db.Integer, nullable=True)

    def __repr__(self):
        return f'<titleBasics {self.primary_title}>'

    def save(self):
        db.session.add(self)
        db.session.commit()


class titleCrew(db.Model):
    __tablename__ = 'title_crew'
    id = db.Column(db.String(255), primary_key=True, default=lambda: str(uuid4()))
    tconst = db.Column(db.String(255), nullable=False)
    directors = db.Column(db.String(255), nullable=True)
    writers = db.Column(db.String(255), nullable=True)

    def __repr__(self):
        return f'<titleCrew {self.tconst}>'

    def save(self):
        db.session.add(self)
        db.session.commit()


class titlePrincipals(db.Model):
    __tablename__ = 'title_principals'
    id = db.Column(db.String(255), primary_key=True, default=lambda: str(uuid4()))
    tconst = db.Column(db.String(255), nullable=False)
    ordering = db.Column(db.Integer, nullable=False)
    nconst = db.Column(db.String(255), nullable=False)
    category = db.Column(db.String(50), nullable=True)
    job = db.Column(db.String(50), nullable=True)
    characters = db.Column(db.String(255), nullable=True)

    def __repr__(self):
        return f'<titlePrincipals {self.tconst}>'

    def save(self):
        db.session.add(self)
        db.session.commit()


class titleRatings(db.Model):
    __tablename__ = 'title_ratings'
    id = db.Column(db.String(255), primary_key=True, default=lambda: str(uuid4()))
    tconst = db.Column(db.String(255), nullable=False)
    average_rating = db.Column(db.Float, nullable=True)
    num_votes = db.Column(db.Integer, nullable=True)

    def __repr__(self):
        return f'<titleRatings {self.tconst}>'

    def save(self):
        db.session.add(self)
        db.session.commit()


# imdb_names
class nameBasics(db.Model):
    __tablename__ = 'name_basics'
    id = db.Column(db.String(255), primary_key=True, default=lambda: str(uuid4()))
    nconst = db.Column(db.String(255), nullable=False)
    primary_name = db.Column(db.String(255), nullable=True)
    birth_year = db.Column(db.Integer, nullable=True)
    death_year = db.Column(db.Integer, nullable=True)
    primary_profession = db.Column(db.String(255), nullable=True)
    known_for_titles = db.Column(db.String(255), nullable=True)

    def __repr__(self):
        return f'<nameBasics {self.primary_name}>'

    def save(self):
        db.session.add(self)
        db.session.commit()
