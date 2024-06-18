from flask import Blueprint, jsonify, request, make_response
from flask_jwt_extended import (create_access_token, create_refresh_token,
                                jwt_required, get_jwt, current_user,
                                get_jwt_identity
                                )
from models import User, TokenBlocklist
from admins import ADMIN_ACCOUNT_USERNAMES_LIST as admin_usernames

auth = Blueprint('auth', __name__)

@auth.post('/register')
def register_user():
    data = request.get_json()
    user = User.check_if_user_exists(username=data['username'], email=data['email'])

    if user:
        return jsonify({'message': 'User already exists!'}), 400

    new_user = User(
        username=data['username'],
        email=data['email']
    )
    new_user.generate_password(
        password=data['password']
    )
    try:
        new_user.save()
    except:
        return jsonify({'message': 'User already exists!'}), 400

    return jsonify({'message': f'User {new_user.username} created successfully!'}), 201

@auth.post('/login')
def login_user():
    data = request.get_json()
    user = User.get_user_by_username(username=data['username'])
    # from claims = get_jwt() get roles

    if not user:
        return jsonify({'message': 'User not found!'}), 404
    if not user.check_password(password=data['password']):
        return jsonify({'message': 'Invalid password!'}), 400
    role = 'admin' if user.username in admin_usernames else 'user'

    access_token = create_access_token(identity=user.username)
    refresh_token = create_refresh_token(identity=user.username)
    response = make_response(jsonify({
        'tokens': {
            'access': access_token,
            'refresh': refresh_token
        },
        'role': role,
    }))
    response.set_cookie('access_token_cookie', access_token, httponly=True)
    response.set_cookie('refresh_token_cookie', refresh_token, httponly=True)

    return response,200
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
@auth.get('/refreshtoken')
@jwt_required(refresh=True, locations=['cookies'])
def refresh_access_token():
    identity = get_jwt_identity()
    new_access_token = create_access_token(identity=identity)

    role = 'admin' if identity in admin_usernames else 'user'
    response = make_response(jsonify({
        'access_token': new_access_token,
        'role': role
    }))
    response.set_cookie('access_token_cookie', new_access_token, httponly=True)
    return response, 200
@auth.get('/logout/cookies')
@jwt_required(verify_type=False, locations=['cookies'])
def logout_user_cookie():
    jwt = get_jwt()
    print(jwt)
    jti = jwt['jti']
    token_type = jwt['type']
    token = TokenBlocklist(jti=jti)
    token.save()
    response = make_response(jsonify({'message': f'{token_type} token revoked'}))
    response.set_cookie('access_token_cookie', '', httponly=True)
    response.set_cookie('refresh_token_cookie', '', httponly=True)
    return response, 200

@auth.get('/logout')
@jwt_required(verify_type=False)
def logout_user():
    jwt = get_jwt()

    jti = jwt['jti']
    token_type = jwt['type']
    token = TokenBlocklist(jti=jti)
    token.save()
    return jsonify({'message': f'{token_type} token revoked'}), 200

