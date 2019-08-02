A class defines basic values, starting attributes as well as what skills a character of that class can use.

Header.\
```
    {
        "key":"shonen_protag",
        "name":"Shonen protagonist",
        "description":"Generic brand Naruto. Really good at punching things.",
```
Details:
* Simple details about the class. Read core.md for more info.

Base Values.\
```
        "base_values":
        [   
            {"key":"BASE_DMG","value":"10"},
            {"key":"BASE_HP","value":"100"},
            {"key":"BASE_MP","value":"50"},
            {"key":"BASE_DEF","value":"0"}
        ],
```
Details:
* How much of every base value defined in core.json this class gets.
* If a line for a specific key defined there is missing, default value is 0.
* These values are all numeric. You can use MicroExpression formulas, but that's discouraged.

Base attributes.\
```
        "basic_attributes":
        [   
            {"key":"STR","value":"5"},
            {"key":"INT","value":"5"},
            {"key":"AGI","value":"5"},
            {"key":"VIT","value":"10"}
        ],
```
Details:
* How much of each attribute this class gets at the start.
* If a line for a specific key defined there is missing, default value is 0.
* These values are all numeric. You can use MicroExpression formulas, but that's discouraged.

Skills.\
```
        "skills":
        [
            "punch",
            "big_punch",
            "beatdown",
            "powerup"
        ]
    }
```
Details:
* Keys of skills (from skills.json) that are given to this class.
* The first one in the list is considered the class' basic attack and will be used for their auto-attack option.