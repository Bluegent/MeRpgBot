# Statuses
A status effect represents a mechanic that is applied to entities and does something to them periodically, such as increasing or decreasing an attribute, applying damage, stunning, etc.

Structure explanation:
```
    {   
        "key":"shonen_powerup",
        "name":"Shonen Powerup",
        "description":"You feel stronger, but somehow dumber.",
        "max_stack":"1",
        "interval":"0",
        "stacking":"refresh",
        "formula":"MOD_VALUE(STR,$0);MOD_VALUE(DEX,$1);MOD_VALUE(INT,$2);"
    }
```

Details:
* `"key":"shonen_powerup",` 
	* __[String]__ The name the engine will assign to this status. Used for functions like `APPLY_STATUS(...)` or `REMOVE_STATUS(...)`
* `"max_stack":1,` 
	* __[Formula][Integer]__ How many times this effect can be stacked. 
	* __[Keyword]__ `$1,$2,...` Numeric value placeholders. These are provided by `MOD_VALUE(...)`.
    * __[Note]__ A value of 0 or less means the effect can be stacked infinitely.
    * __[Default/Missing]__  1 - Only one status effect stack can be applied.
* `"interval":0,` 
	* __[Formula][Integer]__ How many seconds are elapsed between two status effect applications.
    * __[Default/Missing]__  1 - Status effect will tick every second.
    * __[Note]__ A value of 0 means the effect is applied ticked only once, used for effects like buffs and debuffs to stats.
    *  __[Note]__ `MOD_VALUE(...)` scripts only get applied once, at the start of the status and get removed when the status is removed. 
    *  __[Summary]__ Let's say we have a status effect that damages the afflicted, if interval is 10, the damage will be applied every 10 seconds for the duration of the effect.
* `"stacking":"refresh",`
	* __[String]__ Stacking type.
    	* `none` - The buff does not stack, so when it is applied while another stack of it exists on the target, nothing happens.
   		* `refresh` - The duration of existing stacks is refreshed, so if the buff lasts 60 seconds and 30 have elapsed, applying it again will refresh it to 60 seconds.
    	* `independent` - Status stacks have independent durations, so applying a new one will not affect the others.
    * __[Default/Missing]__ `indepdent`
* __[Note]__ A status duration is missing, as it is actually provided by the `MOD_VALUE(...)` function. See: [micro_expression.md](micro_expression.md) for more information.