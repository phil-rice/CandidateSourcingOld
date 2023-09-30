using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xingyi.relationships
{
    public static class RelationshipFixture
    {
        public static Entity e1 = new Entity("sourcea", "nsa", "name1");
        public static Entity e2 = new Entity("sourceb", "nsb", "name2");
        public static Entity e3 = new Entity("sourcec", "nsc", "name3");
        public static Entity e4 = new Entity("sourceb", "nsb", "name4");
        public static Entity e5 = new Entity("sourcec", "nsc", "name5");
        public static Relation r1 = new Relation("rns1", "rname1");
        public static Relation r2 = new Relation("rns2", "rname2");

        public static Relationship rel112 = Relationship.from(e1, r1, e2);
        public static Relationship rel123 = Relationship.from(e1, r2, e3);
        public static Relationship rel114 = Relationship.from(e1, r1, e4); //this can be used to 'update' rel112 - same store/namespace/rel
        public static Relationship rel125 = Relationship.from(e1, r2, e5); // this can be used to 'update'rel124 


    }
}
