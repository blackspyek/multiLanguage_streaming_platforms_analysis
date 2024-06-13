from flask import Blueprint, jsonify, request
from models import User
from schemas import UserSchema
from flask_jwt_extended import jwt_required, get_jwt
users = Blueprint('users', __name__)
@users.route('/all')
@jwt_required()
def get_users():
    claims = get_jwt()
    if claims['role'] != 'admin':
        return jsonify({'message': 'You are not authorized to access this resource'}), 403

    page = request.args.get('page', default=1, type=int)
    per_page = request.args.get('per_page', default=3, type=int)
    users_pagination = User.query.paginate(
        per_page=per_page,
        page=page,
        error_out=True
    )
    users = users_pagination.items
    result = UserSchema().dump(users, many=True)
    return jsonify({
        "users": result
    }), 200
@users.route('/role')
@jwt_required()
def get_user_roles():
    claims = get_jwt()
    return jsonify({'role': claims['role']}), 200