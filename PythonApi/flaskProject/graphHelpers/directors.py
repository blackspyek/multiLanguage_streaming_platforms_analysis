from extensions import redis_client, db
from models import TitleJoined, nameBasics, TitleDirector
from sqlalchemy import func
from json import dumps, loads
from json import loads, dumps  # Add import statements


def get_directors(type = "movie"):
    cache_key = "director_ratings_and_counts"  # Updated cache key
    cache_key = f"{cache_key}_{type}"
    #delete cache_key
    redis_client.delete(cache_key)
    cached_data = redis_client.get(cache_key)

    if cached_data:
        cached_data = cached_data.decode('utf-8')
        cached_data = loads(cached_data)
        return cached_data

    subquery = (
        db.session.query(
            TitleDirector.nconst,
            func.avg(TitleJoined.average_rating).label("average_rating"),
            func.count(TitleDirector.tconst).label("movies_count")
        )
        .join(TitleJoined, (TitleDirector.tconst == TitleJoined.tconst) & (TitleJoined.titleType == type))
        .group_by(TitleDirector.nconst)
        .subquery()
    )

    results = (
        db.session.query(
            nameBasics.primary_name, subquery.c.average_rating, subquery.c.movies_count
        )  # Include movies_count in the main query
        .join(subquery, subquery.c.nconst == nameBasics.nconst)
        .all()
    )

    director_data = {
        director.primary_name: {
            "average_rating": director.average_rating,
            "movies_count": director.movies_count
        } for director in results
    }

    redis_client.set(cache_key, dumps(director_data))

    return director_data






