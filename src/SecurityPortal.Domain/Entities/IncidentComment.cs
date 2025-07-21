using SecurityPortal.Domain.Common;

namespace SecurityPortal.Domain.Entities;

public class IncidentComment : BaseEntity
{
    public string Content { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public Guid IncidentId { get; private set; }
    public bool IsInternal { get; private set; }

    private IncidentComment() { }

    public static IncidentComment Create(string content, string author, Guid incidentId, bool isInternal = false)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty", nameof(content));
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Comment author cannot be empty", nameof(author));

        return new IncidentComment
        {
            Content = content,
            Author = author,
            IncidentId = incidentId,
            IsInternal = isInternal,
            CreatedBy = author
        };
    }

    public void UpdateContent(string newContent, string updatedBy)
    {
        Content = newContent;
        UpdateTimestamp(updatedBy);
    }
}