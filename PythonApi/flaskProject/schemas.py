from marshmallow import Schema, fields

class UserSchema(Schema):
    id = fields.String()
    username = fields.String()
    email = fields.Email()

class NetflixSchema(Schema):
    id = fields.String()
    show_id = fields.String()
    type = fields.String()
    title = fields.String()
    director = fields.String(allow_none=True)
    cast = fields.String(allow_none=True)
    country = fields.String(allow_none=True)
    date_added = fields.String(allow_none=True)
    release_year = fields.Integer(allow_none=True)
    rating = fields.String(allow_none=True)
    duration = fields.String(allow_none=True)
    listed_in = fields.String(allow_none=True)
    description = fields.String(allow_none=True)

class RottenTomatoesMoviesSchema(Schema):
    id = fields.String()
    rotten_tomatoes_link = fields.String()
    movie_title = fields.String(allow_none=True)
    movie_info = fields.String(allow_none=True)
    critics_consensus = fields.String(allow_none=True)
    content_rating = fields.String(allow_none=True)
    genres = fields.String(allow_none=True)
    directors = fields.String(allow_none=True)
    authors = fields.String(allow_none=True)
    actors = fields.String(allow_none=True)
    original_release_date = fields.String(allow_none=True)
    streaming_release_date = fields.String(allow_none=True)
    runtime = fields.String(allow_none=True)
    production_company = fields.String(allow_none=True)
    tomatometer_status = fields.String(allow_none=True)
    tomatometer_rating = fields.Integer(allow_none=True)
    tomatometer_count = fields.Integer(allow_none=True)
    audience_status = fields.String(allow_none=True)
    audience_rating = fields.Integer(allow_none=True)
    audience_count = fields.Integer(allow_none=True)
    tomatometer_top_critics_count = fields.Integer(allow_none=True)
    tomatometer_fresh_critics_count = fields.Integer(allow_none=True)
    tomatometer_rotten_critics_count = fields.Integer(allow_none=True)

#imdb_movies

class titleAkasSchema(Schema):
    id = fields.String()
    title_id = fields.String()
    ordering = fields.Integer(allow_none=True)
    title = fields.String(allow_none=True)
    region = fields.String(allow_none=True)
    language = fields.String(allow_none=True)
    types = fields.String(allow_none=True)
    attributes = fields.String(allow_none=True)
    is_original_title = fields.Integer(allow_none=True)

class titleBasicsSchema(Schema):
    tconst = fields.String()
    primaryTitle = fields.String(allow_none=True)
    isAdult = fields.Integer(allow_none=True)
    startYear = fields.Integer(allow_none=True)

class titleCrewSchema(Schema):
    id = fields.String()
    tconst = fields.String()
    directors = fields.String(allow_none=True)
    writers = fields.String(allow_none=True)

class titlePrincipalsSchema(Schema):
    id = fields.String()
    tconst = fields.String()
    ordering = fields.Integer()
    nconst = fields.String()
    category = fields.String(allow_none=True)
    job = fields.String(allow_none=True)
    characters = fields.String(allow_none=True)

class titleRatingsSchema(Schema):
    id = fields.String()
    tconst = fields.String()
    averageRating = fields.Float(allow_none=True)
    numVotes = fields.Integer(allow_none=True)

#imdb_names

class nameBasicsSchema(Schema):
    id = fields.String()
    nconst = fields.String()
    primaryName = fields.String(allow_none=True)
    birthYear = fields.Integer(allow_none=True)
    deathYear = fields.Integer(allow_none=True)
    primaryProfession = fields.String(allow_none=True)
    knownForTitles = fields.String(allow_none=True)

class titleBasicsRatingSchema(Schema):
    tconst = fields.String()
    primaryTitle = fields.String(allow_none=True)
    isAdult = fields.Integer(allow_none=True)
    startYear = fields.Integer(allow_none=True)
    averageRating = fields.Float(allow_none=True)
    numVotes = fields.Integer(allow_none=True)