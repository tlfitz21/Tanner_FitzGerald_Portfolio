#Tanner FitzGerald
#10/29/24
#Personal Practice Project
'''

This program will check interview results and decide if the candidate was qualified or not.
It will receive a list of times for each question and the total time taken by a candidate,
then it will check to see if each time abides by the time limits

The criteria for qualification are answering all 8 questions within their varying time limits,
and not exceeding a total interview time of 120 minutes. There are 2 very easy questions with a
time limit of 5 minutes, 2 easy questions with a time limit of 10 minutes, 2 medium questions with
a time limit of 15 minutes, and 2 hard questions with a time limit of 20 minutes.

'''
class Interview:
    def __init__(self,name, time_list, total_time):
        self.time_list = time_list
        self.total_time = total_time
        self.name = name
    
    def qualify(self):
        qualified = "Qualified"
        #a dictionary to keep track of which criteria, if any, disqualifies the
        #candidate
        criteria = {'crit1' : True,
                    'crit2' : True,
                    'crit3' : True,
                    'crit4' : True,
                    'crit4' : True,
                    'crit5' : True,}
       #checks each criteria for qualification 
        if len(self.time_list) != 8:
            return self.name + " is disqualified.\nDid not complete all of the questions."
        if self.total_time > 120:
            qualified = "Disqualified"
            criteria['crit1'] = False
        if int(self.time_list[0]) > 5 or int(self.time_list[1]) > 5:
            qualified = "Disqualified"
            criteria['crit2'] = False
        if int(self.time_list[2]) > 10 or int(self.time_list[3]) > 10:
            qualified = "Disqualified"
            criteria['crit3'] = False
        if int(self.time_list[4]) > 15 or int(self.time_list[5]) > 15:
            qualified = "Disqualified"
            criteria['crit4'] = False
        if int(self.time_list[6]) > 20 or int(self.time_list[7]) > 20:
            qualified = "Disqualified"
            criteria['crit5'] = False
        
        if qualified == "Qualified":
            return self.name + " is qualified."
        else:
            strings = [' is disqualified.\n']
            #setting up description as to why candidate was disqualified
            if criteria['crit1'] == False:
                strings.append("Exceeded total interview time limit of 120 minutes\n")
            if criteria['crit2'] == False:
                strings.append("Exceeded time limit for at least 1 very easy question\n")
            if criteria['crit3'] == False:
                strings.append("Exceeded time limit for at least 1 easy question\n")
            if criteria['crit4'] == False:
                strings.append("Exceeded time limit for at least 1 medium question\n")
            if criteria['crit5'] == False:
                strings.append("Exceeded time limit for at least 1 hard question\n")
            disqualified = ''.join(strings)
            return self.name + disqualified
        
Candidate = Interview(input("Enter candidate's name: "),
                      input("Enter the question times seperated by commas: ").split(','),
                      int(input("Enter total interview time: ")))
print(Candidate.qualify()) 
       
        
        
        
        
    