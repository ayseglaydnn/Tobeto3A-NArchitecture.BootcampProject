using NArchitecture.Core.Application.Responses;

namespace Application.Features.Bootcamps.Commands.Update;

public class UpdatedBootcampResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid InstructorId { get; set; }
    public Boolean BootcampState { get; set; }
    public string BootcampImage { get; set; }
}
