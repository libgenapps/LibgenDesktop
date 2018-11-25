namespace LibgenDesktop.Models.Entities
{
    internal class LibraryFile
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string ArchiveEntry { get; set; }
        public LibgenObjectType ObjectType { get; set; }
        public int ObjectId { get; set; }
    }
}
