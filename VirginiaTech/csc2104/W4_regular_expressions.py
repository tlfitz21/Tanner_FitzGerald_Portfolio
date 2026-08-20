import re

#challenge number 1
def contains_only_alphanumeric(string):
    print(string)
    if re.search('[^a-zA-Z0-9]', string) or len(string) == 0:
        return False
    else:
        return True


#challenge number 2
def find_literals(substring, string):
    if re.search(substring, string):
        return True
    else:
        return False
    
#challenge number 3
def remove_parentheses(string):
    result = re.sub("\s*\(.*\)", '', string)
    return result

#challenge number 4
def match_words(string):
    
    my_list = re.findall(r'\b[P]\S*\b', string)
    
    if len(my_list) == 2:
        return True
    else:
        return False
    




