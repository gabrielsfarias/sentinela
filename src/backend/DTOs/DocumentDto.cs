namespace Backend.DTOs;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string? OriginalFileName { get; set; }
    public long? OriginalFileSize { get; set; }
    public DateTime? OriginalFileLastModified { get; set; }
    public string? DisplayName { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
