using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xingyi.relationships
{
    static class RelationshipFixture
    {
        public static Entity e1 = new Entity("source1", "ns1", "name1");
        public static Entity e2 = new Entity("source2", "ns2", "name2");
        public static Entity e3 = new Entity("source3", "ns3", "name3");
        public static Entity e4 = new Entity("source4", "ns4", "name4");
        public static Entity e5 = new Entity("source5", "ns5", "name5");
        public static Relation r1 = new Relation("rns1", "rname1");
        public static Relation r2 = new Relation("rns2", "rname2");

    }
}
