#Tanner FitzGerald
#9/2/24
#Personal Practice Project
'''

This program will convert the input string into an encrypted "Atbash" Cipher
version.(A->Z and B->Y etc.)

capitalization will remain the same

non alphabetic letters will also remain the same

'''

alphabet = ['a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z']

plain_string = input("Enter your plaintext: ")
cipher_list = []

for i in range(len(plain_string)):
    
        is_capital = plain_string[i].isupper()       #setting all letters in given string to lowercase
        if is_capital == True:
            plain_chr = plain_string[i].lower()
        else:
            plain_chr = plain_string[i]
            
            
        if plain_chr not in alphabet:                #accepting any non-letter characters
            cipher_list.append(plain_chr)
        else:                                        #encrypting any letters
            cipher_chr = alphabet[-(alphabet.index(plain_chr)+1)]
            if is_capital == True:                   #converting any letters back to uppercase if needed
                cipher_list.append(cipher_chr.upper())
            else:
                cipher_list.append(cipher_chr)
            
cipher_string = ''.join(cipher_list)                 #combining the list into a string
print(cipher_string)
            