# Result.Pattern.Sample

This project demonstrates a robust implementation of the Result pattern in ASP.NET Core APIs. It provides a standardized way to handle HTTP responses, encapsulate success and error states, and organize code for better clarity and maintainability.

## ✨ Features

### **Core Result Pattern**
- **Generic Result Class**: `Result<T>` that can represent both success and error states
- **State Validation**: Automatic validation to ensure consistency (success results can't have errors, error results can't have values)
- **Type Safety**: Strongly typed responses with comprehensive error handling

### **HTTP Response Standardization**
- **Automatic Conversion**: Seamless conversion from `Result<T>` to HTTP responses
- **Status Code Mapping**: Proper HTTP status codes for different result types
- **Consistent Error Format**: Standardized error response structure

### **Result Types Supported**
- ✅ **Success** (200 OK) - Successful operations with data
- ✅ **Created** (201 Created) - Resource creation with location header
- ✅ **NoContent** (204 No Content) - Successful operations without data
- ✅ **NotFound** (404 Not Found) - Resource not found
- ✅ **Validation** (422 Unprocessable Entity) - Validation errors
- ✅ **Conflict** (409 Conflict) - Resource conflicts
- ✅ **Unauthorized** (401 Unauthorized) - Authentication required
- ✅ **Failure** (500 Internal Server Error) - General failures

### **Extension Methods**
- **`ToObjectResult()`**: Converts `Result<T>` to appropriate HTTP responses
- **`OnSuccess()`**: Execute actions on successful results with error handling
- **`OnFailure()`**: Execute actions on failed results with error handling

## 🏗️ Project Structure

```
Result.Pattern.Sample/
├── Controllers/
│   └── WeatherForecastController.cs    # Example API endpoints
├── Enums/
│   └── ResultType.cs                   # Result type definitions
├── Extensions/
│   └── ResultExtension.cs              # HTTP conversion and utility methods
├── Results/
│   └── Result.cs                       # Core Result pattern implementation
└── WeatherForecast.cs                  # Example data model
```

## 🚀 Usage Examples

### **Basic Success Response**
```csharp
public IActionResult Get()
{
    var data = GetWeatherData();
    var result = Result<IEnumerable<WeatherForecast>>.Success(data);
    return result.ToObjectResult();
}
```

### **Error Handling**
```csharp
public IActionResult GetByName(string name)
{
    if (string.IsNullOrEmpty(name))
        return Result<IEnumerable<WeatherForecast>>
            .ValidationError(["Name is required"])
            .ToObjectResult();

    var data = GetWeatherByName(name);
    if (!data.Any())
        return Result<IEnumerable<WeatherForecast>>
            .NotFound($"No weather found for '{name}'")
            .ToObjectResult();

    return Result<IEnumerable<WeatherForecast>>.Success(data).ToObjectResult();
}
```

### **Resource Creation**
```csharp
public IActionResult Create(WeatherForecast forecast)
{
    var createdForecast = CreateWeatherForecast(forecast);
    return Result<WeatherForecast>
        .Created(createdForecast, nameof(Get), new { id = createdForecast.Id })
        .ToObjectResult();
}
```

### **Using Extension Methods**
```csharp
public IActionResult GetWithLogging()
{
    var result = GetWeatherData()
        .OnSuccess(data => _logger.LogInformation($"Retrieved {data.Count()} weather records"))
        .OnFailure(errors => _logger.LogError($"Failed to get weather data: {string.Join(", ", errors)}"));
    
    return result.ToObjectResult();
}
```

## 📋 API Endpoints

### **GET /weatherforecast**
Returns a list of weather forecasts.

**Response (Success):**
```json
[
  {
    "date": "2024-01-15",
    "temperatureC": 25,
    "temperatureF": 77,
    "summary": "Warm"
  }
]
```

### **GET /weatherforecast/{name}**
Returns weather forecasts filtered by name.

**Response (Success):**
```json
[
  {
    "date": "2024-01-15",
    "temperatureC": 25,
    "temperatureF": 77,
    "summary": "Warm"
  }
]
```

**Response (NotFound):**
```json
{
  "errors": ["No weather found for 'InvalidName'"]
}
```

## 🛠️ Technologies Used

- **.NET 9.0**
- **ASP.NET Core Web API**
- **Swagger/OpenAPI** for API documentation
- **REST Client** for testing (included `.http` file)

## 🚀 How to Run

1. **Prerequisites**: Ensure you have .NET 9.0 SDK installed
2. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/Result.Pattern.Sample.git
   cd Result.Pattern.Sample
   ```
3. **Run the application**:
   ```bash
   dotnet run
   ```
4. **Access the API**:
   - Swagger UI: `http://localhost:5100/swagger`
   - API Endpoints: `http://localhost:5100/weatherforecast`

## 🧪 Testing

The project includes a `.http` file for testing with the REST Client extension in VS Code:

```http
### Get all weather forecasts
GET http://localhost:5100/weatherforecast

### Get weather by name
GET http://localhost:5100/weatherforecast/Warm
```

## 📚 Key Benefits

### **For Developers**
- **Consistency**: Standardized response format across all endpoints
- **Type Safety**: Compile-time checking of result types
- **Error Handling**: Centralized error management with proper HTTP status codes
- **Maintainability**: Clean separation of concerns and reusable components

### **For API Consumers**
- **Predictable Responses**: Consistent response structure
- **Clear Error Messages**: Descriptive error information
- **Proper HTTP Status Codes**: Accurate status codes for different scenarios

## 🔧 Customization

### **Adding New Result Types**
1. Add new enum value in `ResultType.cs`
2. Update the `ToObjectResult()` extension method
3. Add convenience methods in `Result.cs` if needed

### **Custom Error Handling**
```csharp
public static Result<T> CustomError<T>(string message, int statusCode)
    => new(false, ResultType.Failure, default, errors: [message], statusCode: (HttpStatusCode)statusCode);
```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## 📄 License

See [LICENSE](LICENSE) for details.