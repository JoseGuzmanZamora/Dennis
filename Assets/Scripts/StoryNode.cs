using System.Collections.Generic;

namespace Assets.Scripts
{
    public class StoryNode
    {
        public int Id { get; set; }
        public string ReferenceName { get; set; }
        public string ImageUrl { get; set; }
        public string Prompt { get; set; }
        public string Content { get; set; }
        public int? FirstPathId { get; set; }
        public int? SecondPathId { get; set; }
        public string FirstButtonText { get; set; }
        public string SecondButtonText { get; set; }
    }

    public class StoryNodes
    {
        public List<StoryNode> Nodes { get; set; }
    }
}