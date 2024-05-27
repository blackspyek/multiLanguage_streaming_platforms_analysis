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
    title_id = fields.String() #a tconst,
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
    tconst = fields.String() # alphanumeric unique identifier of the title
    directors = fields.String(allow_none=True) # directors of the given title
    writers = fields.String(allow_none=True) # writers of the given title

class titlePrincipalsSchema(Schema):
    id = fields.String()
    tconst = fields.String() # alphanumeric unique identifier of the title
    ordering = fields.Integer() # a number to uniquely identify rows for a given titleId
    nconst = fields.String() # alphanumeric unique identifier of the name/person
    category = fields.String(allow_none=True) # the category of job that person was in
    job = fields.String(allow_none=True) # the specific job title if applicable, else '\N'
    characters = fields.String(allow_none=True) # the name of the character played if applicable, else '\N'

class titleRatingsSchema(Schema):
    id = fields.String()
    tconst = fields.String() # alphanumeric unique identifier of the title
    averageRating = fields.Float(allow_none=True) # weighted average of all the individual user ratings
    numVotes = fields.Integer(allow_none=True) # number of votes the title has received

#imdb_names

class nameBasicsSchema(Schema):
    id = fields.String()
    nconst = fields.String() # alphanumeric unique identifier of the name/person
    primaryName = fields.String(allow_none=True) # name by which the person is most often credited
    birthYear = fields.Integer(allow_none=True) # in YYYY format
    deathYear = fields.Integer(allow_none=True) # in YYYY format if applicable, else '\N'
    primaryProfession = fields.String(allow_none=True) # the top-3 professions of the person
    knownForTitles = fields.String(allow_none=True) # titles the person is known for