using FluentValidation;

namespace Application.Features.Bootcamps.Commands.Update;

public class UpdateBootcampCommandValidator : AbstractValidator<UpdateBootcampCommand>
{
    public UpdateBootcampCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.InstructorId).NotEmpty();
        RuleFor(c => c.StartDate).NotEmpty();
        RuleFor(c => c.EndDate).NotEmpty();
        RuleFor(c => c.BootcampStateId).NotEmpty();
    }
}
