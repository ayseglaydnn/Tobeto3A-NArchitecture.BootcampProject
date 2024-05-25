using Application.Features.Contacts.Constants;
using Application.Features.Contacts.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using static Application.Features.Contacts.Constants.ContactsOperationClaims;

namespace Application.Features.Contacts.Commands.Delete;

public class DeleteContactCommand
    : IRequest<DeletedContactResponse>,
        ISecuredRequest,
        ICacheRemoverRequest,
        ILoggableRequest,
        ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, ContactsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetContacts"];

    public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, DeletedContactResponse>
    {
        private readonly IMapper _mapper;
        private readonly IContactRepository _contactRepository;
        private readonly ContactBusinessRules _contactBusinessRules;

        public DeleteContactCommandHandler(
            IMapper mapper,
            IContactRepository contactRepository,
            ContactBusinessRules contactBusinessRules
        )
        {
            _mapper = mapper;
            _contactRepository = contactRepository;
            _contactBusinessRules = contactBusinessRules;
        }

        public async Task<DeletedContactResponse> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            Contact? contact = await _contactRepository.GetAsync(
                predicate: c => c.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _contactBusinessRules.ContactShouldExistWhenSelected(contact);

            await _contactRepository.DeleteAsync(contact!);

            DeletedContactResponse response = _mapper.Map<DeletedContactResponse>(contact);
            return response;
        }
    }
}
