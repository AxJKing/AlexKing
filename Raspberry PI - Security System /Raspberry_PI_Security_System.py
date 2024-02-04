# Import necessary libraries
import RPi.GPIO as GPIO
import time
import Keypad
import math
import threading
import smtplib
import signal
from UltrasonicRangingModule import getSonar

# For the LCD display
from PCF8574 import PCF8574_GPIO
from Adafruit_LCD1602 import Adafruit_CharLCD
from time import sleep, strftime
from datetime import datetime


# Declaring email variables
SMTP_SERVER = 'smtp.gmail.com' #Email Server 
SMTP_PORT = 587 #Server Port 
GMAIL_USERNAME = '*******' 
GMAIL_PASSWORD = '*******' #App-password

# Define keypad layout 
ROWS = 4
COLS = 4
keys = [ '1','2','3','A', 
         '4','5','6','B',
         '7','8','9','C',
         '*','0','#','D' ]

# Define default state and code | Code is 1234
code = "1234"
state = "unarmed"

# Declaring an empty string for the code input to be appended to
inputCode = ""

# Boolean values to define end conditions for while loops
intruderDetected = False
motion_detected = False
emailSent = False
maxAttemptsReached = False

# Sets a time (in seconds) where the user will be alerted if the LED continues flashing
led_duration = 60

# Declaring variables to limit the number of times the user can enter the code
attempts = 0
max_attempts = 3

# Define pin numbers for components
rowPins = [36,38,40,37]
colsPins = [35,33,31,29]
yesButtonPin = 37
noButtonPin = 36
ledPin = 11
trigPin = 8
echoPin = 10
buzzerPin = 7

# Define maximum distance for sensor (should be cm)
max_dist = 10

# For the LCD display
PCF8574_address = 0x27 # I2C address of the PCF8574 chip.
mcp = PCF8574_GPIO(PCF8574_address)
lcd = Adafruit_CharLCD(pin_rs=0, pin_e=2, pins_db=[4,5,6,7], GPIO=mcp)

# Define a signal handler for the interrupt signal (Ctrl+C)
def signal_handler(signal, frame):
    print("Ctrl+C detected. Cleaning up...")
    lcd.clear()
    destroy()

# Set the signal handler for the interrupt signal
signal.signal(signal.SIGINT, signal_handler)


class Emailer:
    def sendmail(self, recipient, subject, content):

        #Create Headers
        headers = ["From: " + GMAIL_USERNAME, "Subject: " + subject, "To: " + recipient,
            "MIME-Version: 1.0", "Content-Type: text/html"]
        headers = "\r\n".join(headers)

        #Connect to Gmail Server
        session = smtplib.SMTP(SMTP_SERVER, SMTP_PORT)
        session.ehlo()
        session.starttls()
        session.ehlo()

        #Login to Gmail
        session.login(GMAIL_USERNAME, GMAIL_PASSWORD)

        #Send Email & Exit
        session.sendmail(GMAIL_USERNAME, recipient, headers + "\r\n\r\n" + content)
        session.quit


# Initial setup GPIO pins for components
def setup():
    global p
    GPIO.setmode(GPIO.BOARD)
    GPIO.setup(ledPin, GPIO.OUT)
    GPIO.setup(yesButtonPin, GPIO.IN, pull_up_down=GPIO.PUD_UP)
    GPIO.setup(noButtonPin, GPIO.IN, pull_up_down=GPIO.PUD_UP)
    GPIO.setup(trigPin, GPIO.OUT)
    GPIO.setup(echoPin, GPIO.IN)
    GPIO.setup(buzzerPin, GPIO.OUT)
    #GPIO.setup(rowPins + colsPins, GPIO.IN, pull_up_down=GPIO.PUD_UP)
    GPIO.setup(rowPins, GPIO.IN, pull_up_down=GPIO.PUD_UP)  # Set row pins as inputs with pull-up resistors
    GPIO.setup(colsPins, GPIO.OUT)
    p = GPIO.PWM(buzzerPin, 1)
    p.start(0);

# Checks if the code matches | Code is 1234
def checkCode():
    global inputCode, attempts, max_attempts, intruderDetected, maxAttemptsReached, motion_detected
    
    if inputCode == code:
        intruderDetected = False
        attempts = 0
        takeAction()
    else:
        # If the code doesn't match, the number of attempts is incremented by 1
        attempts += 1

        # Message displayed to the user showing the number of attempts they have remaining
        lcd.clear()
        lcd.setCursor(0,0)
        lcd.message("Incorrect Code!")
        lcd.setCursor(0,1)
        lcd.message("Attempts: {}/{}.".format(attempts, max_attempts))
        time.sleep(2)
        # Resets the input code string
        inputCode = ""

        # Checks whether the user has had 3 attempts to enter the code
        if attempts == max_attempts:
            motion_detected = False
            maxAttemptsReached = True
            # If so, the following message is shown on the LCD display
            lcd.clear()
            lcd.setCursor(0,0)
            lcd.message("Maximum Attempts")
            lcd.setCursor(0,1)
            lcd.message("Reached.")
            time.sleep(2)
            lcd.clear()
            getEmailDetails()
            destroy()

# Outputs the code being entered by the user on the LCD display
def displayLCD(inputCode):
    lcd.clear()
    lcd.setCursor(0,0)
    lcd.message(get_time_now())
    lcd.setCursor(0,1)
    lcd.message("Code:")
    lcd.setCursor(10,1)
    lcd.message(inputCode)

# Resets the input code to an empty string 
def clearButton():
    global inputCode
    inputCode = ""
    displayLCD(inputCode)

# Removes the last character from the end of the input code string
def backButton():
    global inputCode
    inputCode = inputCode[:-1]
    displayLCD(inputCode)

# Appends tothe input code string on a key press of the matrix keypad 
def updateCode(key):
    global inputCode
    inputCode += key
    displayLCD(inputCode)
    
# Once the input code is validated and is correct, this function is called
# Depending on the state of the system, the user will be asked if they want to arm or disarm the system
def takeAction():
    global inputCode, state

    # If unarmed, user will want to arm the system
    # If armed, user will want to disarm the system
    if state == "unarmed":
        # Shows message on LCD display
        lcd.clear()
        lcd.setCursor(0,0)
        lcd.message("Arm the system?")
        lcd.setCursor(0,1)
        lcd.message("Yes or No")

        # Checks whether the yes or no button has been pressed
        while True:
            # Yes button = Arm the System
            if GPIO.input(yesButtonPin) == GPIO.LOW:
                inputCode = ""
                armSystem()

            # No button = Return to main loop 
            if GPIO.input(noButtonPin) == GPIO.LOW:
                inputCode = ""
                loop()
                time.sleep(0.1)
    else:
        lcd.clear()
        lcd.setCursor(0,0)
        lcd.message("Disarm the ")
        lcd.setCursor(0,1)
        lcd.message("system?")

        # Turns the Blue LED off
        GPIO.output(ledPin, GPIO.LOW)

        # Checks whether the yes button has been pressed
        # If so, the system will be disarmed
        while True:
            if GPIO.input(yesButtonPin) == GPIO.LOW:
                inputCode = ""
                # Shows message on LCD display for 2 seconds before returning to main loop
                lcd.clear()
                lcd.setCursor(0,0)
                lcd.message("System Disarmed!")
                lcd.setCursor(0,1)
                lcd.message(""*16)
                time.sleep(2)
                loop()
                time.sleep(0.1)

            # Nothing happens if the no button is pressed
            if GPIO.input(noButtonPin) == GPIO.LOW:
                pass
            
# Updates the state of the system and begins checking for intruders
def armSystem():
    global state, intruderDetected
    state = "armed"
    inputCode = ""

    lcd.clear()
    lcd.setCursor(0,0)
    lcd.message("System Armed!")
    lcd.setCursor(0,1)
    lcd.message(""*16)
    time.sleep(2)
    
    GPIO.output(ledPin, GPIO.HIGH)
    intruderDetected = False
    checkForIntruder()

# Updates the state of the system and calls check code function
def disarm():
    global state
    state = "armed"
    checkCode()

# Check for intruders using ultrasonic sensor
def checkForIntruder():
    global inputCode, motion_detected, attempts
    
    motion_detected = True
    motion_thread = threading.Thread(target=detectMotion)
    motion_thread.start()

    keypad = Keypad.Keypad(keys, rowPins, colsPins, ROWS, COLS)
    keypad.setDebounceTime(100)
    inputCode = ""

    while motion_detected:
        if attempts > 0 and not inputCode:
            lcd.setCursor(0,1)
            lcd.message("Re-enter Code...")

        # Checking for user input
        key = keypad.getKey()
        if key != keypad.NULL:
            if key == "#":
                disarm()
            elif key == "C":
                clearButton()
            elif key == "B":
                backButton()
            else:
                updateCode(key)

        # Stops detecting motion when the variable is set to False 
        if motion_detected == False:
            motion_thread.join()
    
    motion_detected = False
    motion_thread.join()
    

# Once in an armed state, the ultrasonic ranging module will begin to detect motion in close proximity 
def detectMotion():
    global intruderDetected, motion_detected
    # Message shown on the LCD display
    lcd.clear()
    lcd.setCursor(0,0)
    lcd.message("Detecting ")
    lcd.setCursor(0,1)
    lcd.message("Motion...")

    # Program repeatedly checks for the distance 
    while motion_detected:
        distance = getSonar() # get distance
        print ("Motion detected at : %.2f cm"%(distance))
        time.sleep(1)

        # If a person/object gets within 10cm of the ultrasonic ranging module, the alarm is triggered 
        if distance < max_dist:
            intruderDetected = True
            # Message shown on the LCD display
            lcd.clear()
            lcd.setCursor(0,0)
            lcd.message("Alarm Triggered!")
            lcd.setCursor(0,1)
            lcd.message("Enter Code...")
            motion_detected = False
            triggerAlarm()
    

# If motion is detected in close proximity of the ultrasonic ranging module, the alarm is triggered
def triggerAlarm():
    global intruderDetected, inputCode, led_duration, emailSent, attempts
    GPIO.output(buzzerPin, GPIO.HIGH)

    # Using threads means that multiple actions can occur simultaneously 
    # Start a separate thread to handle LED flashing
    # Checking for user input whilst the led is flashing and the buzzer sounds 
    led_thread = threading.Thread(target=flashLED)
    led_thread.start()

    buzzer_thread = threading.Thread(target=alertor)
    buzzer_thread.start()

    keypad = Keypad.Keypad(keys, rowPins, colsPins, ROWS, COLS)
    keypad.setDebounceTime(100)
    inputCode = ""
    start_time = time.time()

    # Checks for user input
    while intruderDetected:
        if attempts > 0 and not inputCode:
            lcd.setCursor(0,1)
            lcd.message("Re-enter Code...")
            
        key = keypad.getKey()
        if key != keypad.NULL:
            if key == "#":
                disarm()
            elif key == "C":
                clearButton()
            elif key == "B":
                backButton()
            else:
                updateCode(key)

        # If the Blue LED has been flashing for more than 1 minute, an email will be sent
        elapsed_time = time.time() - start_time
        if elapsed_time >= led_duration and not emailSent:
            emailSent = True
            intruderDetected = False
            led_thread.join()
            getEmailDetails()

    # Wait for the LED thread to finish before exiting the function
    led_thread.join()

# When the alarm has been triggered, the Blue LED will flash until the system is disarmed
def flashLED():
    global intruderDetected
    while intruderDetected:
        GPIO.output(ledPin, GPIO.HIGH)
        time.sleep(0.5)
        GPIO.output(ledPin, GPIO.LOW)
        time.sleep(0.5)

# Stops the buzzer from sounding 
def stop_alertor():
    p.stop()
    GPIO.output(buzzerPin, GPIO.LOW)

# When the alarm has been triggered, the buzzer will sound for a specified time period
def alertor():
    global intruderDetected
    if intruderDetected:
        p.start(10)
        for x in range(0, 361):
            sinVal = math.sin(x * (math.pi / 180.0))
            toneVal = 175 + sinVal * 80
            p.ChangeFrequency(toneVal)
            
        # Calls stop_alertor() after a time delay
        threading.Timer(5, stop_alertor).start()

# Called when exiting the program  
def destroy():
    global state
    state = "unarmed"
    lcd.clear()
    GPIO.output(ledPin, GPIO.LOW)
    GPIO.cleanup()
    exit()

# Sends the email to the user with the information passed in as parameters 
def sendEmail(sendTo, emailSubject, emailContent, maxAttemptsReached):
    sender = Emailer()
    sender.sendmail(sendTo, emailSubject, emailContent)
    if maxAttemptsReached == False:
        loop()

# Retrieves details in preparation to send an email to the user
def getEmailDetails():
    global maxAttemptsReached
    sendTo = '*******'
    date = datetime.now().strftime('%d %b')
    time = datetime.now().strftime('%H:%M:%S')

    # 2 cases where an email will be sent to the user
    # Case 1: when someone has entered the code 3 times and reached the max number of attempts
    # Case 2: when the alarm has been triggered and hasn't been disarmed within 1 minute 
    if maxAttemptsReached == True:
        emailSubject = "Security Alert- Max Attempts has been reached!"
        emailContent = "The alarm code has been entered 3 times incorrectly on {} at {}.".format(date, time)
    else:
        emailSubject = "Security Alert- Motion Detected!"
        emailContent = "Motion has been detected on {} at {}.".format(date, time)
        
    sendEmail(sendTo, emailSubject, emailContent, maxAttemptsReached)

   
# Displays day of month, month name (short hand), hour, minute, second
def get_time_now():
    return datetime.now().strftime('%d %b %H:%M:%S')

# The main loop
def loop():
    global inputCode, state, motion_detected, attempts

    lcd.clear()
    state = "unarmed"
    inputCode = ""
    motion_detected = False
    mcp.output(3,1)
    lcd.begin(16,2)
    keypad = Keypad.Keypad(keys, rowPins, colsPins, ROWS, COLS)
    keypad.setDebounceTime(100)
    
    while True:
        # Sets the cursor to the first line
        lcd.setCursor(0,0)
        lcd.message(get_time_now())

        # If an incorrect attempt has been made, the following will be shown on the LCD display
        if attempts > 0 and not inputCode:
            lcd.setCursor(0,1)
            lcd.message("Re-enter Code...")
        else:
            if not inputCode:
                # Sets the cursor to the second line
                lcd.setCursor(0,1)
                lcd.message("Enter Code...")
            else:
                lcd.setCursor(0,1)
                lcd.message("Code: ")
                lcd.setCursor(10,1)
                lcd.message(inputCode)

        # Checking for user input on the matrix keypad and calls corresponding function
        key = keypad.getKey()
        if (key != keypad.NULL):
            if key == "#":
                print("# key is pressed")
                checkCode()
            elif key == "C":
                clearButton()
            elif key == "B":
                backButton()
            else:
                updateCode(key)


if __name__ == '__main__':
    print("Program is starting")
    setup()
    try:
        loop()
    except KeyboardInterrupt:
        destroy()





