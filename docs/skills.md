Skills define how an entity interacts with another entity, like how a player damages a boss or vice versa.

Header.\
```
    {   
        "key":"punch",
        "name":"Basic Punch",
        "aliases":["punch","attack"],
        "description":"Apply fist to target's face.",
        "type":"cast",
```
Details:
* ` "aliases":["punch","attack"],` - What the skill can be referred to as when a player types in `cast <attack>`. All skill aliases need to be unique to avoid conflicts. If this field is missing, the alias used will be the key.
* `"type":"cast",` - The type of the skill.
    * `cast` - The action happens after the skill is done casting.
    * `channel` - The action happens repeatedly while the caster keeps channelling the skill, up to a duration.
    * If this line is missing, type is assumed to be `cast`.
    
Values by level.\
Skills can be levled up at trainer NPCs (TBD), therefore these block provide the stats for the different levels of the skill.
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
Details:
* `"cooldown":"30/GET_PROP(CASTER,ATT_SPEED)",` - How long it takes before the skill can be used again, in seconds.
    *If this line is missing or the value is lower than 1, the skill has 1 second of cooldown (to avoid the possibility someone spamming).
* `"formula":"HARM($T,$S,P,GET_PROP($S,B_DMG)+10+GET_PROP($S,STR)/2)",` - The script that details what the skill does.
    *In this example case, "Basic Punch" harms the caster's target for the caster's Base damage + 10 + half of the caster's strength. Damage type is Physical.
* "needed_level":1 - At what level the skill can be purchased from the trainer NPC.

Cast skill with longer duration.\
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
                "push_back":"true",
                "interrupt":"true",
                "formula":"HEAL($T,$S,10+GET_PROP($S,INT)*5)",
                "needed_level":5,
                "cost":{"key":"MP","value":"10"},
				"treat":"5"
            },
```
Details:
* ` "duration":"10"` - How long it takes to cast this skill, in seconds.
* ` "push_back":"true",` - If it's possible to be pushed-back while casting this skill
    * In this example case, if you are pushed back for 6 seconds while casting "Inspiration", your total cast time will be 16 seconds.
    * If this line is missing, default push-back is true.
    * For channelling type spells, push-back works in reverse, expending your duration. If channelling a spell that lasts 10 second, and and you are pushed-back 2 seconds, instead of 10 seconds, the spell will channel for 8 seconds.
    * A value of "false" means the cast/channel of this skill will never be pushed back.
* `"interrupt":"true",` - Whether the skill can be interrupted by interrupt-type attacks. 
    * If this line is missing, the skill can be interrupted.
    * Accepts formulas that return true/false like operators `> < or =`.
* `"treat":"5"` - How much threat this level of this skill generates before threat multiplier is taken into account.
* '"cost":{"key":"MP","value":"10"}, ' - How much of a resource casting this skill costs. If this field is missing, it is assume the skill costs 0 HP to cast.
 (HP is used because it is the only guaranteed resource of the engine)



Channelling skill example.\
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
Details:
* `"duration":"60",` - How long the skill is channelled for, in seconds.
* `"interval":"1+NONNEG(9-DEX/20)"` - The interval between two applications of the skill.
    * In this example case, "Consecutive Normal Punches" deals damage every 10 seconds if the caster's dexterity is 0. 1 second is shaved off with every 20 points of dexterity, until. at 180 DEX, the damage instance happens once every second for the duration.