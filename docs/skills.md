
# Skills
Skills define how an entity interacts with another entity, like how a player damages a boss or vice versa.

#### Header
```
    {   
        "key":"punch",
        "name":"Basic Punch",
        "aliases":["punch","attack"],
        "description":"Apply fist to target's face.",
        "type":"cast",
```

* ` "aliases":["punch","attack"],` - 
	* __[String]__ What the skill can be referred to as when a player types in `cast <attack>`. 
	* __[Note]__ All skill aliases need to be unique to avoid conflicts. 
	* __[Default/Missing]__ The key will be used as an alias.
* `"type":"cast",` 
	* __[String]__ The type of the skill.
   		* `cast` - The action happens after the skill is done casting.
    	* `channel` - The action happens repeatedly while the caster keeps channelling the skill, up to a duration.
    * __[Default/Missing]__ If this line is missing, type is assumed to be `cast`.
    
#### Values by level.
Skills can be levled up at trainer NPCs (TBD), therefore these blocks provide the stats for the different levels of the skill.
```
        "values_by_level":
        [
            {
                "cooldown":"30/GET_PROP($S,ATT_SPEED)",
                "formula":"HARM($T,$S,P,GET_PROP($S,B_DMG)+10+GET_PROP($S,STR)/2)",
                "needed_level":1
            },
            ...
        ]
```
* `"cooldown":"30/GET_PROP(CASTER,ATT_SPEED)",` 
	* __[Formula][Integer/Time]__ How long it takes before the skill can be used again, in seconds.
    * __[Missing/Default]__ The skill has 1 second of cooldown.
* `"formula":"HARM($T,$S,P,GET_PROP($S,B_DMG)+10+GET_PROP($S,STR)/2)",`
	* __[Formula]__ The script that details what the skill does.
    * __[Summary]__ "Basic Punch" harms the caster's target for `the caster's Base damage + 10 + half of the caster's strength` Damage type is Physical.
* `"needed_level":1` 
	* __[Integer]__ At what level the skill can be purchased from the trainer NPC.
	* __[Missing/Default]__ 1

#### More detailed skill
```
        "key":"basic_heal",
        "name":"Inspiration",
        "aliases":["heal","mend"],
        "description":"Remember the words of your mentor and feel better.",   
        "type":"cast",
        "values_by_level":
        [
            {
                "cooldown":"300",
                "duration":"10",
                "push_back":true,
                "interrupt":true,
                "formula":"HEAL($T,$S,10+GET_PROP($S,INT)*5)",
                "needed_level":5,
                "cost":{"key":"MP","value":"10"},
				"threat":"5"
            },
            ...
```
* ` "duration":"10"` 
	* __[Formula][Integer/Time]__  How long it takes to cast this skill, in seconds. 
	* __[Note]__ For channeling type skills, how long the channeling is executed for.
	* __[Missing/Default]__ Skill is casted instantly. Negative values apply the same.
* ` "push_back":true,` 
	* __[Formula][Boolean]__ If it's possible to be pushed-back while casting this skill.
	* __[Missing/Default]__ `true`
    * __[Summary]__ If you are pushed back for 6 seconds while casting "Inspiration", your total cast time will be 16 seconds.
    * __[Note]__ For channelling type spells, push-back works in reverse, expending your duration. If channelling a spell that lasts 10 second, and and you are pushed-back 2 seconds, instead of 10 seconds, the spell will channel for 8 seconds.
    * __[Note]__ A value of `false` means the cast/channel of this skill will never be pushed back.
* `"interrupt":"true",`
	*  __[Formula][Boolean]__ Whether the skill can be interrupted by interrupt-type attacks. 
    * __[Missing/Default]__ `true`
* `"threat":"5"`
	* __[Formula][Integer]__ How much threat this level of this skill generates before threat multiplier is taken into account.
	* __[Missing/Default]__ The value given in core.json.
* `"cost":{"key":"MP","value":"10"},` 
	* __[String/Integer/Formula]__ How much of a resource casting this skill costs. 
	* __[Default/Missing]__ It is assumed the skill costs 0 HP to cast.
 (HP is used because it is the only guaranteed resource of the engine)



#### Channeling skills
```
        "key":"beatdown",
        "name":"Consecutive Normal Punches",
        "aliases":["flurry","beatdown","combo"],
        "description":"Punch the shit out of your target.",  
        "type":"channel",
        "values_by_level":
        [
            {
                "cooldown":"1200",
                "duration":"60",
                "formula":"HARM($T,$S,P,GET_PROP($S,B_DMG)+GET_PROP($S,STR)/2)",
                "needed_level":15,
                "interval":"1+NON_NEG(9-DEX/20)"
            },
```
* `"interval":"1+NONNEG(9-DEX/20)"` 
	* __[Formula/Integer/Time]__ The interval between two applications of the skill, in seconds.
	* __[Missing/Default]__ Skill is applied every second.
    * __[Summary]__ `Consecutive Normal Punches` deals damage every 10 seconds if the caster's dexterity is 0. 1 second is shaved off with every 20 points of dexterity, until. at 180 DEX, the damage instance happens once every second for the 60 second duration.


Minimum Example:
```
        "key":"basic_heal",
        "values_by_level":
        [
            {
                "formula":"HEAL($T,$S,10+GET_PROP($S,INT)*5)",
            }
        ]
        ...
```