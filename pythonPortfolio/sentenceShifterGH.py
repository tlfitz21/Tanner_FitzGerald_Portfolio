#Tanner FitzGerald
#9/5/24
#Personal Practice Project
'''

This program contains a function that will take an input sentence and shift the
first letter of every word to the 1st position of the next word.

the last word will shift its 1st letter back around to the first word

"sentences" with a single word will remain unchanged

numbers and special characters will be treated the same way as letters here

'''

def sentenceShifter():
    user_sentence = input("Enter your sentence: ")
    user_words = user_sentence.split()                  #creates a list of strings
    first_letters = []  
    new_words = []    
    if len(user_words) == 1:
        return user_sentence
    else:
        for i in range(len(user_words)):
            first_letters.append(user_words[i][0])
        new_words.append(first_letters[-1] + user_words[0][1:])
        for i in range(len(user_words) - 1):
            new_words.append(first_letters[i] + user_words[i+1][1:])
        return ' '.join(new_words)

print(sentenceShifter())