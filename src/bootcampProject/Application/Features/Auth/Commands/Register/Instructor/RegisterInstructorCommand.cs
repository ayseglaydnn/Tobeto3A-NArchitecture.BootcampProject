﻿using System.ComponentModel;
using Application.Common.Helpers;
using Application.Common.Services;
using Application.Features.Auth.Rules;
using Application.Features.Instructors.Constants;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using MimeKit;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Mailing;
using NArchitecture.Core.Security.Hashing;
using static Application.Features.Instructors.Constants.InstructorsOperationClaims;

namespace Application.Features.Auth.Commands.Register.Instructor;

public class RegisterInstructorCommand : IRequest<RegisteredInstructorResponse>, ISecuredRequest
{
    public InstructorRegisterDto InstructorForRegisterDto { get; set; }
    public string IpAddress { get; set; }

    public string[] Roles => [Admin, Write, InstructorsOperationClaims.Create];

    public RegisterInstructorCommand()
    {
        InstructorForRegisterDto = null;
        IpAddress = string.Empty;
    }

    public RegisterInstructorCommand(InstructorRegisterDto instructorForRegisterDto, string ipAddress)
    {
        InstructorForRegisterDto = instructorForRegisterDto;
        IpAddress = ipAddress;
    }

    public class RegisterInstructorCommandHandler : IRequestHandler<RegisterInstructorCommand, RegisteredInstructorResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IInstructorRepository _instructorRepository;
        private readonly IPasswordGenerateService _passwordGenerateService;
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IMailService _mailService;
        private readonly IEmailTemplateFillerService _emailTemplateFillerService;

        public RegisterInstructorCommandHandler(
            IUserRepository userRepository,
            IAuthService authService,
            AuthBusinessRules authBusinessRules,
            IInstructorRepository instructorRepository,
            IPasswordGenerateService passwordGenerateService,
            IUserOperationClaimRepository userOperationClaimRepository,
            IMailService mailService,
            IEmailTemplateFillerService emailTemplateFillerService
        )
        {
            _userRepository = userRepository;
            _authService = authService;
            _authBusinessRules = authBusinessRules;
            _instructorRepository = instructorRepository;
            _passwordGenerateService = passwordGenerateService;
            _userOperationClaimRepository = userOperationClaimRepository;
            _mailService = mailService;
            _emailTemplateFillerService = emailTemplateFillerService;
        }

        public async Task<RegisteredInstructorResponse> Handle(
            RegisterInstructorCommand request,
            CancellationToken cancellationToken
        )
        {
            await _authBusinessRules.UserEmailShouldBeNotExists(request.InstructorForRegisterDto.Email);

            var password = _passwordGenerateService.GetRandomPassword();

            HashingHelper.CreatePasswordHash(
                password,
                passwordHash: out byte[] passwordHash,
                passwordSalt: out byte[] passwordSalt
            );

            Domain.Entities.Instructor newUser =
                new()
                {
                    FirstName = request.InstructorForRegisterDto.FirstName,
                    LastName = request.InstructorForRegisterDto.LastName,
                    NationalIdentity = request.InstructorForRegisterDto.NationalIdentity,
                    DateOfBirth = request.InstructorForRegisterDto.DateOfBirth,
                    CompanyName = request.InstructorForRegisterDto.CompanyName,
                    UserName = request.InstructorForRegisterDto.Email,
                    Email = request.InstructorForRegisterDto.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                };

            Domain.Entities.Instructor createdUser = await _instructorRepository.AddAsync(newUser);

            // User operation claims

            List<UserOperationClaim> userOperationClaims = new List<UserOperationClaim>
            {
                new UserOperationClaim { UserId = createdUser.Id, OperationClaimId = 36 },
                new UserOperationClaim { UserId = createdUser.Id, OperationClaimId = 66 }
            };

            await _userOperationClaimRepository.AddRangeAsync(userOperationClaims);

            // Mail

            var fullName = $"{createdUser.FirstName} {createdUser.LastName}";
            var email = createdUser.Email;

            var toEmailList = new List<MailboxAddress> { new(fullName, email) };

            var htmlContent = _emailTemplateFillerService.PopulateInstructorApplicationApproveEmail(
                createdUser.FirstName,
                createdUser.LastName,
                email,
                password
            );

            _mailService.SendMail(
                new Mail
                {
                    Subject = "New Instructor Account Information - Teach It Free",
                    TextBody = "",
                    HtmlBody = htmlContent,
                    ToList = toEmailList,
                }
            );

            RegisteredInstructorResponse registeredInstructorResponse =
                new() { Email = request.InstructorForRegisterDto.Email, Password = password };

            return registeredInstructorResponse;
        }
    }
}
