#nullable disable

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BookStore.Core.Migrations
{
    [DbContext(typeof(BookStoreDbContext))]
    internal partial class BookStoreDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookStore.Core.Models.Book", b =>
            {
                b.Property<int>("Id")
                 .ValueGeneratedOnAdd()
                 .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("Author")
                 .IsRequired()
                 .HasMaxLength(100)
                 .HasColumnType("nvarchar(100)");

                b.Property<DateTime>("CreatedDate")
                 .ValueGeneratedOnAdd()
                 .HasColumnType("datetime2")
                 .HasDefaultValueSql("GETUTCDATE()");

                b.Property<decimal>("Price")
                 .HasColumnType("decimal(18,2)");

                b.Property<DateTime?>("PublishedDate")
                 .HasColumnType("datetime2");

                b.Property<string>("Title")
                 .IsRequired()
                 .HasMaxLength(200)
                 .HasColumnType("nvarchar(200)");

                b.Property<DateTime?>("UpdatedDate")
                 .HasColumnType("datetime2");

                b.HasKey("Id");

                b.ToTable("Books");
            });
#pragma warning restore 612, 618
        }
    }
}