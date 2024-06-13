from controllers.authController import auth
from controllers.usersController import users
from controllers.rottenTomatoController import rotten_tomatoes_movies
from controllers.imdbController import imdb
from controllers.mapController import _map
from controllers.directorsController import directors
from controllers.bestRatedController import bestRated
from controllers.titlesPaginationController import pagination
from controllers.streamingController import streaming

def register_blueprints(app):
    app.register_blueprint(auth, url_prefix='/auth')
    app.register_blueprint(users, url_prefix='/users')
    app.register_blueprint(rotten_tomatoes_movies, url_prefix='/rotten')
    app.register_blueprint(imdb, url_prefix='/imdb')
    app.register_blueprint(_map, url_prefix='/map')
    app.register_blueprint(pagination, url_prefix='/paginate')
    app.register_blueprint(directors, url_prefix='/directors')
    app.register_blueprint(bestRated, url_prefix='/bestRated')
    app.register_blueprint(streaming, url_prefix='/streaming')
