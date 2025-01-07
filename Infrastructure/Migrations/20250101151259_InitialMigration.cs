using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostOfficeCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomAmenities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAmenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    PricePerNight = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FloorsNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomAmenityRoomType",
                columns: table => new
                {
                    AmenitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomTypesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAmenityRoomType", x => new { x.AmenitiesId, x.RoomTypesId });
                    table.ForeignKey(
                        name: "FK_RoomAmenityRoomType_RoomAmenities_AmenitiesId",
                        column: x => x.AmenitiesId,
                        principalTable: "RoomAmenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomAmenityRoomType_RoomTypes_RoomTypesId",
                        column: x => x.RoomTypesId,
                        principalTable: "RoomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomTypes_RoomTypeId",
                        column: x => x.RoomTypeId,
                        principalTable: "RoomTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Country", "CountryCode", "Name", "PostOfficeCode" },
                values: new object[,]
                {
                    { new Guid("3c7e66f5-46a9-4b8d-8e90-85b5a9e2c2fd"), "Japan", "JP", "Tokyo", "TKY" },
                    { new Guid("8d2aeb7a-7c67-4911-aa2c-d6a3b4dc7e9e"), "United Kingdom", "UK", "London", "LDN" },
                    { new Guid("f9e85d04-548c-4f98-afe9-2a8831c62a90"), "United States", "US", "New York", "NYC" }
                });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "Id", "Category", "HotelId", "PricePerNight" },
                values: new object[,]
                {
                    { new Guid("4b4c0ea5-0b9a-4a20-8ad9-77441fb912d2"), 2, new Guid("9461e08b-92d3-45da-b6b3-efc0cfcc4a3a"), 200f },
                    { new Guid("5a5de3b8-3ed8-4f0a-bda9-cf73225a64a1"), 0, new Guid("98c2c9fe-1a1c-4eaa-a7f5-b9d19b246c27"), 100f },
                    { new Guid("d67ddbe4-1f1a-4d85-bcc1-ec3a475ecb68"), 3, new Guid("bfa4173d-7893-48b9-a497-5f4c7fb2492b"), 150f }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "PasswordHash", "PhoneNumber", "Salt", "isAdmin" },
                values: new object[,]
                {
                    { new Guid("aaf21a7d-8fc3-4c9f-8a8e-1eeec8dcd462"), "alice.smith@example.com", "Alice", "Smith", "hashedpassword2", "0987654321", "salt2", false },
                    { new Guid("c6c45f7c-2dfe-4c1e-9a9b-8b173c71b32c"), "john.doe@example.com", "John", "Doe", "hashedpassword1", "1234567890", "salt1", false },
                    { new Guid("f44c3eb4-2c8a-4a77-a31b-04c4619aa15a"), "robert.johnson@example.com", "Robert", "Johnson", "hashedpassword3", "1122334455", "salt3", true }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingDate", "CheckInDate", "CheckOutDate", "Price", "RoomId", "UserId" },
                values: new object[,]
                {
                    { new Guid("0bf4a177-98b8-4f67-8a56-95669c320890"), new DateTime(2025, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 200.0, new Guid("c6898b7e-ee09-4b36-8b20-22e8c2a63e29"), new Guid("f44c3eb4-2c8a-4a77-a31b-04c4619aa15a") },
                    { new Guid("7d3155a2-95f8-4d9b-bc24-662ae053f1c9"), new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 100.0, new Guid("a98b8a9d-4c5a-4a90-a2d2-5f1441b93db6"), new Guid("aaf21a7d-8fc3-4c9f-8a8e-1eeec8dcd462") },
                    { new Guid("efeb3d13-3dab-46c9-aa9a-9f22dd58e06e"), new DateTime(2025, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 150.0, new Guid("4e1cb3d9-bc3b-4997-a3d5-0c56cf17fe7a"), new Guid("c6c45f7c-2dfe-4c1e-9a9b-8b173c71b32c") }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "CityId", "Description", "FloorsNumber", "Name", "PhoneNumber", "Rating", "StreetAddress" },
                values: new object[,]
                {
                    { new Guid("9461e08b-92d3-45da-b6b3-efc0cfcc4a3a"), new Guid("3c7e66f5-46a9-4b8d-8e90-85b5a9e2c2fd"), "A resort with breathtaking sunset views over the ocean.", 5, "Sunset Resort", "312345678", 4.2f, "789 Beachfront Road" },
                    { new Guid("98c2c9fe-1a1c-4eaa-a7f5-b9d19b246c27"), new Guid("f9e85d04-548c-4f98-afe9-2a8831c62a90"), "A luxurious hotel with top-notch amenities.", 10, "Luxury Inn", "1234567890", 4.5f, "123 Main Street" },
                    { new Guid("bfa4173d-7893-48b9-a497-5f4c7fb2492b"), new Guid("8d2aeb7a-7c67-4911-aa2c-d6a3b4dc7e9e"), "A cozy lodge nestled in the heart of nature.", 3, "Cozy Lodge", "2012345678", 3.8f, "456 Oak Avenue" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "Rating", "RoomTypeId" },
                values: new object[] { new Guid("a98b8a9d-4c5a-4a90-a2d2-5f1441b93db6"), 2, 4.5f, new Guid("5a5de3b8-3ed8-4f0a-bda9-cf73225a64a1") });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "BookingId", "Method", "Status" },
                values: new object[,]
                {
                    { new Guid("1c8d70bd-2534-4991-bddf-84c7edee1a79"), 1200.0, new Guid("efeb3d13-3dab-46c9-aa9a-9f22dd58e06e"), 1, 0 },
                    { new Guid("7f5cc9f0-796f-498d-9f3f-9e5249a4f6ae"), 1500.0, new Guid("0bf4a177-98b8-4f67-8a56-95669c320890"), 2, 1 },
                    { new Guid("8f974636-4f53-48d9-af99-2f7f1d3e0474"), 2000.0, new Guid("7d3155a2-95f8-4d9b-bc24-662ae053f1c9"), 0, 1 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "BookingId", "Comment", "Rating", "ReviewDate" },
                values: new object[,]
                {
                    { new Guid("192045db-c6db-49c9-aa6b-2e3d6c7f3b79"), new Guid("7d3155a2-95f8-4d9b-bc24-662ae053f1c9"), "Clean rooms and beautiful views.", 4.2f, new DateTime(2023, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("63e4bb25-28b1-4fc4-9b93-9254d94dab23"), new Guid("0bf4a177-98b8-4f67-8a56-95669c320890"), "Excellent service and comfortable stay!", 4.8f, new DateTime(2023, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("85a5a0b4-0e04-4c46-b7ac-6cf609e4f2aa"), new Guid("efeb3d13-3dab-46c9-aa9a-9f22dd58e06e"), "Friendly staff and great location.", 4.5f, new DateTime(2023, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CityId",
                table: "Hotels",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId",
                table: "Reviews",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomAmenityRoomType_RoomTypesId",
                table: "RoomAmenityRoomType",
                column: "RoomTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomTypeId",
                table: "Rooms",
                column: "RoomTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "RoomAmenityRoomType");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "RoomAmenities");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
