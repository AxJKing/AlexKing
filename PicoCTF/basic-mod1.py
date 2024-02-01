import string

numbers = [202, 137, 390, 235, 114, 369, 198, 110, 350, 396, 390, 383, 225, 258, 38, 291, 75, 324, 401, 142, 288, 397]

mod_37_numbers = []


for num in numbers:
    mod_37_numbers.append(num % 37)

char_set = string.ascii_uppercase + string.digits + '_'

mapped_chars = []

for num in mod_37_numbers:
    if num < 26:
        mapped_chars.append(char_set[num])
    elif num < 36:
        mapped_chars.append(str(num-26))
    else:
        mapped_chars.append('_')

joined_string = ''.join(mapped_chars)

print("PicoCTF{" + joined_string + "}")

