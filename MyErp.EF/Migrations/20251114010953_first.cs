using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MyErp.EF.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    AreaId = table.Column<string>(type: "longtext", nullable: true),
                    AreaName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "longtext", nullable: false),
                    TIN = table.Column<string>(type: "longtext", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Country = table.Column<string>(type: "longtext", nullable: false),
                    Mangername = table.Column<string>(type: "longtext", nullable: true),
                    branchType = table.Column<string>(type: "longtext", nullable: true),
                    phonenumber = table.Column<string>(type: "longtext", nullable: false),
                    email = table.Column<string>(type: "longtext", nullable: false),
                    MainBranchId = table.Column<string>(type: "longtext", nullable: false),
                    latitude = table.Column<string>(type: "longtext", nullable: false),
                    logo = table.Column<string>(type: "longtext", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false),
                    Governate = table.Column<string>(type: "longtext", nullable: false),
                    Street = table.Column<string>(type: "longtext", nullable: false),
                    RegionCity = table.Column<string>(type: "longtext", nullable: false),
                    Address = table.Column<string>(type: "longtext", nullable: false),
                    BuildingNumber = table.Column<string>(type: "longtext", nullable: false),
                    Floor = table.Column<string>(type: "longtext", nullable: false),
                    LandMark = table.Column<string>(type: "longtext", nullable: false),
                    PostalCode = table.Column<string>(type: "longtext", nullable: false),
                    Room = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Internal_Id = table.Column<string>(type: "longtext", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    InternalId = table.Column<string>(type: "longtext", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false),
                    notes = table.Column<string>(type: "longtext", nullable: false),
                    ExchangeRate = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    InternalId = table.Column<string>(type: "longtext", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Phone = table.Column<string>(type: "longtext", nullable: true),
                    Phone2 = table.Column<string>(type: "longtext", nullable: true),
                    Phone3 = table.Column<string>(type: "longtext", nullable: true),
                    Email = table.Column<string>(type: "longtext", nullable: true),
                    CustomerType = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "longtext", nullable: true),
                    Governate = table.Column<string>(type: "longtext", nullable: true),
                    RegionCity = table.Column<string>(type: "longtext", nullable: true),
                    Street = table.Column<string>(type: "longtext", nullable: true),
                    BuildingNumber = table.Column<string>(type: "longtext", nullable: true),
                    address = table.Column<string>(type: "longtext", nullable: true),
                    opening_credit = table.Column<string>(type: "longtext", nullable: true),
                    opening_debit = table.Column<string>(type: "longtext", nullable: true),
                    treasurycode = table.Column<string>(type: "longtext", nullable: false),
                    SchemeID = table.Column<string>(type: "longtext", nullable: true),
                    treeCode = table.Column<string>(type: "longtext", nullable: false),
                    parentAcc = table.Column<string>(type: "longtext", nullable: true),
                    AdditionalNotes = table.Column<string>(type: "longtext", nullable: true),
                    companyType = table.Column<string>(type: "longtext", nullable: true),
                    companyTaxNo = table.Column<string>(type: "longtext", nullable: true),
                    companyAdress = table.Column<string>(type: "longtext", nullable: true),
                    companyCeoName = table.Column<string>(type: "longtext", nullable: true),
                    companyCeoNumber = table.Column<string>(type: "longtext", nullable: true),
                    companyopenningbalance = table.Column<string>(type: "longtext", nullable: true),
                    companyName = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    InternalId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    title = table.Column<int>(type: "int", nullable: false),
                    phoneNum = table.Column<string>(type: "longtext", nullable: false),
                    city = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesMen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    salesmanId = table.Column<string>(type: "longtext", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    address = table.Column<string>(type: "longtext", nullable: true),
                    email = table.Column<string>(type: "longtext", nullable: true),
                    phone_num = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesMen", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockReqDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockReqDetails", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockReqs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockReqs", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "longtext", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Location = table.Column<string>(type: "longtext", nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Treasuries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treasuries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Treasuries_Treasuries_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Treasuries",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Password = table.Column<string>(type: "longtext", nullable: false),
                    Rights = table.Column<string>(type: "longtext", nullable: true),
                    GroupRolId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    internalId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTypes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CashAndBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "longtext", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    location = table.Column<string>(type: "longtext", nullable: false),
                    Note1 = table.Column<string>(type: "longtext", nullable: false),
                    Note2 = table.Column<string>(type: "longtext", nullable: false),
                    AccountNo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    currencyname = table.Column<string>(type: "longtext", nullable: false),
                    exchangeRate = table.Column<string>(type: "longtext", nullable: false),
                    madeen = table.Column<string>(type: "longtext", nullable: false),
                    treasuryCode = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    isActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalInflow = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOutflow = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashAndBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashAndBanks_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    InternalId = table.Column<string>(type: "longtext", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    statment = table.Column<string>(type: "longtext", nullable: true),
                    statment2 = table.Column<string>(type: "longtext", nullable: true),
                    UnitCode = table.Column<string>(type: "longtext", nullable: true),
                    Barcode = table.Column<string>(type: "longtext", nullable: true),
                    TypeId = table.Column<string>(type: "longtext", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    purchaseprice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sellingprice1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sellingprice2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sellingprice3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sellingprice4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sellingprice5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sellingprice6 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sellingprice7 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sellingprice8 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    serialnumber = table.Column<string>(type: "longtext", nullable: true),
                    sellingprice9 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    sellingprice10 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    discount1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    discount2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    discount3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    discount4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    limit = table.Column<int>(type: "int", nullable: true),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DescaraptionEnglish = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    image = table.Column<string>(type: "longtext", nullable: true),
                    StockId = table.Column<int>(type: "int", nullable: true),
                    profitpercatanage = table.Column<int>(type: "int", nullable: true),
                    created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    expire = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TableTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    wholesaleprice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HALFwholesaleprice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Sectoral = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StockActiontype = table.Column<int>(type: "int", nullable: false),
                    physicalinvNumber = table.Column<string>(type: "longtext", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    EmpName = table.Column<string>(type: "longtext", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CurName = table.Column<string>(type: "longtext", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UseName = table.Column<string>(type: "longtext", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    StoName = table.Column<string>(type: "longtext", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockActions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockActions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockActions_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockActions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockActiontransfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    physicalinvNumber = table.Column<string>(type: "longtext", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    EmpName = table.Column<string>(type: "longtext", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CurName = table.Column<string>(type: "longtext", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UseName = table.Column<string>(type: "longtext", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    ToStockId = table.Column<int>(type: "int", nullable: false),
                    StoName = table.Column<string>(type: "longtext", nullable: false),
                    StoName2 = table.Column<string>(type: "longtext", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockActiontransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockActiontransfers_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockActiontransfers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockActiontransfers_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockActiontransfers_Stocks_ToStockId",
                        column: x => x.ToStockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockActiontransfers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ordermes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    internalId = table.Column<string>(type: "longtext", nullable: true),
                    orderType = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Notes2 = table.Column<string>(type: "longtext", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    currencyBalance = table.Column<int>(type: "int", nullable: true),
                    treasuryAcc = table.Column<string>(type: "longtext", nullable: true),
                    customerId = table.Column<int>(type: "int", nullable: false),
                    CreditPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Notes1 = table.Column<string>(type: "longtext", nullable: true),
                    CustomerName = table.Column<string>(type: "longtext", nullable: true),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<string>(type: "longtext", nullable: true),
                    wayofpayemnt = table.Column<int>(type: "int", nullable: true),
                    totDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashAndBankId = table.Column<int>(type: "int", nullable: false),
                    barcode = table.Column<string>(type: "longtext", nullable: true),
                    POSId = table.Column<int>(type: "int", nullable: true),
                    receipt = table.Column<string>(type: "longtext", nullable: true),
                    taxcode = table.Column<string>(type: "longtext", nullable: true),
                    invoice = table.Column<string>(type: "longtext", nullable: true),
                    CustomerPhone = table.Column<string>(type: "longtext", nullable: true),
                    CustomerCountry = table.Column<string>(type: "longtext", nullable: true),
                    AccountRec = table.Column<string>(type: "longtext", nullable: true),
                    SalesAcc = table.Column<string>(type: "longtext", nullable: true),
                    baseordertype = table.Column<int>(type: "int", nullable: true),
                    offerprice = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordermes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ordermes_CashAndBanks_CashAndBankId",
                        column: x => x.CashAndBankId,
                        principalTable: "CashAndBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordermes_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordermes_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordermes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockTakings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "longtext", nullable: false),
                    internalId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    stockId = table.Column<int>(type: "int", nullable: false),
                    productId = table.Column<int>(type: "int", nullable: false),
                    Curenncy = table.Column<string>(type: "longtext", nullable: false),
                    currencyId = table.Column<int>(type: "int", nullable: false),
                    exchangeRate = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTakings_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTakings_Currencies_currencyId",
                        column: x => x.currencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTakings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTakings_Products_productId",
                        column: x => x.productId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTakings_Stocks_stockId",
                        column: x => x.stockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CashFlows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CashAndBankId = table.Column<int>(type: "int", nullable: false),
                    OrdermeId = table.Column<int>(type: "int", nullable: false),
                    ordertype = table.Column<string>(type: "longtext", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsInflow = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Customer = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashFlows_CashAndBanks_CashAndBankId",
                        column: x => x.CashAndBankId,
                        principalTable: "CashAndBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashFlows_Ordermes_OrdermeId",
                        column: x => x.OrdermeId,
                        principalTable: "Ordermes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ordermedetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "longtext", nullable: false),
                    OrdermeId = table.Column<int>(type: "int", nullable: false),
                    internalId = table.Column<string>(type: "longtext", nullable: false),
                    Productname = table.Column<string>(type: "longtext", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    beforeTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    afterTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    qty = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    unitcode = table.Column<string>(type: "longtext", nullable: false),
                    categoryId = table.Column<int>(type: "int", nullable: false),
                    createdProddate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    expiredate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    serialnumber = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordermedetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ordermedetails_Categories_categoryId",
                        column: x => x.categoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordermedetails_Ordermes_OrdermeId",
                        column: x => x.OrdermeId,
                        principalTable: "Ordermes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordermedetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockActionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StockActionsId = table.Column<int>(type: "int", nullable: true),
                    StockActiontransferId = table.Column<int>(type: "int", nullable: true),
                    OrdermeId = table.Column<int>(type: "int", nullable: true),
                    internalId = table.Column<string>(type: "longtext", nullable: false),
                    FinalValue = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: true),
                    productName = table.Column<string>(type: "longtext", nullable: true),
                    UnitTypes = table.Column<string>(type: "longtext", nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    qty = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "longtext", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    createdProddate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    expiredate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    serialnumber = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockActionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockActionDetails_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockActionDetails_Ordermes_OrdermeId",
                        column: x => x.OrdermeId,
                        principalTable: "Ordermes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockActionDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockActionDetails_StockActions_StockActionsId",
                        column: x => x.StockActionsId,
                        principalTable: "StockActions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockActionDetails_StockActiontransfers_StockActiontransferId",
                        column: x => x.StockActiontransferId,
                        principalTable: "StockActiontransfers",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CashAndBanks_CurrencyId",
                table: "CashAndBanks",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlows_CashAndBankId",
                table: "CashFlows",
                column: "CashAndBankId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlows_OrdermeId",
                table: "CashFlows",
                column: "OrdermeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordermedetails_categoryId",
                table: "Ordermedetails",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordermedetails_OrdermeId",
                table: "Ordermedetails",
                column: "OrdermeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordermedetails_ProductId",
                table: "Ordermedetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordermes_CashAndBankId",
                table: "Ordermes",
                column: "CashAndBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordermes_CurrencyId",
                table: "Ordermes",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordermes_StockId",
                table: "Ordermes",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordermes_UserId",
                table: "Ordermes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CustomerId",
                table: "Products",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypes_CategoryId",
                table: "ProductTypes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActionDetails_CategoryId",
                table: "StockActionDetails",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActionDetails_OrdermeId",
                table: "StockActionDetails",
                column: "OrdermeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActionDetails_ProductId",
                table: "StockActionDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActionDetails_StockActionsId",
                table: "StockActionDetails",
                column: "StockActionsId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActionDetails_StockActiontransferId",
                table: "StockActionDetails",
                column: "StockActiontransferId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActions_CurrencyId",
                table: "StockActions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActions_EmployeeId",
                table: "StockActions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActions_StockId",
                table: "StockActions",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActions_UserId",
                table: "StockActions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActiontransfers_CurrencyId",
                table: "StockActiontransfers",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActiontransfers_EmployeeId",
                table: "StockActiontransfers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActiontransfers_StockId",
                table: "StockActiontransfers",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActiontransfers_ToStockId",
                table: "StockActiontransfers",
                column: "ToStockId");

            migrationBuilder.CreateIndex(
                name: "IX_StockActiontransfers_UserId",
                table: "StockActiontransfers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_CategoryId",
                table: "StockTakings",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_currencyId",
                table: "StockTakings",
                column: "currencyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_EmployeeId",
                table: "StockTakings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_productId",
                table: "StockTakings",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_stockId",
                table: "StockTakings",
                column: "stockId");

            migrationBuilder.CreateIndex(
                name: "IX_Treasuries_ParentId",
                table: "Treasuries",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "CashFlows");

            migrationBuilder.DropTable(
                name: "Ordermedetails");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropTable(
                name: "SalesMen");

            migrationBuilder.DropTable(
                name: "StockActionDetails");

            migrationBuilder.DropTable(
                name: "StockReqDetails");

            migrationBuilder.DropTable(
                name: "StockReqs");

            migrationBuilder.DropTable(
                name: "StockTakings");

            migrationBuilder.DropTable(
                name: "Treasuries");

            migrationBuilder.DropTable(
                name: "Ordermes");

            migrationBuilder.DropTable(
                name: "StockActions");

            migrationBuilder.DropTable(
                name: "StockActiontransfers");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "CashAndBanks");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
