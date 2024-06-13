from extensions import redis_client, db
from models import TitleJoined
from sqlalchemy import func
from json import dumps, loads
from json import loads, dumps  # Add import statements


def get_bestRated(type = "best"):
    cache_key = "best_title_ratings"  # Updated cache key
    cache_key = f"{cache_key}_{type}"
    #delete cache_key
    redis_client.delete(cache_key)
    cached_data = redis_client.get(cache_key)

    if cached_data:
        cached_data = cached_data.decode('utf-8')
        cached_data = loads(cached_data)
        return cached_data


    results = (
        db.session.query(
            TitleJoined.primaryTitle, TitleJoined.average_rating, TitleJoined.num_votes, TitleJoined.titleType
        )  
        .all()
    )

    titles_data = {
        title.primaryTitle: {
            "average_rating": title.average_rating,
            "votes_count": title.num_votes,
            "title_type": title.titleType,
        } for title in results
    }

    redis_client.set(cache_key, dumps(titles_data))

    return titles_data



