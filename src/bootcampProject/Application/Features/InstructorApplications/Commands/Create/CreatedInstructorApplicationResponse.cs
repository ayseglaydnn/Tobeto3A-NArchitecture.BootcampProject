using NArchitecture.Core.Application.Responses;

namespace Application.Features.InstructorApplications.Commands.Create;

public class CreatedInstructorApplicationResponse : IResponse
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? NationalIdentity { get; set; }
    public string? CompanyName { get; set; }
    public string? AdditionalInformation { get; set; }
    public bool? IsApproved { get; set; }
}
