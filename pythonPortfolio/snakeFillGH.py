#Tanner FitzGerald
#8/27/24
#Personal Practice Project
'''

This program will simulate a "snake" videogame somewhat, and contains a function
that will take a variable length of a game board size, and retrun the number of
times the snake can eat(and double its length) before it runs out of area. 

The snake starts at length 1.

'''

def snakefill(side_length):
    area = side_length * side_length
    snake = 1
    apples = 0
    while snake < area:
        snake *= 2
        apples += 1
    return str(apples - 1)

print(snakefill(int(input("Enter the length of one side of the game board "))))