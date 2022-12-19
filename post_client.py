import requests
import json
import time
from csv import reader
import argparse
# app = Flask(__name__)
# # app.config['CORS_HEADERS'] = 'Content-Type'
# CORS(app)

class PostClient(object):
    def __init__(self):
        self.url = "160.40.54.171"# set an ip address
        self.ip = "5052"
        # instantiate a channel
        self.headers = {
            'accept': 'application/json'
        }

    def post_message(self, msg):
        """
        Client function to call the rpc for GetServerResponse
        """
#        example = {'command': 'summary'} #this would be the json msg to the server
#        example_json = json.dumps(example)
        try:
            response = requests.post("http://{}:{}/send".format(self.url,self.ip), headers= self.headers, data=msg)
            print(response.content)
        except requests.exceptions.RequestException as e:
            response = e
        return (response)

if __name__ == '__main__':
    try:
        # Parse arguments
        parser = argparse.ArgumentParser()
        parser.add_argument('--host')
        parser.add_argument('--port')
        args = parser.parse_args()

        # If the user has provided custom IP and port
        host = args.host
        port = args.port

    except Exception:
        # Use default values
        host = '0.0.0.0'
        port = 5002

    #app.run(debug=False, host=host, port=int(port))
    post = PostClient()
    #while(True):
    with open('sample1.CSV', 'r') as read_obj:
        # pass the file object to reader() to get the reader object
        json_msg = {}
        json_fields = {}
        csv_reader = reader(read_obj)
        for row in csv_reader:
            # row variable is a list that represents a row in csv
            try:
                if not row:
                    continue
                json_msg["DRONE_ID"] = "UAV1"
                json_msg[json_fields[0]] = float(row[4])
                json_msg[json_fields[1]] = float(row[5])
                json_msg[json_fields[2]] = float(row[7])
                json_msg[json_fields[3]] = float(row[1])
            except ValueError:
                #Values are srtings- use them as json's field names
                json_fields[0] = row[4]
                json_fields[1] = row[5]
                json_fields[2] = row[7]
                json_fields[3] = row[0]
                continue
            # convert into JSON:
            msg = json.dumps(json_msg)

            post.post_message(msg)
            time.sleep(1)
