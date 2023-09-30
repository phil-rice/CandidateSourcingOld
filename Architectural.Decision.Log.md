

# Why Content Addressable Storage

This gives an enormous amount of protection from hacking. 
* No one can edit the data without changing the key
* Thus you can't just 'change a value'
* 

# Why event stores
* This is a natural match.
	* We need a strong audit
	* We need to be able to show 'what we knew at a particular date'
* It's less work than audit history
	* And 'we know' that the history is correct

# Why APIs for events, cas and relationships
Because of constraints in the assignment. There is no need
at all for these expensive and timeconsuming (to make and run) APIs. 
All of this can be done 'on the edge' for enormously less resources
and no barrier to scaling. 

# Why SQL Server
I would prefer to use a file system for events and CAS as it is quicker
just as robust, enormously cheaper and so on. I would also rather use
a graph database for relationships. However it is a constraint
on the project that we use SQL server

# Why contract testing
This is a microservice based architecture. Every such architecture such use
contract testing. I think without exception

# Why XUnit for testing
Because its the 'current defacto'

# Why JSON.NET
Because we have the need for polymorphic JSON in the events. An event
could be one of many classes and the standard JSON library can't do this natively
The JSON produced is not great, but it's not human facing so OK

# Why sections and job rather than hardcoding
Because we might want to interview many different types of people:
* HR
* DBA
* Backend
* Frontend
* SAP
* L1 support...

The questions will be different and the process may be different.

In addition by doing it this way we have less gui code to produce.

The gui is entirely data driven: each of the gui screens for the candidate,
hr manager, manager etc are 

# Why single method interfaces for the repository
Because of SOLID design principles.
* A piece of software should do one and only one thing
* We should not inject an interface that does more than the recipient wants to do with it

# Why are we not addressing the N+1 problem

Example of problem: Given an entity and relation, find all the json at the end.
At the moment we go to relationships and then for each found entity make another call

Answer:
* N is small, this is an MVP and there is no need to fix this for the MVP
* Note the use of IRelationshipToJson is designed to mask this and allow a single place to fix it later

# Why is Job directly creating things in the event store instead of commands
This is a MVP. Adding in a command layer is another layer of complexity

While it dramatically improves testing and the ability to reason it will take time

I can refactor it later

