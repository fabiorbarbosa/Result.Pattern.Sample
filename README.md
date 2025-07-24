# Result.Pattern.Sample

This project is an example implementation of a result pattern in ASP.NET Core APIs. It demonstrates how to standardize HTTP responses and organize code for better clarity and reusability.

## Features

- **Response Patterns**:
  - Standardized responses for success, error, and other HTTP statuses (`200 OK`, `404 Not Found`, `201 Created`, etc.).
  - Generic models to encapsulate data and messages.

- **Example Endpoints**:
  - `GET /weatherforecast`: Returns a list of weather forecasts.
  - `GET /weatherforecast/{id}`: Returns a specific forecast or `NotFound` if the ID does not exist.

## Project Structure

- **Extensions**: Contains extensions to simplify the creation of standardized HTTP responses.
- **Models**: Defines data models to standardize responses.
- **Program.cs**: Main application configuration and endpoint definitions.
- **appsettings.json**: Application settings.

## Technologies Used

- **.NET 9.0**
- **ASP.NET Core**
- **Swagger/OpenAPI** for endpoint documentation.

## How to Run

1. Make sure you have the .NET 9.0 SDK installed.
2. Clone this repository:
   ```bash
   git clone https://github.com/your-username/Result.Pattern.Sample.git
   ```
3. Navigate to the project directory:
   ```bash
   cd Result.Pattern.Sample
   ```
4. Run the project:
   ```bash
   dotnet run
   ```
5. Access the endpoints in your browser or an HTTP client:
   - `http://localhost:5100/weatherforecast`
   - `http://localhost:5100/weatherforecast/{id}`

## Testing

You can test the endpoints using the `.http` file included in the project, with the "REST Client" extension in Visual Studio Code.

## Contribution

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

See [LICENSE](https://raw.githubusercontent.com/fabiorbarbosa/Result.Pattern.Sample/refs/heads/main/LICENSE) for details.