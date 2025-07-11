﻿// <auto-generated />
using System;
using KSE.GameStore.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace KSE.GameStore.Migrations.Migrations
{
    [DbContext(typeof(GameStoreDbContext))]
    partial class GameStoreDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<int>("PublisherId")
                        .HasColumnType("int")
                        .HasColumnName("publisher_id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("title");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("PublisherId");

                    b.ToTable("games", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.GamePrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("end_date");

                    b.Property<int>("GameId")
                        .HasColumnType("int")
                        .HasColumnName("game_id");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("start_date");

                    b.Property<int?>("Stock")
                        .HasColumnType("int")
                        .HasColumnName("stock");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("price_value");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("game_prices", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("genres", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("orders", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GameId")
                        .HasColumnType("int")
                        .HasColumnName("game_id");

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("order_id");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("price");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("OrderId");

                    b.ToTable("order_items", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Confirmed")
                        .HasColumnType("bit")
                        .HasColumnName("confirmed");

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("order_id");

                    b.Property<DateTime>("PayedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("payed_at");

                    b.Property<int>("PaymentMethod")
                        .HasColumnType("int")
                        .HasColumnName("payment_method");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("payments", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("permissions", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "All.View"
                        },
                        new
                        {
                            Id = 2,
                            Name = "All.Create"
                        },
                        new
                        {
                            Id = 3,
                            Name = "All.Update"
                        },
                        new
                        {
                            Id = 4,
                            Name = "All.Delete"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Games.View"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Games.Create"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Games.Update"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Games.Delete"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Users.Create"
                        },
                        new
                        {
                            Id = 10,
                            Name = "Users.Update"
                        },
                        new
                        {
                            Id = 11,
                            Name = "Users.Delete"
                        });
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Platform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("platforms", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Publisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.Property<string>("WebsiteUrl")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("website_url");

                    b.HasKey("Id");

                    b.ToTable("publishers", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("bit");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("refresh_tokens", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("code");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("regions", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Manager"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Moderator"
                        },
                        new
                        {
                            Id = 4,
                            Name = "User"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Guest"
                        });
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.RolePermission", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("PermissionId")
                        .HasColumnType("int");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("role_permissions", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("email");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("name");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RegionId")
                        .HasColumnType("int")
                        .HasColumnName("region_id");

                    b.HasKey("Id");

                    b.HasIndex("RegionId");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.UserGameStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GameId")
                        .HasColumnType("int")
                        .HasColumnName("game_id");

                    b.Property<string>("License")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("license");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("UserId");

                    b.ToTable("user_game_stock", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.UserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("user_roles", (string)null);
                });

            modelBuilder.Entity("game_genres", b =>
                {
                    b.Property<int>("game_id")
                        .HasColumnType("int");

                    b.Property<int>("genre_id")
                        .HasColumnType("int");

                    b.HasKey("game_id", "genre_id");

                    b.HasIndex("genre_id");

                    b.ToTable("game_genres", (string)null);
                });

            modelBuilder.Entity("game_platforms", b =>
                {
                    b.Property<int>("game_id")
                        .HasColumnType("int");

                    b.Property<int>("platform_id")
                        .HasColumnType("int");

                    b.HasKey("game_id", "platform_id");

                    b.HasIndex("platform_id");

                    b.ToTable("game_platforms", (string)null);
                });

            modelBuilder.Entity("game_region_permissions", b =>
                {
                    b.Property<int>("game_id")
                        .HasColumnType("int");

                    b.Property<int>("region_id")
                        .HasColumnType("int");

                    b.HasKey("game_id", "region_id");

                    b.HasIndex("region_id");

                    b.ToTable("game_region_permissions", (string)null);
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Game", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Publisher", "Publisher")
                        .WithMany("Games")
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.GamePrice", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Game", "Game")
                        .WithMany("Prices")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Order", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.OrderItem", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KSE.GameStore.DataAccess.Entities.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Payment", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Order", "Order")
                        .WithOne("Payment")
                        .HasForeignKey("KSE.GameStore.DataAccess.Entities.Payment", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.RefreshToken", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.RolePermission", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KSE.GameStore.DataAccess.Entities.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.User", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Region", "Region")
                        .WithMany("Users")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Region");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.UserGameStock", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Game", "Game")
                        .WithMany("Customers")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KSE.GameStore.DataAccess.Entities.User", "User")
                        .WithMany("GameStock")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.UserRole", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KSE.GameStore.DataAccess.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("game_genres", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Game", null)
                        .WithMany()
                        .HasForeignKey("game_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KSE.GameStore.DataAccess.Entities.Genre", null)
                        .WithMany()
                        .HasForeignKey("genre_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("game_platforms", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Game", null)
                        .WithMany()
                        .HasForeignKey("game_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KSE.GameStore.DataAccess.Entities.Platform", null)
                        .WithMany()
                        .HasForeignKey("platform_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("game_region_permissions", b =>
                {
                    b.HasOne("KSE.GameStore.DataAccess.Entities.Game", null)
                        .WithMany()
                        .HasForeignKey("game_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KSE.GameStore.DataAccess.Entities.Region", null)
                        .WithMany()
                        .HasForeignKey("region_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Game", b =>
                {
                    b.Navigation("Customers");

                    b.Navigation("Prices");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Order", b =>
                {
                    b.Navigation("OrderItems");

                    b.Navigation("Payment");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Publisher", b =>
                {
                    b.Navigation("Games");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Region", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.Role", b =>
                {
                    b.Navigation("RolePermissions");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("KSE.GameStore.DataAccess.Entities.User", b =>
                {
                    b.Navigation("GameStock");

                    b.Navigation("Orders");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
