%establish input variables
initLevel = input('Enter the inital blood glucose level in mg/dl: ');
newLevel = input('Enter the new blood glucose level in mg/dl: ');

%if the magnitute of their difference is within a safe range, output so
if abs(newLevel - initLevel) < 15
    disp("No signifigant change in blood glucose levels detected.")

%otherwise, output which direction the signifigant chang is in
elseif newLevel - initLevel < 0 
    disp("Signifigant drop in blood glucose level detected.")
else
    disp("Signifigant increase in blood glucose level detected.")
end

%display warnings if the new level is outside of safe range
if newLevel > 250
    disp("WARNING: blood glucose level exceeds safe range for exercise.")
end

if newLevel <= 70
    disp("WARNING: blood glucose level below safe range for exercise.")
end