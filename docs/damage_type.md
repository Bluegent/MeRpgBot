Damage Types. \
While most aspects of damage types are optional, at least one is required for a functional game as the HARM function takes a damage type as a paramater. \

Struncture explanation: \
```
	{
		"key": "P",
		"mitigation": "NON_NEG($V-GET_PROP($TARGET,DEF))",
		"dodge":"GET_PROP($T,DEX)/1000",
		"crit":"GET_PROP($S,DEX)/1000",
		"crit_multiplier":"3",
		"name": "Physical",
		"description": "Physical damage, mitigated by a target's DEF attribute."
	},
	...
    ]
```
* `"mitigation": "NON_NEG($V-DEF)",` - The formula for modifying damage.
    * If this line is missing, the full amount of damage will be dealt.
* ` "dodge":"GET_PROP($TARGET,DEX)/1000",` - Formula to calculate dodge chance or this damage type . Lowest chance taken into account is .1%, amount given is percentage. (EG: 100 means you always dodge)
* `"crit":"GET_PROP($SOURCE,DEX)/1000",` - Formula to calculate critical chance for this damage type. 
    *if "dodge" or "crit" are missing, default values are 0.
* `"crit_multiplier":"3",` - The mutliplier for crit damage.
    * If this line is missing, default crit multiplier is 2.
    * Damage is multiplied _BEFORE_ mitigation.
* In this example case, physical damage is calculated as a flat deduction from DEF, however the value cannot be lower than 0.
\

Minimum example : \

```
	{
		"key": "P",
	}
```