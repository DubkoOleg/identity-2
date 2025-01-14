using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Text.Json;
using FluentAssertions;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OlMag.Manufacture2.Helpers.OperationResult;
using Xunit.Abstractions;
using OlMag.Manufacture2.Helpers;

namespace OlMag.Manufacture2.Tests.Maintenance;

public class TestBase(WebAppFixture fixture, ITestOutputHelper outputHelper) : IClassFixture<WebAppFixture>
{
    protected readonly HttpClient client = fixture.Application.CreateClient(new WebApplicationFactoryClientOptions()
    { AllowAutoRedirect = false, HandleCookies = false });
    protected readonly IMapper mapper = fixture.Application.Services.GetRequiredService<IMapper>();

    public readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /*protected async ValueTask Assert<T>(HttpResponseMessage response, TestResult info,
        Func<OperationResult<T>, ValueTask>? assert = default, bool dumpContent = false,
        [CallerMemberName] string callerName = "",
        [CallerFilePath] string callerPath = "")
    {
        if (dumpContent)
        {
            outputHelper.WriteLine("Dump response for {0}", callerPath);

            var path = Path.Combine(Path.GetDirectoryName(callerPath)!, "Responses",
                Path.GetFileNameWithoutExtension(callerPath), callerName);
            Directory.CreateDirectory(path);

            var filePath = Path.Combine(path, $"test_{DateTime.Now:s}.resp");
            outputHelper.WriteLine("Dump output: {0}", filePath);

            var content = await response.Content.ReadAsStringAsync();
            await File.AppendAllTextAsync(filePath, $"{Environment.NewLine}{DateTime.Now:G}\t{content}");
        }

        if (info.Value)
        {
            response.Should().BeSuccessful(await response.Content.ReadAsStringAsync());

            OperationResult<T>? data;
            if (response.IsSuccessStatusCode)
            {
                data = (await response.Content.ReadFromJsonAsync<T>(JsonTestExtensions
                    .JsonSerializerOptions))!;
            }
            else
            {
                data = await response.Content.ReadFromJsonAsync<OperationResult<T>>(JsonTestExtensions
                    .JsonSerializerOptions);
            }

            data.Should<OperationResult<T>, T>().BeTrue();

            if (assert != null) await assert.Invoke(data!);
        }
        else
        {
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }*/

    protected static HttpContent CreateContent<T>(T value) =>
        JsonContent.Create(value, options: SerializerOptionsHelper.SerializerOptions);
}