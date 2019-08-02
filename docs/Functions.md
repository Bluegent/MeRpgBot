Chapters:
* Operators
* Functions
* Keywords

Operators: 
* "+ - / *" - self explanatory
* "> < =" - comparison
* ^ - power operator
    
Functions:
* MAX(value1,value2,...)  - returns the maximum value between the supplied ones, only takes numeric values
    * MAX(10,11,12) = 12
* MIN(value1,value2,...)  - returns the minimum value between the supplied ones, only takes numeric values
    * MIN(10,11,12) = 10
* ABS(value) - returns the absolute of value
    * ABS(10) = ABS(-10) = 10
* NON_NEG(value) - returns value if it's non-negative or 0 otherwise 
    * NON_NEG(10) = 10, NON_NEG(-100) = 0, NON_NEG(-10) = 0
* RANDOM(min, max) - returns a random integer value between min and max
    * RANDOM(3,10) = 5
    * Note: the value returned will never be equal to max, but can be equal to min
* HARM(entity,damage_type,value) - damages entity for value of damage_type type
    * HARM(BOSS,physical,30) - Entity BOSS would be damaged for 30 points, but the actual damage will be calculated based on physical's mitigation formula 
* HEAL(entity,value) - heals entity for value
    * HEAL(BOSS,30) - Entity BOSS would be healed for 30 points     
* ARRAY(value1, value2, ...) - produces an array for functions that take one as an argument
    * ARRAY(1,2,3,4) = [1,2,3,4]
* GET_PLAYERS() - returns an array with all the existing players
    * GET_PLAYERS() = [Jimmy, Billy, Johnny, Frank]
* GET_ACTIVE_PLAYERS() - returns an array with all the online players
    * GET_ACTIVE_PLAYERS() = [Jimmy, Johnny]
* ARR_RANDOM(array) - returns a random value from the array
    * ARR_RANDOM(ARRAY(1,2,3)) = 2
    * ARR_RANDOM(GET_PLAYERS()) = Billy (assuming Billy is one of the players)
* GET_PROP(entity,key) - retrieves the property of the entity that matches the given key
    * GET_PROP(BOSS,DEF) = boss' defence
* IF(condition, then, else) - evaluates condition then executes either then or else
    * IF(GET_PROP($CASTER,STR) > GET_PROP($CASTER,DEX) , STR, DEX) - returns the higher value between the caster's STR and DEX attributes
    * IF(GET_PROP($CASTER,INT) > 10, HEAL($CASTER,100), HARM($CASTER,M,10)) - if the caster has more than 10 INT points, they get healed for 100, else they get harmed for 10 magical damage
* CAST(caster,target,skill_key) - casts a skill, if it's found in the skill dictionary
    *CAST(Billy,Jimmy,heal) - Billy will cast heal on Jimmy
* CHANCE(percentage) - rolls for an outcome with a percentage chance
    * CHANCE(25) - will return true 25% of the time
* APPLY_STATUS(entity,status,duration,value(s)) - apply a status effec to given entity that lasts a certain duration
    * APPLY_STATUS(Billy,poison,60,ARRAY(10,10)) - will poison Billy for 60 seconds, where Billy takes 10 damage every 10 seconds.
    * Note: The last argument can be a single value or an array and is interpreted by the status itself. In this case we assume the values that poison takes are damage and interval.
          
Keywords:
* skill:
    * $TARGET - represents the target entity
        * HEAL($TARGET,10) - resolves to healing the target of the skill
    * $CASTER - represents the caster entity
        * HEAL($TARGET,10) - resolves to healing whoever casts the skill
* damage types:
    * $TARGET - the entity being damaged
    * $VALUE - the value of the damage before it is mitigated
        * $VALUE-GET_PROP($TARGET,DEF) - reduces the value of the damage by the DEF stat of the target
* boss scripts:
    * $ME - boss entity
        * GET_PROP($ME,CHP) - returns bosses's current HP
* statuses:
    * $0, $1, $2... - 
* level values:
    * $PREV - how many experience points were needed to level up for the previous level
    * LEVEL - your current level
    
