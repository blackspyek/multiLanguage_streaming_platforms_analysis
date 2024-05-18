from extensions import db
from uuid import uuid4
from werkzeug.security import generate_password_hash, check_password_hash
from datetime import datetime
class User(db.Model):
    __tablename__ = 'users'
    id = db.Column(db.String(36), primary_key=True, default=lambda: str(uuid4()))
    username = db.Column(db.String(255), unique=True, nullable=False)
    email = db.Column(db.String(255), unique=True, nullable=False)
    password = db.Column(db.Text(255), nullable=False)

    def __repr__(self):
        return f'<User {self.username}>: {self.email}'

    def generate_password(self, password): #sha256
        self.password = generate_password_hash(password)
    def check_password(self, password):
        return check_password_hash(self.password, password)
    @classmethod
    def get_user_by_username(self, username): #TU Miał cls instead of self
        return User.query.filter_by(username=username).first()
    @classmethod
    def get_user_by_email(self, email): #TU Miał cls instead of self
        return User.query.filter_by(email=email).first()
    @classmethod
    def check_if_user_exists(self, username, email):
        return User.query.filter(
            (User.username == username) | (User.email == email)
        ).first()
    def save(self):
        db.session.add(self)
        db.session.commit()
    def delete(self):
        db.session.delete(self)
        db.session.commit()

class TokenBlocklist(db.Model):
    __tablename__ = 'token_blocklist'
    id = db.Column(db.Integer, primary_key=True)
    jti = db.Column(db.String(120), nullable=False)
    create_at = db.Column(db.DateTime, nullable=False, default=datetime.utcnow)

    def __repr__(self):
        return f'<Token {self.jti}>'
    def save(self):
        db.session.add(self)
        db.session.commit()