using MediatR;

namespace Appts.Features.SharedKernel.Features.Identity;
public record GetUserByIdQuery(string UserId) : IRequest<GetUserByIdResponseModel>;