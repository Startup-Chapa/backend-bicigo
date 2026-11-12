namespace backend_bicigo.Trip.Domain.Model.Queries;

/// <summary>Query to fetch a trip user by id.</summary>
/// <param name="Id">Trip user id.</param>
public record GetTripUserByIdQuery(int Id);

/// <summary>Query to fetch a route point by id.</summary>
/// <param name="Id">Route point id.</param>
public record GetTripRoutePointByIdQuery(int Id);

/// <summary>Query to fetch a photo evidence by id.</summary>
/// <param name="Id">Photo evidence id.</param>
public record GetTripPhotoEvidenceByIdQuery(int Id);
