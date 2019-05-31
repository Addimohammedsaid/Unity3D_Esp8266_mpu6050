from machine import Pin, I2C
import time
import gc
import socket

import mpu6050


# enable automatic garbage collection
gc.enable()


# define I2C pins :
i2c = I2C(scl=Pin(13), sda=Pin(12))

# define the ip addresse & port
IP = ""
PORT = 5015

BUFFER = 20


# definition for the mpu6050
accel = mpu6050.accel(i2c)
dictAccel = accel.get_values()

# function for calibrating the sensor must be in
# fix position at the start


def calibrate(thresh):

    e = smooth_values()

    for key in thresh:
        e[key] -= thresh[key]

    return e

# function for getting a calibration threshold


def threshold(threshold=50, samples=100):
    while True:
        v1 = smooth_values(samples)
        v2 = smooth_values(samples)

        if all(abs(v1[key]-v2[key]) < threshold for key in v1.keys()):
            return v1


# function for smoothing values of the mpu6050
def smooth_values(samples=10):

    result = accel.get_values()

    for key in result:
        result[key] = 1

    for _ in range(samples):
        data = accel.get_values()

        for key in data:
            result[key] += (data[key] / samples)

    return result


# define threshold :
thresh = threshold()

# create the socket object
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to the port
sock.bind((IP, PORT))


sock.listen(1)

try:
    while True:

            # get data from the mpu6050
            #dictAccel = accel.get_values()

            # wait for connection
        print("wating......")

        conn, addr = sock.accept()

        print(addr)

        try:
            while True:
                data = conn.recv(BUFFER)
                if not data:
                    break

                # smooth the values by an average
                dictAccel = calibrate(thresh)
                print(dictAccel)

                print("received message:", data)
                data = data.decode('utf-8').split()
                response = ''

                for i in data:
                    response += str(dictAccel[i]) + ','

                conn.sendall(response)

        finally:
            print('socket close 1')

finally:
    sock.close()
    print('socket close 2')
