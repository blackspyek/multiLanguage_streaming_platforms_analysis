FROM python:3.10.14-alpine3.19
ENV FLASK_APP=app.py
ENV FLASK_RUN_HOST=0.0.0.0
ENV FLASK_RUN_PORT=5000
WORKDIR /app
COPY . /app
RUN pip install -r requirements.txt 
EXPOSE 5000
CMD ["flask", "run"]