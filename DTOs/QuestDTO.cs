using System.ComponentModel.DataAnnotations;

public class QuestDTO
{
     public required string Title {get; set;}
     public required  string Description {get; set;}
    public DateTime Deadline { get; set; }

}