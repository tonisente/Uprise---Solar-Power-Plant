﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Uprise___Solar_Power_Plant.Data;

#nullable disable

namespace Uprise___Solar_Power_Plant.Migrations
{
    [DbContext(typeof(SolarPowerPlantDbContext))]
    [Migration("20250221085330_AddingUsersTable")]
    partial class AddingUsersTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Uprise___Solar_Power_Plant.Models.PowerPlant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("InstalationDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("LocationLatitude")
                        .HasColumnType("float");

                    b.Property<double>("LocationLongitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Power")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PowerPlants");
                });

            modelBuilder.Entity("Uprise___Solar_Power_Plant.Models.PowerPlantReading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("PowerOutput")
                        .HasColumnType("int");

                    b.Property<int>("PowerPlantId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReadAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PowerPlantId");

                    b.ToTable("PowerPlantsReading");
                });

            modelBuilder.Entity("Uprise___Solar_Power_Plant.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Uprise___Solar_Power_Plant.Models.PowerPlantReading", b =>
                {
                    b.HasOne("Uprise___Solar_Power_Plant.Models.PowerPlant", "PowerPlant")
                        .WithMany("PowerPlantReadings")
                        .HasForeignKey("PowerPlantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PowerPlant");
                });

            modelBuilder.Entity("Uprise___Solar_Power_Plant.Models.PowerPlant", b =>
                {
                    b.Navigation("PowerPlantReadings");
                });
#pragma warning restore 612, 618
        }
    }
}
