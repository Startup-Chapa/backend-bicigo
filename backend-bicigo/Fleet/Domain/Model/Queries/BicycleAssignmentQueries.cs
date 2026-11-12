namespace backend_bicigo.Fleet.Domain.Model.Queries;

/// <summary>Query to fetch a bicycle assignment by id.</summary>
/// <param name="Id">Assignment id.</param>
public record GetBicycleAssignmentByIdQuery(int Id);
