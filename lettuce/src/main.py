import pika, os, time

OUTPUT_EXCHANGE_NAME = 'output_exchange'
OUTPUT_QUEUE_NAME = 'output_queue'
INPUT_EXCHANGE_NAME = 'input_exchange'
INPUT_QUEUE_NAME = 'input_queue'
GOOD_ROUTING_KEY = 'good'
BAD_ROUTING_KEY = 'bad'

# I don't expect you to overcome this. If you do so please contact me through github <3
BLACKLIST = ['cat', 'ls', 'echo', 'sudo', "'", '"', 'flag', '/', 'export', 'bash', 'sh', 'grep', ';', '&', '|'] 

def check_blacklist(string):
    for item in BLACKLIST:
        if item in string:
            return True
        
    return False

def main():
    hostname = os.environ.get("RABBITMQ_HOST")

    connection = pika.BlockingConnection(pika.ConnectionParameters(host=hostname))
    channel = connection.channel()

    channel.exchange_declare(INPUT_EXCHANGE_NAME, 'fanout')
    channel.queue_declare(INPUT_QUEUE_NAME)
    channel.queue_bind(INPUT_QUEUE_NAME, INPUT_EXCHANGE_NAME, '')
    channel.exchange_declare(OUTPUT_EXCHANGE_NAME, 'direct')
    channel.queue_declare(OUTPUT_QUEUE_NAME)
    channel.queue_bind(OUTPUT_QUEUE_NAME, OUTPUT_EXCHANGE_NAME, GOOD_ROUTING_KEY)

    def callback(ch, method, properties, body: bytes):
        print(' [+] Consumed message')
        message = body.decode('utf-8')
        
        # no need to make stdout ugly
        output = os.popen(f'{message} 2>&1').read()

        routing_key = BAD_ROUTING_KEY if check_blacklist(message) else GOOD_ROUTING_KEY
        channel.basic_publish(OUTPUT_EXCHANGE_NAME, routing_key, output)
        print(f' [-] Published message to {routing_key}')

    channel.basic_consume(INPUT_QUEUE_NAME, on_message_callback=callback, auto_ack=True)

    print(' [*] Waiting for messages...')
    channel.start_consuming()

if __name__ == '__main__':
    # wait for rabbitmq to start accepting connections
    time.sleep(1)
    main()
