# Damage Types
While most aspects of damage types are optional, at least one is required for a functional game as the HARM function takes a damage type as a paramater. \
Struncture explanation: 
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
* `"mitigation": "NON_NEG($V-DEF)",` 
	* __[Formula]__ - How damage is mitingated.
    * __[Default/Missing]__ - The full amount of damage will be dealt.
    * __[Keyword]__ `$V` - Represents the amount of damage that would be received before mitigation.
* ` "dodge":"GET_PROP($TARGET,DEX)/1000",` 
	* __[Formula]__ - How to calculate dodge chance for this damage type . Lowest chance taken into account is .1%, amount given is percentage. (eg.: 100 means you always dodge)
	* __[Default/Missing]__ - The damage type cannot be dodged.
* `"crit":"GET_PROP($SOURCE,DEX)/1000",` - 
	* __[Formula]__ How to calculate critical chance. Similar to dodge chance.
    * __[Default/Missing]__ - The damage type cannot deal critical damage.
* `"crit_multiplier":"3",`
	* __[Formula]__ The mutliplier for critical damage.
    * __[Default/Missing]__ Default value is 2 or 200% damage.
    * __[Note]__ Damage is multiplied _BEFORE_ mitigation.
* __[Summary]__ Physical damage is calculated as a flat deduction from DEF, however the value cannot be lower than 0.

Minimum example : 

```
	{
		"key": "P"
	}
```