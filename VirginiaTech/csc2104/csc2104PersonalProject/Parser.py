import json

#cleanly open and read the dataset
with open('PokeData.json', 'r') as file:
    data = json.load(file)

#create the first parts of the html file, up until the actual data that gets put in
print('''<!DOCTYPE html>
<html lang="en">
    
<head>
    <style>
        .collasible{
            background-color: #eee;
            color: #444;
            cursor: pointer;
            padding: 18px;
            width: 100%;
            border: none;
            text-align: left;
            outline: none;
            font-size: 15px;      
        }
        .active, .collasible:hover{
            background-color: #ccc;
        }
        .content{
            padding: 0 18px;
            display: none;
            overflow: hidden;
            background-color: #f1f1f1;
        }
        table
        {
            margin:auto;
            text-align: center;
        }
    </style>
</head>

<body>
    <h1 style="text-align: center;">Sinnoh Pokedex Tool</h1>
    <table style="border: 3px solid black; border-collapse: collapse;">
        <thead>
            <tr>
                <th style="border: 3px solid black; border-collapse: collapse;">Pokedex Number</th>
                <th style="border: 3px solid black; border-collapse: collapse;">Name</th>
                <th style="border: 3px solid black; border-collapse: collapse;">Sprite</th>
                <th style="border: 3px solid black; border-collapse: collapse;">Details</th>
            </tr>
        </thead>
        <tbody>''')

#this for loop will print all of the data
for pokemon in range(len(data)):

    #establish the Moves string to print it cleanly later. includes line breaks
    firstMove = next(iter(data[pokemon]["Moves"]))
    Moves = ""
    for currMove in data[pokemon]["Moves"]:
        if currMove == firstMove:
            Moves += currMove + ": " + data[pokemon]["Moves"][currMove]
        else:
            Moves += "<br>" + currMove + ": " + data[pokemon]["Moves"][currMove]

    #establish the Types string to print it cleanly later. includes line breaks
    firstType = next(iter(data[pokemon]["TypeEffectiveness"]))
    Types = ""
    for currType in data[pokemon]["TypeEffectiveness"]:
        if currType == firstType:
            Types += currType + ": " + data[pokemon]["TypeEffectiveness"][currType]
        else:
            Types += "<br>" + currType + ": " + data[pokemon]["TypeEffectiveness"][currType]

    #Print each row to fill in data for that pokemon. the row includes 4 data entries: the pokedex number, the name, the pokemon's sprite, and a collapsable table for more detailed information
    print("<tr>")
    print("    <td style=\"border: 3px solid black; border-collapse: collapse;\">" + data[pokemon]["Basic Info"]["PokedexNumber"] + "</td>")
    print("    <td style=\"border: 3px solid black; border-collapse: collapse;\">" + data[pokemon]["Basic Info"]["Name"] + "</td>")
    print("    <td style=\"border: 3px solid black; border-collapse: collapse;\">" + "<img src=" + data[pokemon]["Images"]["Sprite"] + ".png" + " alt=\"Sprite of " + data[pokemon]["Basic Info"]["Name"] + "\">" "</td>")
    print("    <td style=\"border: 3px solid black; border-collapse: collapse;\">")
    #this is where the button to expand and collapse the table is
    print("        <button type=\"button\" class=\"collapsible\">Expand</button>")
    print("        <div class=\"content\">")
    #this table includes 2 rows of 2 data entires each
    print("        <table>")
    print("            <tbody>")
    #this row contains the pokemon's full image, and a paragraph of text detailing its basic information
    print("                <tr>")
    print("                    <td><img src=" + data[pokemon]["Images"]["Full"] + ".png" + " alt=\"Picture of " + data[pokemon]["Basic Info"]["Name"] + "\" width= \"250\" height= \"250\"><br>\
          <p><b><u>Basic Information</b></u><br>\
        Name: " + data[pokemon]["Basic Info"]["Name"] + "<br>\
        Classification: " + data[pokemon]["Basic Info"]["Classification"] + "<br>\
        Typing: " + data[pokemon]["Basic Info"]["Typing"] + "<br>\
        Height: " + data[pokemon]["Basic Info"]["Height"] + "<br>\
        Weight: " + data[pokemon]["Basic Info"]["Weight"] + "<br>\
        Egg Groups: " + ", ".join(data[pokemon]["Basic Info"]["EggGroups"]) +  "<br>\
        Catch Rate: " + data[pokemon]["Basic Info"]["CatchRate"] + "<br>\
        EV Yield: " + data[pokemon]["Basic Info"]["EVYield"] + "<br>\
        Abilities: " + ", ".join(data[pokemon]["Basic Info"]["Abilities"]) +  "<br>\
        Evolves From: " + data[pokemon]["Basic Info"]["EvolvesFrom"] + "<br>\
        Evolves Into: " + data[pokemon]["Basic Info"]["EvolvesInto"] + "\
            </p></td>")
    print("                    <td><p><b><u>Base Stats</b></u><br>\
        Base HP: " + str(data[pokemon]["Basic Info"]["BaseStats"]["HP"]) + "<br>\
        Base Attack: " + str(data[pokemon]["Basic Info"]["BaseStats"]["Attack"]) + "<br>\
        Base Defense: " + str(data[pokemon]["Basic Info"]["BaseStats"]["Defense"]) + "<br>\
        Base Special Attack: " + str(data[pokemon]["Basic Info"]["BaseStats"]["SpecialAttack"]) + "<br>\
        Base Special Defense: " + str(data[pokemon]["Basic Info"]["BaseStats"]["SpecialDefense"]) + "<br>\
        Base Speed: " + str(data[pokemon]["Basic Info"]["BaseStats"]["Speed"]) + "<br>\
        Base Stat Total: " + str(data[pokemon]["Basic Info"]["BaseStats"]["Total"]) + "<br>\
            </p></td>")
    print("                </tr>")
    #this row contains the Moves and Types strings created earlier
    print("                <tr>")
    print("                    <td><p><b><u>Moves Learned</b></u><br> " + Moves + "</p></td>")      
    print("                    <td><p><b><u>Type Effectiveness</b></u><br> " + Types + "</p></td>")
    print("                </tr>")
    print("            </tbody>")
    print("        </table>")
    print("        </div>")
    print("    </td>")
    print("</tr>")

#now(outside of the loop) I print the final closing tags for the html file. I also have a script that handles the button to expand and collapse the extra details
print('''</tbody>
    </table>

    <script>
        var coll = document.getElementsByClassName("collapsible");
        var i;

        for (i = 0; i < coll.length; i++) {
        coll[i].addEventListener("click", function() {
            this.classList.toggle("active");
            var content = this.nextElementSibling;
            if (content.style.display === "block") {
            content.style.display = "none";
            } else {
            content.style.display = "block";
            }
        });
        }  
    </script>

</body>

</html>''')
















