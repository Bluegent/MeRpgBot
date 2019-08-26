The core module is mostly used to defined the attributes, statistics and generally the numbers that are used by the engine
    
Base values.
Each row contains the key that the engine will use to identify this specific value in formulas. The names can contain anything except special characters reserved for formulas(see glossary.md).
The values themselves are defined by the class of the entity or the boss type.
```
	"base_values": [
		"BASE_DMG",
		"BASE_HP",
		"BASE_MP"
	],
```

Attributes.
Attributes are numbers that influence other numbers in the game.
You can add anything you want as an attribute, but bear in mind how this will affect the scaling of the game.
```
	"attributes": [{
			"key": "STR",
			"name": "Strength",
			"description": "Boosts physical abilities."
		},
        ...
	],
```
Details:
* `"key": "STR",` - an unique string that will be used to look up this attribute in scripts
* `"name": "Strength",` - A human readable name. This line can be removed and the name used for this attribute will be its key.
* `"description": "Boosts physical abilities.",` - A human readable description. This line can be removed and the engine won't give a description when asked about this attribute.
* The notes for "name" and "description" will apply to everything from this point onwards.

Level values.
Values used for the leveling up part of the game.
```
    "level_values":
    {
        "points_per_level":"1",
        "start_exp":"500",
        "exp_formula":"$PREV * 1.1 +20*2^(LEVEL/5)"
    }
```
Details:
* `"points_per_level":"1",` - how many attribute points are given with each level up
* `"start_exp":"500",` - experience points required to go from level 1 to level 2
* `"exp_formula":"$PREV * 1.1 +20*2^(LEVEL/5)"` -formula required to calculate how much experience is needed to go to the next level
    * $PREV - how many experience points were needed to level up for the previous level
    * LEVEL - your current level

Compound stats.
```
	"compound_stats": [{
			"key": "DEF",
			"formula": "BASE_DEF+STR/4",
			"name": "Physical Defense",
			"description": "Helps you avoid physical damage."
		},
        ...
    ]
```  
Details:      
* `"formula": "BASE_DEF+STR/4",` - the formula based on which the stat is calculated, in a MicroExpression format. For more info, read Functions.md
    * STR,BASE_DEF - Here we use keys defined previously, these will be resolved the amount of the attribute or base value the entity has when calculating the total value of the stat.
    * In our current case, the DEF stat is calculated as base defence + a quarter of the entity's strength attribute.
   


Other configuration items.
```
    "misc_config":
    {
        "revive_time":"1800",
        "revive_modifier":"0.5",
		"skill_threat":"5"
   
```
Details:
* `"revive_time":"1800",`- How long it takes for a character to be revived naturally, in seconds. 
* `"revive_modifier":"0.5"` - All a character's resources will be multiplied by this when revived. 
    * In our case, a character is always revived with half of their total HP and MP.
* `"skill_threat":"5"` - The default amount of threat a skill generates unless specificed otherwise.