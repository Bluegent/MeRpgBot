# Classes
A class defines basic values, starting attributes as well as what skills a character of that class can use.

#### Header
```
    {
        "key":"shonen_protag",
        "name":"Shonen protagonist",
        "description":"Generic brand Naruto. Really good at punching things.",
```
* __[Note]__ Simple details about the class. Read [attributes.md](attributes.md) for more info.

#### Base Values
```
        "base_values":
        [   
            {"key":"BASE_DMG","value":10},
            {"key":"BASE_HP","value":100},
            {"key":"BASE_MP","value":50},
            {"key":"BASE_DEF","value":0}
        ],
```
* __[Integer]__ How much of every base value defined in [core.json](core.json) this class gets.
* __[Missing/Default]__ If a line for a specific key defined there is missing, default value is 0. Entire section can be missing.

#### Base attributes
```
        "basic_attributes":
        [   
            {"key":"STR","value":5},
            {"key":"INT","value":5},
            {"key":"AGI","value":5},
            {"key":"VIT","value":10}
        ],
```
Details:
* __[Integer]__ How much of each attribute this class gets at the start.
* __[Missing/Default]__ If a line for a specific key defined there is missing, default value is 0. Entire section can be missing.


#### Skills
```
       	"base_attack":"punch",
        "skills":
        [
            "big_punch",
            "beatdown",
            "powerup"
        ]
    }
```
* `"base_attack":"punch",`
	* __[String]__	The key of the skill used as the basic attack of this class. Also used for auto attacking.
* `"skills":[ "big_punch",`
	* __[String]__ Keys of skills (from skills.json) that are given to this class.
	* __[Missing/Default]__ The only skills this class will get is the basic attack.


Minimum Example:
```
    {
        "key":"shonen_protag",
		"base_attack":"punch"
    }
```