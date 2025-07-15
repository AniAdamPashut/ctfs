FROM python:alpine

WORKDIR /app

COPY requirements.txt .

RUN pip install -r requirements.txt

COPY flag flag
COPY src .

RUN chmod +x wait-for-rabbitmq.sh

ENTRYPOINT  ["sh", "-c", "./wait-for-rabbitmq.sh python main.py"]