import json

def open_and_close_json_file(mode):
    try:
        with open('userData.json', mode) as file:
            return json.load(file)
    except FileNotFoundError:
        return {}

def save_user_data(data):
    with open('userData.json', 'w') as file:
        json.dump(data, file, indent=2)

def validate_password(password):
    return len(password) >= 8 and ' ' not in password and not password[0].isdigit()

def new_user():
    while True:
        username = input("Enter a username: ")
        password = input("Enter a password: ")
        confirm = input("Re-enter the password to confirm: ")
        
        if password == confirm and validate_password(password):
            data = open_and_close_json_file('r+')
            data[username] = password
            save_user_data(data)
            print("User created successfully!")
            break
        else:
            print("Invalid input. Please try again.")

def existing_user():
    attempts = 3
    username = input("Enter your username: ")

    data = open_and_close_json_file('r')
    
    while attempts > 0:
        if username in data:
            password = input("Enter your password: ")
            if data[username] == password:
                print("Welcome, {}!".format(username))
                break
            else:
                attempts -= 1
                print("Incorrect password! {} attempts left".format(attempts))
        else:
            print("Username not found!")
            break

    if attempts == 0:
        print("You are locked out!")

def main():
    print("Welcome to the Password Protected Entry System!")
    print("1. New user")
    print("2. Existing user")
    choice = input("Please select an option: ")

    if choice == '1':
        new_user()
    elif choice == '2':
        existing_user()
    else:
        print("Invalid choice!")

if __name__ == "__main__":
    main()
