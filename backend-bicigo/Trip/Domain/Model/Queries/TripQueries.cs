namespace backend_bicigo.Trip.Domain.Model.Queries;

/// <summary>Query to fetch a trip by id (GET /trips/:id).</summary>
/// <param name="Id">Trip id.</param>
public record GetTripByIdQuery(int Id);

/// <summary>Query to fetch a QR validation by id (GET /qr-validations/:id).</summary>
/// <param name="Id">Validation id.</param>
public record GetQrValidationByIdQuery(int Id);
