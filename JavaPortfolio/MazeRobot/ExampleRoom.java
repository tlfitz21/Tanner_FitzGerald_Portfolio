/*Tanner FitzGerald
12/4/24
CSC 222 MiniTermProject

This program will create objects of the Robot class, also in this folder, which use the DoorRoom class to
create a maze, and place a robot inside it. 

After each object creation, the program contains an algorithm to have the robot solve the maze, using its
.check, .go, and .say methods. 

The first part uses an algorithm that hard codes the solution of maze 1

The second part uses an algorithm that lets the robot detect how to solve it, using mazes 1-3, which have only
one solution

The third part uses an algorithm that allows the robot to solve the maze automatically, given any type of maze.
examples of maze 4 and 5, which don't only have one solution, are provided, as well as an example where a new
maze could be added for it to solve. 
 
 */
import java.util.*;

public class ExampleRoom {
	public static void main(String[] args) {
		
		/* Part 1.)
		Robot robot = new Robot("xxx \n  x \n  xx", 0, 0);
		robot.go('E');
		robot.go('E');
		robot.go('S');
		robot.go('S');
		robot.go('E');
		robot.say("Won!");
		*/

		/* Part 2.)
  			
		--------------maze 1:

		Robot robot = new Robot("xxx \n  x \n  xx", 0, 0);
		char lastDir = '0';
		while (!(robot.getX() == 3 && robot.getY() == 2)){
			if (robot.check('N') && lastDir != 'N'){
				robot.go('N');
				lastDir = 'S';
			}

			else if (robot.check('E') && lastDir != 'E'){
				robot.go('E');
				lastDir = 'W';
			}

			else if (robot.check('S') && lastDir != 'S'){
				robot.go('S');
				lastDir = 'N';
			}

			else if (robot.check('W') && lastDir != 'W'){
				robot.go('W');
				lastDir = 'E';
			}

			else {
				robot.say("I'm Stuck!");
				System.exit(0);
			}			
		}
		robot.say("I won!");

		--------------maze 2.)
		
		Robot robot = new Robot("xxxxx\n    x\n xxxx\n x   \n xxxx", 0, 0);
		char lastDir = '0';
		while (!(robot.getX() == 4 && robot.getY() == 4)){
			if (robot.check('N') && lastDir != 'N'){
				robot.go('N');
				lastDir = 'S';
			}

			else if (robot.check('E') && lastDir != 'E'){
				robot.go('E');
				lastDir = 'W';
			}

			else if (robot.check('S') && lastDir != 'S'){
				robot.go('S');
				lastDir = 'N';
			}

			else if (robot.check('W') && lastDir != 'W'){
				robot.go('W');
				lastDir = 'E';
			}

			else {
				robot.say("I'm Stuck!");
				System.exit(0);
			}			
		}
		robot.say("I won!");

		--------------maze 3.)

		Robot robot = new Robot("xxxxx\n    x\nxxx x\nx xxx\nx    \nxxxxx", 0, 0);
		char lastDir = '0';
		while (!(robot.getX() == 4 && robot.getY() == 5)){
			if (robot.check('N') && lastDir != 'N'){
				robot.go('N');
				lastDir = 'S';
			}

			else if (robot.check('E') && lastDir != 'E'){
				robot.go('E');
				lastDir = 'W';
			}

			else if (robot.check('S') && lastDir != 'S'){
				robot.go('S');
				lastDir = 'N';
			}

			else if (robot.check('W') && lastDir != 'W'){
				robot.go('W');
				lastDir = 'E';
			}

			else {
				robot.say("I'm Stuck!");
				System.exit(0);
			}			
		}
		robot.say("I won!");

		*/

		/* Part 3.)

		--------------maze 4

		Robot robot = new Robot("xxxx\nxx xxx\nx  x\nx xxxx", 0, 0);

		char lastDir = 'E';
		while (!(robot.getX() == robot.room.cols -1 && robot.getY() == robot.room.rows -1)){

			if (lastDir == 'E'){
				if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}
			

			else if (lastDir == 'S'){
				if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}

			else if (lastDir == 'W'){
				if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}

			else if (lastDir == 'N'){
				if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}
		}
		robot.say("I won!");

		--------------maze 5

		Robot robot = new Robot("xx xx  xxx\n"
                +        "xx x    x \n"
                +        "x   xxxxx \n"
                +        "xx xx x xx\n"
                +        " xxxx x x \n"
                +        "     xxx x\n"
                +        "xxxxx x xx\n"
                +        "xxxxxxxxxx\n"
                +        "xx    x   \n"
                +        "x xxxxxxxx\n", 0, 0);

		char lastDir = 'E';
		while (!(robot.getX() == robot.room.cols -1 && robot.getY() == robot.room.rows -1)){

			if (lastDir == 'E'){
				if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}
			

			else if (lastDir == 'S'){
				if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}

			else if (lastDir == 'W'){
				if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}

			else if (lastDir == 'N'){
				if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}
		}
		robot.say("I won!");

		--------------for any other maze

		Robot robot = new Robot(//Enter maze parameter here);

		char lastDir = 'E';
		while (!(robot.getX() == robot.room.cols -1 && robot.getY() == robot.room.rows -1)){

			if (lastDir == 'E'){
				if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}
			

			else if (lastDir == 'S'){
				if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}

			else if (lastDir == 'W'){
				if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}

			else if (lastDir == 'N'){
				if (robot.check('W')) {
					robot.go('W');
					lastDir = 'W'; }
				else if (robot.check('N')) {
					robot.go('N');
					lastDir = 'N'; }
				else if (robot.check('E')) {
					robot.go('E');
					lastDir = 'E'; }
				else if (robot.check('S')) {
					robot.go('S');
					lastDir = 'S'; }
				else {
					robot.say("I'm Stuck!");
					System.exit(0); }
			}
		}
		robot.say("I won!");
		*/
	}

}
