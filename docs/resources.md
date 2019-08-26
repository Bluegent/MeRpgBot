# Resources
While the name is a bit misleading, these are the resources used for using skills, casting spells and so on. Some examples would be mana, stamina, rage and so on. \
Important Note: `"HP"` is an engine-required resource, it is necessary for the normal functioning of RPGBot.  While HP itself needs to be present, the formulas for calculating the maximum value, regeneration and so on are up to you. \
Struncture explanation: 
```		
	{
		"key": "HP",
		"formula": "BASE_HP+VIT*20",
        "interval":"5",
		"regen":"15+VIT/1000",
		"name": "Health",
		"description": "Your health pool. You die if it drops to 0."
		"start_percentage":"1"
	},
	...
```
Details:
* `"formula": "BASE_HP+VIT*20",` 
	* __[Formula]__ _Maximum_ value of this resource.
* `"interval":"5",` 
	* __[Formula]__ How often the regeneration happens, in seconds.
    * __[Default/Missing]__ The regeneration happens once per second. Also applies if value is less than 1.
* `"regen":"15+VIT/1000",`
	* __[Formula]__ How much of this resource regenerates every interval.
    * __[Default/Missing]__ The resource does not regenerate by itself.
    * __[Note]__ The value can be negative as well, meaning the resource goes down as time passes.
    * __[Summary]__ We regenerate `15+1000th of the VIT attribute` every 5 seconds.
* `"start_percentage":"1",` 
	* __[Formula]__ How mouch of the resource is present on revive/spawn. 
	* __[Summary]__ A resource like Health starts at 100%. 
	* __[Note]__ The value is given from 0 to 1, i.e. 0.2 = 20%.


Minimum example:

```		
	{
		"key": "HP",
		"formula": "BASE_HP+VIT*20",
	},
	...
```