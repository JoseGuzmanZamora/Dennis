namespace Assets.Scripts
{
    public class StoryNode
    {
        public int Id { get; set; }
        public string ReferenceName { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public int FirstPathId { get; set; }
        public int SecondPathId { get; set; }
    }
}