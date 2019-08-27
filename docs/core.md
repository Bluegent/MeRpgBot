# Core
The core module is mostly used to define configuration values for the engine.

```
{
  "revive_time":"1800",
  "skill_threat":5,
  "attribute_points_per_level":1,
  "start_exp":500,
  "exp_formula":"$PREV * 1.1 +50*2^($LEVEL/5)",
  "max_level":100
}
   
```
* `"revive_time":"1800",` 
	* __[Formula]__ How much time, in seconds, must pass before a player is automatically revived.
    * __[Time]__ Format is also supported ("1000h10m10s").
* `"skill_threat":"5",`
	* __[Integer]__ How much threat a skill will generate by default. See [__boss.md__](boss.md) for more information.
* `"attribute_points_per_level":1,`
	* __[Integer]__ How many attribute points are given each time a player levels up. See [__intro.md__](intro.md) for more information.
*  `"start_exp":500,`
	* __[Integer]__ How much experience is needed to get to level 2. See [__intro.md__](intro.md) for more 
* `"exp_formula":"$PREV * 1.1 +50*2^($LEVEL/5)"` 
	* __[Formula][Integer]__ How to calculate experience needed for leveling up.
	* __[Keyword]__ `$PREV` - Experience necessary for the previous level.
	* __[Keyword]__ `$LEVEL` - Current player level.
	* __[Summary]__ Assuming our player is level 10, and it took 1000 experience to get from level 9 to 10, then to get from level 10 to 11, they would need `1000 * 1.1 + 50 * 2^2 = 1300` experience points.
* "max_level":100
	* __[Integer]__ Maximum level.
	* __[Missing/Default]__ No maximum level, players can keep leveling forever. Technical limit is actually around 2 billions. Equivalent to `"max_level":0`.
	* __[Note]__ If the value is negative, that means leveling is disabled.