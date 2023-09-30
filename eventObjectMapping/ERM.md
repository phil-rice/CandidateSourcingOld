# Event Object Mapping

If we want to persist and retrieve our objects we need to be able to split
them up and recombine them

For example in the demo project 'candidate sourcing' each 
section is its own entity in the event store

Do I really need this?

Actually no. It's seriously cool but lets put it off because it could
easily take two or three more days to get really going.

We can kick off with just a CAS store for all values in the event

We can get everything else working and then come back.

# Powerful stuff
The event can reference the ERM... And then everything is data driver. 
i.e. go to the Event store and you know how to perist (note that loading is 'process the events' this is just 'how to create the events')

We can change this mid flight and that's OK: these are just 'how do I save changes'. 

# Relationship updating

We need this though. We need to be able to specify the rules for relationships
so that when we persist objects we update the relationships

## Definitions

Fields (and fields in lists/dictionaries) are relationships
For example:
* the field 'who' in a list of sections in an application
* the job sha links to applications


## MVP implementation
Obviously we want to do a delta, but an MVP is 'delete everything and add it back'




