# Bounded Context: Trip

This folder contains the Trip bounded context implementation. It uses the existing `AppDbContext`, the existing Fleet `Bicycle` aggregate and the project's current REST conventions. No files outside `Trip/` are changed by this delivery.

## Main endpoints

Base URL: `/api/v1`

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/trips?userId=USER-001` | List trips; optional `userId`, `bicycleId` and `status` filters |
| POST | `/trips` | Create a pending trip; rejects a duplicate trip ID, another pending/in-progress trip for the same user, a missing bicycle or a non-available bicycle |
| GET | `/trips/{id}` | Get trip details by database ID |
| GET | `/trips/active?userId=USER-001` | Get the user's in-progress trip; returns `204` when there is none |
| GET | `/trips/history?userId=USER-001` | Get the user's finished trips |
| POST | `/trips/{id}/unlock` | Validate the scanned QR against the QR stored for the bicycle, set the bicycle to `IN_USE` and start the trip |
| POST | `/trips/{id}/finish` | Finish the trip, calculate the cost and release the bicycle to `AVAILABLE` |
| PUT | `/trips/{id}` | Compatibility endpoint for starting a trip; use `/unlock` and `/finish` for lifecycle transitions |
| DELETE | `/trips/{id}` | Delete a trip only while it is `PENDING` |
| GET/POST | `/qr-validations` | List/create QR validation records; validation uses the QR stored in Fleet, not the client-provided expected value |
| GET/POST | `/trip-route-points` | List/add GPS points for an in-progress trip |
| GET/POST | `/trip-photo-evidences` | List/add photo evidence associated with an existing trip |
| GET/POST | `/trip-users` | List/create Trip user reference records |

## Request examples

Create a pending trip:

```json
{
  "tripId": "TRIP-2026-001",
  "userId": "USER-001",
  "bicycleId": "BGO-1042"
}
```

Unlock the bicycle (the QR value must match the bicycle's persisted `QrCode` in Fleet):

```json
{
  "userId": "USER-001",
  "qrCodeSnapshot": "BICIGO-BGO-1042"
}
```

Finish a trip:

```json
{
  "userId": "USER-001",
  "distanceKm": 4.5,
  "pricePerKm": 0.29,
  "isPremium": false
}
```

Add a route point:

```json
{
  "tripId": "TRIP-2026-001",
  "latitude": -12.12,
  "longitude": -77.03,
  "sequence": 1
}
```

## Important integration notes

- `bicycleId` must identify a Fleet bicycle by its `BikeCode`, `SerialNumber` or numeric database ID. The sample seed trip currently uses `BIKE-001`, which does not match the seeded Fleet bicycle codes (`BGO-1042` and `BGO-1043`). The seed data should be aligned separately before using that sample trip for a full unlock/finish flow; the seeder was intentionally not changed here.
- The current backend does not configure authentication/authorization middleware. The `userId` supplied to these endpoints is therefore an ownership consistency check, **not proof of the caller's identity**. To enforce the authenticated-user acceptance criteria, the application must later connect these endpoints to the IAM authentication mechanism and derive the user ID from the authenticated principal. This delivery intentionally does not modify IAM or global middleware.
- The API accepts the rate per kilometre and premium flag in the finish request because the current Trip context has no tariff/subscription query contract. When Billing exposes the applicable tariff and subscription status, those values should be obtained server-side instead of trusting the request body.
- `dotnet build` and runtime tests could not be executed in the provided environment because the .NET SDK is not installed. Run `dotnet build` in the team's development environment before merging.
