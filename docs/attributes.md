# Attributes


Attributes are numbers that influence other numbers in the game. \
You can add anything you want as an attribute, just be careful with the `key` property, as it is what will be used to identify your attribute in other formulas.
```
	{
    "key": "STR",
    "name": "Strength",
    "description": "Boosts physical abilities."
    },
    ...
```
Details:
* `"key": "STR",` - an unique string that will be used to look up this attribute in scripts
* `"name": "Strength",` 
	* __[String]__ A human readable name. 
	* __[Missing/Default]__ The key will be used as the attribute's name.
* `"description": "Boosts physical abilities.",`
	* __[String]__  A human readable description. 
	* __[Missing/Default]__ No description will be given for this attribute.

Minimum example:
```
	{
   		"key":"STR"
    }
    
```


__[Note]__ The notes for "name" and "description" will apply to everything from this point onwards.

