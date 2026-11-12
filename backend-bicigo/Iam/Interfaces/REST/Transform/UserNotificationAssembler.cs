using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Interfaces.REST.Resources;

namespace backend_bicigo.Iam.Interfaces.REST.Transform;

/// <summary>Assembles UserNotification resources.</summary>
public static class UserNotificationAssembler
{
    /// <summary>Assembles a resource from an entity.</summary>
    /// <param name="entity">The notification.</param>
    /// <returns>The resource.</returns>
    public static UserNotificationResource ToResourceFromEntity(UserNotification entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Title = entity.Title,
        Message = entity.Message,
        Type = entity.Type,
        IsRead = entity.IsRead
    };
}