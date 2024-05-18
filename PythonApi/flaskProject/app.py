import time

from flask import Flask, jsonify
from extensions import db, jwt, validate_database
from auth import auth
from models import User, TokenBlocklist
from users import users
from admins import ADMIN_ACCOUNT_USERNAMES_LIST as admin_usernames
import alembic.config
def create_app():
    time.sleep(10)
    validate_database()
    alembic.config.main(argv=['upgrade', 'head'])


    app = Flask(__name__)
    app.config.from_prefixed_env()

    # Initialize the extensions section
    db.init_app(app)
    jwt.init_app(app)


    # Registering the blueprint section
    app.register_blueprint(auth, url_prefix='/auth')
    app.register_blueprint(users, url_prefix='/users')

    # load user
    @jwt.user_lookup_loader
    def user_lookup_callback(_jwt_header, jwt_data):
        identity = jwt_data['sub']
        return User.query.filter_by(username=identity).one_or_none()

    # additional claims
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
