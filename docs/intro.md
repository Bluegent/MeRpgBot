# Intro
Some general notes for configuring RPGBot.

Values marked with "key" have to be unique among themselves. That is to say, you can't have two attributes, skills, classes etc with the same key as these are used to search the various values stored in the game.
With a few exceptions marked as engine-required, everything can be customized. The idea here was to make a flexible engine that would allow people to make server-themed RPGs without having to write code, so I created a small scripting language called __MicroExpression__ for this purpose.
These files aim to explain how to configure RPGBot to your liking.
The .json files come in [JSON format](https://en.wikipedia.org/wiki/JSON) . It's generally a file format that holds key-value data for easier parsing.
The .me files contain MicroExpression script and are generally used to establish a boss' behaviour pattern.
Certain lines in the configuration can be missing and will have default values attributed to them according to the explanation in their respective .md file.

It might be hard to grasp things at first, especially for a non-programmer, but this guide has line by line instructions on what all the configuration elements are and the .json/.me files provided are a skeleton configuration to illustrate mechanics.  


#### Contents:
* [Glossary](glossary.md)
* [MicroExpression notes](micro_expression.md)
* [Explanation of tags](tags.md)
* [Core](core.md)
* [Attributes](attributes.md)
* [Stats](stats.md)
* [Resources](resources.md)
* [Damage Types](damage_types.md)
* [Statuses](statuses.md)
* [Skills](skills.md)
* [Classes](classes.md)
* [Bosses]( TBD )





#### Gameplay loop
1. Create a character, picking a name and class for them.
2. Configure RPGBot's interaction(if it's going to tag you in its messages, if you want to be dm-ed the output of help commands, etc.) with you. (TBD)
3. Wait until a boss spawns.
4. Fight the boss.
5. If you die, wait for the revive timer or another player to revive you.
6. If the boss is defeated, earn loot, experience and gold.
7. Upgrade your attributes, skills and items, purchase consumables from the various NPCs. (Items are TBD)
8. Repeat from 3.

