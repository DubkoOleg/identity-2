using OlMag.Manufacture2.Tests.Maintenance;
using Xunit.Abstractions;

namespace OlMag.Manufacture2.Tests;

public class FullTests(WebAppFixture fixture, ITestOutputHelper outputHelper)
    : TestBase(fixture, outputHelper)
{


    [Fact]
    public async Task Success_test()
    {
        // Минимальный успешный путь выполнения заказа
        // Создать заказчика
        // Создать контактное лицо заказчика
        // Создать заказ менеджера по продажам
        // Создать заказ на производство
        // ...
    }
}