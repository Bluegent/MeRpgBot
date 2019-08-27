# Stats

Numeric values based on other values. \
Check [Attributes](attributes.md) for additional info.

```
    {
        "key": "DEF",
        "formula": "BASE_DEF+STR/4",
        "name": "Physical Defense",
        "description": "Helps you avoid physical damage."
    },
...
```  
Details:      
* `"formula": "BASE_DEF+STR/4",`
	* __[Formula]__ How the stat is calculated.
    * __[Summary]__ In our current case, the DEF stat is calculated as base defence + a quarter of the entity's strength attribute.

Minimum example:

```
    {
        "key": "DEF",
        "formula": "BASE_DEF+STR/4",
    }
```  
   

