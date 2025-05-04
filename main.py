import json
import time

n = 0

while True:
    sensor_value = "sensor value" 
    json_data = {
        "mensaje": n,
        "contenido": sensor_value
    }

    json_string = json.dumps(json_data, indent=4)
    print(json_string)

    n += 1
    time.sleep(5)