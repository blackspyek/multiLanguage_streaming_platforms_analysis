from flask import Blueprint, jsonify, request
from flask_jwt_extended import (create_access_token, create_refresh_token,
                                jwt_required, get_jwt, current_user,
                                get_jwt_identity
                                )
from models import User, TokenBlocklist

auth = Blueprint('auth', __name__)

@auth.post('/register')
def register_user():
    data = request.get_json()
    user = User.get_user_by_username(username=data['username'])
    if user:
        return jsonify({'message': 'User already exists!'}), 400

    new_user = User(
        username=data['username'],
        email=data['email']
    )
    new_user.generate_password(
        password=data['password']
    )
    new_user.save()
    return jsonify({'message': f'User {new_user.username} created successfully!'}), 201

@auth.post('/login')
def login_user():
    data = request.get_json()
    user = User.get_user_by_username(username=data['username'])
    if not user:
        return jsonify({'message': 'User not found!'}), 404
    if not user.check_password(password=data['password']):
        return jsonify({'message': 'Invalid password!'}), 400
    return jsonify({
        'message': 'Logged in successfully!',
        'tokens': {
            'access': create_access_token(identity=user.username),
            'refresh': create_refresh_token(identity=user.username)
        }
    }), 200
@auth.get('/whoami')
@jwt_required()
def who_am_i():
    return jsonify(
        {
            'message': f'message',
            'user_details': {
                'username': current_user.username,
                'email': current_user.email
            }
        }
    )
@auth.get('/refresh')
@jwt_required(refresh=True)
def refresh_access():
    identity = get_jwt_identity()
    new_access_token = create_access_token(identity=identity)
    return jsonify({
        'access_token': new_access_token
    }), 200
@auth.get('/logout')
@jwt_required(verify_type=False)
def logout_user():
    jwt = get_jwt()
    jti = jwt['jti']
    token_type = jwt['type']
    token = TokenBlocklist(jti=jti)
    token.save()
    return jsonify({'message': f'{token_type} token revoked'}), 200