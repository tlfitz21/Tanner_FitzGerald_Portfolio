'''
Tanner FitzGerald
tlfhokie@vt.edu
9/19/25

This program will solve the "wannabe palindrome" problem. The goal is to identify that largest possible substring of a given sring that counts as a wannabe palindrome.
a wannabe palindrome is like a normal palindrome, where the "word" reads the same forward as it does backwards, excpet it allows for 0 or 1 mismatches. 
Mismatches can either be treated as a mitchmatch between both character, where both are not considered to count, or as a deletion, where one it would be a palindrome if just one character was removed

I did not use LLMs to to help do this assignment

'''
def wannabe_palindrome(s):
    #print("now testing " + s)

    longest_length = 1                          #here we check every UNIQUE character in the string
    checked = []
    for i in range(len(s)):
        if s[i] in checked:
            continue
        checked.append(s[i])
        
        if len(s) - i < longest_length:         #only check potential palindromes if they would be larger than the current largest
            break
        for j in range(len(s) - 1, i, -1):      #compare the "i" character with every character between it and the end of the string
            strike = 0
            rdeletion = 0
            ldeletion = 0
            broken = False
            if s[i] == s[j]:                    #make sure to only check cases that start and end with the same character, and do the palindrome check within it as to not track those characters
                length = j - i
                if length < longest_length:     #only check potential palindromes if they would be larger than the current largest
                    break
                                                #now we check if it's a wannabe palindrome
                for k in range(i + 1, ((i + j) // 2) + 1, 1):
                    #print("I: " + str(i) + " J: " + str(j) + " K: " + str(k))
                    if s[k + ldeletion] != s[(j) - (k-i) - rdeletion]:         
                        if strike == 1:                     #stop checking this potential palindrome after the second strike
                            #print("second strike")
                            broken = True
                            break
                        #print("first strike")
                        if s[k] == s[j - k - 1]:                #if only the first strike, determine how to treat it depending on if it's a mismatch or deletion
                            #print("right deletion of " + s[j-k])
                            rdeletion = 1    #does right deletion work?
                           
                        elif s[k+1] == s[j-k]:
                            #print("left deletion of " + s[k])
                            ldeletion = 1            #does left deletion work?

                        #else:
                            #print("mismatch between " + s[k] + " and " + s[j-k])  

                        strike += 1
                        continue
                
                if broken:
                    continue
                else:
                    longest_length = j - i + 1
                    #print("length record now at " + str(longest_length))
                    break
    return longest_length
                





'''

print(wannabe_palindrome("abcba"))
print()
print(wannabe_palindrome("abcbda"))
print()
print(wannabe_palindrome("abcd"))
print()
print(wannabe_palindrome("abbad"))
print()
print(wannabe_palindrome("aabdcbaakkk"))
print()
print(wannabe_palindrome("zzz 123 3251 zzz"))
print()
print(wannabe_palindrome("aabacaaa"))
print()
print(wannabe_palindrome("kkkdaabaa"))

expected outputs: 5,6,1,4,8,16,8,5
'''
