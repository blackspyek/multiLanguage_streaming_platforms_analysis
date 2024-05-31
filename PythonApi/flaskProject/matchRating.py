from models import titleBasics, titleRatings
from extensions import redis_client, db
from sqlalchemy import func
import json
import time
def save_match_rating():
    start = time.time()

    if redis_client.get("ratings_loaded") is not None:
        return

    result = db.session.query(
                titleBasics.start_year,
                func.avg(titleRatings.average_rating).label('avg_rating')
            ).join(
                titleRatings, titleBasics.tconst == titleRatings.tconst
            ).group_by(
                titleBasics.start_year
            ).all()
    ratings_by_year = [{"year": row.start_year, "avg": float(row.avg_rating)} for row in result if row.start_year is not None]

    ratings_by_year_str = json.dumps(ratings_by_year)
    redis_client.set("ratings_by_year", ratings_by_year_str)
    redis_client.set("ratings_loaded", 1)
    end = time.time()

    print(f"Time taken to save match rating: {end - start}")

def get_average_rating_by_year(year):
    ratings_by_year = redis_client.get("ratings_by_year")
    if ratings_by_year is None:
        return None
    ratings_by_year = eval(ratings_by_year.decode('utf-8'))
    return ratings_by_year.get(year)
def get_years_avg():
    years = redis_client.get("ratings_by_year")
    if not years:
        return {}
    years = years.decode('utf-8')
    return json.loads(years)