%initial input
concentration = input('Enter methane concentration in ppm: ');

%incorrect input handling
if concentration < 0
    disp('Error, input cannot be below zero.')


%normal input handling
elseif concentration < 5000
    disp('necessary ventilation rate: 15 m^3/s.')
elseif concentration < 15000
    disp('necessary ventilation rate: 35 m^3/s.')
elseif concentration < 35000
    disp('necessary ventilation rate: 70 m^3/s.')
elseif concentration < 50000
    disp('necessary ventilation rate: 120 m^3/s.')


%exceessive concentration levels 
else
    disp('necessary ventilation rate: 200 m^3/s.')
    disp('WARNING: methane concentration levels exceed threshold and pose explosion risk, evacuate immediately!')
end

