A status effect represents a mechanic that is applied to entities and does something to them periodically, such as increasing or decreasing an attribute, applying damage, stunning, etc.
A status effect block looks like this:

```
    {   
        "key":"shonen_powerup",
        "name":"Shonen Powerup",
        "description":"You feel stronger, but somehow dumber.",
        "max_stack":"1",
        "interval":"0",
        "stacking":"refresh",
        "formula":"MOD_VALUE(STR,$0)~MOD_VALUE(DEX,$1)~MOD_VALUE(INT,$2)"
    }
```

Details:
* `"key":"shonen_powerup",` - The name the engine will assign to this status. Used for functions like APPLY_STATUS(...) or REMOVE_STATUS(...)
* `"max_stack":1,` - How many times this effect can be stacked. 
    * Note: A value of 0 or less means the effect can be stacked infinitely.
* `"interval":0,` - How many seconds are elapsed between the status effect is ticked again.
    * Note: A value of 0 means the effect is applied ticked only once, used for effects like buffs and debuffs to stats.
    * Note: Let's say we have a status effect that damages the afflicted, if interval is 10, the damage will be applied every 10 seconds for the duration of the effect.
* `"stacking":"refresh",` - Stacking type.
    * `none` - The buff does not stack, so when it is applied while another stack of it exists on the target, nothing happens.
    * `refresh` - The duration of existing stacks is refreshed, so if the buff lasts 60 seconds and 30 have elapsed, applying it again will refresh it to 60 seconds.
    * `independent` - Status stacks have independent durations, so applying a new one will not affect the others.
