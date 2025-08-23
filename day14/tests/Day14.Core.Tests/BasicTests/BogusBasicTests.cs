namespace Day14.Core.Tests.BasicTests;

/// <summary>
/// Bogus 基本功能測試
/// </summary>
public class BogusBasicTests
{
    [Fact]
    public void ProductFaker_Generate_應產生有效的產品資料()
    {
        // Arrange & Act
        var product = BogusDataGenerator.ProductFaker.Generate();

        // Assert
        product.Should().NotBeNull();
        product.Id.Should().BeGreaterThan(0);
        product.Name.Should().NotBeNull().And.NotBeEmpty();
        product.Description.Should().NotBeNull().And.NotBeEmpty();
        product.Price.Should().BeGreaterThan(0);
        product.Category.Should().NotBeNull().And.NotBeEmpty();
        product.CreatedDate.Should().BeBefore(DateTime.Now);
        product.StockQuantity.Should().BeGreaterThanOrEqualTo(0);
        product.Tags.Should().NotBeNull();
    }

    [Fact]
    public void UserFaker_Generate_應產生有效的使用者資料()
    {
        // Arrange & Act
        var user = BogusDataGenerator.UserFaker.Generate();

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBe(Guid.Empty);
        user.FirstName.Should().NotBeNull().And.NotBeEmpty();
        user.LastName.Should().NotBeNull().And.NotBeEmpty();
        user.Email.Should().NotBeNull().And.NotBeEmpty().And.Contain("@");
        user.Age.Should().BeInRange(18, 80);
        user.Department.Should().BeOneOf("IT", "HR", "Finance", "Marketing");
        user.Role.Should().BeOneOf("User", "Admin", "SuperAdmin");
        user.CreatedAt.Should().BeBefore(DateTime.Now);

        // Premium 使用者的積分應該較高
        if (user.IsPremium)
        {
            user.Points.Should().BeInRange(1000, 5000);
        }
        else
        {
            user.Points.Should().BeInRange(0, 500);
        }
    }

    [Fact]
    public void TaiwanPersonFaker_Generate_應產生台灣本土化資料()
    {
        // Arrange & Act
        var person = BogusDataGenerator.TaiwanPersonFaker.Generate();

        // Assert
        person.Should().NotBeNull();
        person.Id.Should().NotBe(Guid.Empty);
        person.Name.Should().NotBeNull().And.NotBeEmpty();
        person.City.Should().NotBeNull().And.NotBeEmpty();
        person.University.Should().NotBeNull().And.NotBeEmpty();
        person.Company.Should().NotBeNull().And.NotBeEmpty();
        person.IdCard.Should().NotBeNull().And.HaveLength(10);
        person.Mobile.Should().NotBeNull().And.StartWith("09").And.HaveLength(10);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void GenerateProducts_輸入不同數量_應產生對應數量的產品(int count)
    {
        // Arrange & Act
        var products = BogusDataGenerator.GenerateProducts(count);

        // Assert
        products.Should().HaveCount(count);
        products.Should().OnlyContain(p => !string.IsNullOrEmpty(p.Name));
        products.Should().OnlyContain(p => p.Price > 0);
    }

    [Fact]
    public void OrderFaker_Generate_應產生包含項目的訂單()
    {
        // Arrange & Act
        var order = BogusDataGenerator.OrderFaker.Generate();

        // Assert
        order.Should().NotBeNull();
        order.Id.Should().NotBe(Guid.Empty);
        order.OrderNumber.Should().NotBeNull().And.StartWith("ORD-");
        order.CustomerName.Should().NotBeNull().And.NotBeEmpty();
        order.CustomerEmail.Should().NotBeNull().And.Contain("@");
        order.Items.Should().NotBeEmpty().And.HaveCountGreaterThan(0);
        order.SubTotal.Should().BeGreaterThan(0);
        order.TaxAmount.Should().BeGreaterThan(0);
        order.TotalAmount.Should().BeGreaterThan(0);

        // 驗證總金額計算
        var expectedTotal = order.SubTotal + order.TaxAmount + order.ShippingFee;
        order.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact]
    public void EmployeeFaker_Generate_應產生具有合理業務邏輯的員工()
    {
        // Arrange & Act
        var employee = BogusDataGenerator.EmployeeFaker.Generate();

        // Assert
        employee.Should().NotBeNull();
        employee.Id.Should().NotBe(Guid.Empty);
        employee.EmployeeId.Should().StartWith("EMP-");
        employee.Email.Should().EndWith("@company.com");
        employee.Age.Should().BeInRange(22, 65);
        employee.HireDate.Should().BeBefore(DateTime.Now);
        employee.Skills.Should().NotBeEmpty();
        employee.Skills.Count.Should().BeInRange(1, 8);
        employee.Projects.Should().NotBeEmpty();

        // 驗證職級與年齡的關係
        switch (employee.Level)
        {
            case "Junior":
                employee.Age.Should().BeLessThan(25);
                employee.Salary.Should().BeInRange(35000, 50000);
                break;
            case "Senior":
                employee.Age.Should().BeInRange(25, 34);
                employee.Salary.Should().BeInRange(50000, 80000);
                break;
            case "Lead":
                employee.Age.Should().BeInRange(35, 44);
                employee.Salary.Should().BeInRange(80000, 120000);
                break;
            case "Principal":
                employee.Age.Should().BeGreaterThanOrEqualTo(45);
                employee.Salary.Should().BeInRange(120000, 200000);
                break;
        }

        // 驗證專案日期邏輯
        foreach (var project in employee.Projects)
        {
            project.StartDate.Should().BeAfter(employee.HireDate);
            project.EndDate.Should().BeAfter(project.StartDate);
            project.Technologies.Should().BeSubsetOf(employee.Skills);
        }
    }
}