
%take input diameter
diam = input('Enter hemisphere diameter in meters: ');

%calculate volume and surface area based on formulas
r = diam / 2;
v = (2 / 3) * pi * r^3;
sa = 3 * pi * r^2;

%calculate mass of the copper core, gold plating, and the total mass
massC = 8960 * v;
massG = 0.0185 * sa;
massT = massC + massG;

%calculate the cost and round the result to 2 decimal places
cost = (10.32 * massC) + (122107 * massG);
cost = round(cost, 2);

%output results and safety warning
disp('The mass of your sphere will be ' + string(massT) + ' kg.')
disp('The cost of your sphere will be $' + string(cost))
disp('Please do not actually try this at home, but if you do, wear PPE!')