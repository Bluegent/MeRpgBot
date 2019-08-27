# Base values
Each row contains the key that the engine will use to identify this specific value in formulas. The names can contain anything except special characters reserved for formulas (see [glosarry.md](glosarry.md)).
The values themselves are defined by the class of the entity or the boss type.
```
    {
        "key":"BASE_DMG" ,
        "name":"Base Damage", 
        "description":"Base amount of damage"
    },
    ...
```

* `"key": "BASE_DMG",`
	* __[String]__ A unique string that will be used to look up this attribute in scripts
* `"name": "Base Damage",` 
	* __[String]__ A human readable name. 
	* __[Missing/Default]__ The key will be used as the attribute's name.
* `"description": "Base amount of damage",`
	* __[String]__  A human readable description. 
	* __[Missing/Default]__ No description will be given for this attribute.

Minimum example:
```
    {
        "key":"BASE_DMG"
    }
```

__[Note]__ The notes for "name" and "description" will apply to everything from this point onwards.