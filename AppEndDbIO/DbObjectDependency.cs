namespace AppEndDbIO
{
    public class DbObjectDependency
    {
        public string ReferencingSchema { get; set; }
        public string ReferencingObject { get; set; }
        public string ReferencingObjectType { get; set; }
        public string ReferencedSchema { get; set; }
        public string ReferencedObject { get; set; }
        public string ReferencedObjectType { get; set; }

        public DbObjectDependency(string referencingSchema, string referencingObject, string referencingObjectType,
                                 string referencedSchema, string referencedObject, string referencedObjectType)
        {
            ReferencingSchema = referencingSchema;
            ReferencingObject = referencingObject;
            ReferencingObjectType = referencingObjectType;
            ReferencedSchema = referencedSchema;
            ReferencedObject = referencedObject;
            ReferencedObjectType = referencedObjectType;
        }
    }
}


