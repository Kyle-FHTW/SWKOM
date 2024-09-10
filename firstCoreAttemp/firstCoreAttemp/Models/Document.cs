namespace firstCoreAttemp.Models
{
    
    

    public class Document
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Metadata { get; set; }
        public string? Description { get; set; }
        public Tag[] TagsAssigned { get; set; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            // Handle TagsAssigned array to print the tags
            string tags = TagsAssigned != null && TagsAssigned.Length > 0
                ? string.Join(", ", TagsAssigned.Select(tag => tag.ToString()))
                : "None";

            // Return formatted string representation of the object
            return $"Id: {Id}, Title: {Title ?? "N/A"}, Metadata: {Metadata ?? "N/A"}, " +
                   $"Description: {Description ?? "N/A"}, TagsAssigned: [{tags}]";
        }

        public enum Tag
        {
            tag1,
            tag2,
            tag3
        }


    }
}
