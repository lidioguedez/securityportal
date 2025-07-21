using SecurityPortal.Domain.Common;

namespace SecurityPortal.Domain.Entities;

public class IncidentAttachment : BaseEntity
{
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string UploadedBy { get; private set; } = string.Empty;
    public Guid IncidentId { get; private set; }

    private IncidentAttachment() { }

    public static IncidentAttachment Create(string fileName, string filePath, string uploadedBy, Guid incidentId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));
        if (string.IsNullOrWhiteSpace(uploadedBy))
            throw new ArgumentException("UploadedBy cannot be empty", nameof(uploadedBy));

        return new IncidentAttachment
        {
            FileName = fileName,
            FilePath = filePath,
            ContentType = GetContentType(fileName),
            UploadedBy = uploadedBy,
            IncidentId = incidentId,
            CreatedBy = uploadedBy
        };
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

    public void UpdateFileSize(long size)
    {
        FileSize = size;
    }
}