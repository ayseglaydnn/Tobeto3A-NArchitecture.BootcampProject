﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.InstructorApplications.Commands.Approve;
using FluentValidation;

namespace Application.Features.InstructorApplications.Commands.Decline;

public class DeclinedInstructorApplicationCommandValidator : AbstractValidator<DeclineInstructorApplicationCommand>
{
    public DeclinedInstructorApplicationCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Comment).MaximumLength(100);
    }
}
