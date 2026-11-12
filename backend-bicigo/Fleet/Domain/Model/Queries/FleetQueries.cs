namespace backend_bicigo.Fleet.Domain.Model.Queries;

/// <summary>Query to fetch a bicycle by id (GET /bicycles/:id).</summary>
/// <param name="Id">Bicycle id.</param>
public record GetBicycleByIdQuery(int Id);

/// <summary>Query to fetch a bike point by id (GET /bike-points/:id).</summary>
/// <param name="Id">Bike-point id.</param>
public record GetBikePointByIdQuery(int Id);
