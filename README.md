# Links
* [Trello](https://trello.com/b/ZUVU6zzr/dotnet)
* [Github](https://github.com/phil-rice/CandidateSourcing)
* [Build pipeline](https://github.com/phil-rice/CandidateSourcing/actions)
* [Stackoverflow question](https://stackoverflow.com/questions/77196218/i-have-a-interop-exception-with-a-consumer-contract-test-under-dotnet)
* [Issue](https://github.com/pact-foundation/pact-net/issues/471)

# Candidate Sourcing 

## Given requirements

For Large Organisation, it is not practical to maintain the records of the interviewing process. Candidate Sourcing portal enables maintenance of the interview records and score the candidates based on the discussion. Managers will login to the system and will be able to review the candidature  
Each interview would have rounds panellist 1, panellist 2 ,HR, Manager, Recruiter, Candidate  

## Scenarios  

* Candidate Login : Candidate updates his details, Passport Number, Name, address, DOB. He should not be able to view panellist forms  
 * First Round : Panellist 1 would login to the portal and provide feedback in various sections on a score with scale of 1 to 10 (10 being highest). Detail feedback section should allow a maximum of 1000 characters.  
The weightage for the score is 40%  
* Second Round : Panellist 2 would login to the portal and provide feedback in various sections on a score with scale of 1 to 10 (10 being highest). Detail feedback section should allow a maximum of 1000 characters. The weightage for the score is 50%  
* HR Round : Login to the portal and provide feedback and give a score on a scale of 1 to 10(10 being highest). Detail feedback section should allow a maximum of 1000 characters. The weightage for this score is 10%  
* Manager : Manager logs in and checks the candidate rating which is weighted average of the interviews’ score from the three rounds of the candidate   
  
Scoring
* Weighted average is > 8 then rating is Expert  
* Weighted average is > 6 and <=7.99 then rating is Proficient  
* Weighted average is > 4 and <=5.99 then rating is Proficient  
* Weighted average is <4 then rating is Reject  
  
# Search and view reports of candidate feedback  
 * The candidate details are maintained in the organisation for 1 year and candidates rejected cant appear the interview within 6 months of their attempt. Recruiter can login and check if the candidate has appeared for interview in the last 6 months  

## Observations on requirements

### Legal

At least two pieces of legislation hit us very quickly
* Recruitment (different by country)
* GDPR

The recruitment legislation is quite onerous. It leads me to conclude that we need auditable records so that we can recreate 
the view of the data at any moment in time. This is awkward when people are allowed to edit data: for example to refine the scores
that they give. And the usability of the application becomes terrible if we don't allow that.

GDPR requires us to encrypt the data and have a consent form. And record that the person has seen and agreed to the consent form 
(we pretend that people actually read these things)

### Jobs are different
We want the same code to work for multiple types of jobs. For example
* HR manager
* DBA
* Backend developer
* SAP architect

This leads to the observation that the questions asked for each job will be different. And that probably slightly different 
processes will be used. (For example we might a technical test and a interview). This is easily handled if we design it in 
at the beginning.

### Authentication is difficult
To create a login system that is compliant with the OWASP authentication guidelines https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html is
not possible in the time scales we have. 

Thus we will use OAUTH2 for login and allow people to login with google. I would do it with HCL LDAP but that is impractical in the timescales 
we have.

### Name matching is difficult
How do we know if this candidate is the same as someone who has done this before. That problem is sufficiently complex that we won't
try and solve it. We will simplify this to 'we use email as the primary key of a person'. While that is spoofable, so is every other
primary key we could think of (passport, name, name+address, name + postcode...etc) and has the advantage that it is simple to understand

# Architecture

## Event sourcing
We will use event sourcing to store all our database entities

As we need strong audit, I am going to use an executable audit log. Also known as event sourcing
* https://developer.ibm.com/articles/event-sourcing-introduction/
* https://dzone.com/articles/introduction-to-event-sourcing

This is represented in the code by the three projects eventClient, eventApi and eventCommon

Each event takes a block of json and modifies it by the event. This is abstract, so lets use an example while explaining it. We have 
a piece of information that represents an candidate application

We have three main events:
* SetToCas(SetToCasEvent e, T t). -- In our example we would use this to initialise the candidate application to our template
* SetFieldToValue                 -- In our example we use this to set a single string field in the application. Such as the email address of an interviewer
* SetFieldToCas                   -- In our example we use this to set a field to a value stored in the Cas. Such as linking to the Candidate details, or to the results of an interview
* setEventTo

Although there are much better technologies we will use SqlServer as that is a constraint on the project.
* We will use the document database model approach
* Data will be a 'set of bytes'. 
* Our id is namespace + name (and the name is the sha of the data)

## Content Addressable Storage

We have a number of important pieces of information that we want to know have not been edited (because of the legal reasons), so I will
use content addressable storage to store these. These include 'candidate details', 'a reviewers summary of the interview'. 
* https://en.wikipedia.org/wiki/Content-addressable_storage
* https://www.techtarget.com/searchstorage/definition/content-addressed-storage

This is represented in the code by three projects casClient, casApi and casCommon

Important ideas:
* NameSpace -- I added this so we can find all the things that are 'candidate info', 'applications', 'interview results'
* Name -- this is the SHA of the value stored

The namespace isn't critical yet. But if I expand the project it will store things like json schema and perhaps other validation criteria.

Although there are much better technologies we will use SqlServer as that is a constraint on the project
* We will use the document database model approach
* We will have a list of events stored as a json string.
* Our id is a namespace + name (and the name can be anything)


## Relationships

Obviously we need to be able to answer questions like the following

* Show all the applications for this candidate
* Show all the applications that I am an interviewer/hr manager/manager for
* show all the applications for this job
 

These are relationships between 'entities' (i.e. events in an event store and 'something'
* The something at the moment is an email address.
* In the future it might be a 'cas' item or an 'entity' in the eventstore.

It is possible (but bad) to store these relationships in the data. It is far better to store them in a separate table. 
* Watch this for why https://www.youtube.com/watch?v=aaAtKNrcIS0

This is represented in the code by the three projects eventClient, eventApi and eventCommon

A relationship
* Has a source (store/namespace/name), relationship(namespace/name), and target(store/namespace/name)
* store can be 'cas', 'events', 'email', 
* for things like email we will set the namespace to email. For cas/events the namespace is whatever it is
* This is enough to represent RDF or any other reasonable triple/graph store. 

Probably we can do better, but this is just a demo project and I don't want to spend too long in design time

Although there are much better technologies (any graph database) we will use SqlServer as that is a constraint on the project


# Domain

## Application

This is
* A candidate is applying for a job
* And there are interviewers
* And a HR manager
* And a manager
* The candidate needs to fill in their details
* The interviewers, HR managers need to fill in their details (typically interviews)

Notes:
* An Application is an entity. So it will be stored in the event store
* It will start off with a template from a job. We take a copy of the current value of the job at the moment the application is created.
    * Within this application it will not be possible to update the template once created
* People will be assigned to fill in sections.
    * The candidate is special and cannot be changed. 
* As people fill in sections, those sections will typically be entities themselves (candidate details/interviews)
* We will create relationships between the people filling in the sections.
    * When the person filling in the section changes we need to update relationships

## Application Section

This is a piece of data that has to be filled in by someone. Perhaps the candidate, perhaps an interviewer

* A section has a name/title/description
* An email for the person who will fill it in
* A flag on whether the person filling it can change (can't for candidate for example)
* And has Questions.
    * Question type includes 'string', 'scored'
    * A question has title/description/max length/whether mandatory
    * a scored question has the same but also will be displayed with a score as well as the text
* Layout: We can consider adding layout details to this. But this is for later 

Note now that the same gui can be used to display the candidate details, 
the interviews and so on. There is on longer any difference between any of these



## Job

A job is a template for an interview process. Note that we want different interview questions and processes for different jobs. For 
example we might be recruiting
* An HR Manager
* A DBA
* A backend developer
* An architect
* A call center L1 support person

The job includes a template. This template is copied (and partially filled in) when an application is created


# Ideas we don't have or need

## Roles
In the spec we have manager, hr manager, candidate, interview

Note that in different applications a person could be in different roles.
* I might be the HR maanger
* And I could be applying for a job as a higher grade HR manager - the candidate
* Recruiting someone to work for me- manager
* And I might be an assigned interviewer...

Thus we say 'when looking at an application it's about the section you are filling in'. Your 'role' in that application is based on 
whether you can fill in a section





 











# Project structure

## 





# Setting up database

Remember to run migrations
```bash
dotnet ef database update
```