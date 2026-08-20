import sqlite3
import sys
from os.path import exists

inYear = sys.argv[1]

if exists('cs_course_scheduling.sqlite'):
    conn = sqlite3.connect('cs_course_scheduling.sqlite')

    cursor = conn.execute("SELECT first_name, last_name, academic_year FROM students WHERE academic_year = ?", [inYear])
    for row in cursor:
        print("First Name = " + row[0])
        print ("Last Name = " + row[1])
        print ("Academic Year = " + str(row[2]))
        print("----------------------------------")


   
    conn.close()
    print("Database was accessed and closed")

else:
    print('Database file cs_course_scheduling.sqlite not found in current working directory.')