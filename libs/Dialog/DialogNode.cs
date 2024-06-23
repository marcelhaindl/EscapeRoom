namespace libs
{
    public class DialogNode
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int[] Choices { get; set; } // Array of next node Ids
    }
}
