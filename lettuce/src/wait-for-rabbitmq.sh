#!/bin/sh
# Wait for RabbitMQ to be ready before starting the backend

echo "Waiting for RabbitMQ..."
until nc -z rabbitmq 5672; do
  sleep 1
done

echo "RabbitMQ is up - starting backend"
exec "$@"