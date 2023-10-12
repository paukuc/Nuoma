﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(RentalDbContext))]
    [Migration("20231005141130_enums")]
    partial class enums
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Apartment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("FloorID")
                        .HasColumnType("int");

                    b.Property<int>("OwnerID")
                        .HasColumnType("int");

                    b.Property<int?>("RenterID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FloorID");

                    b.HasIndex("OwnerID");

                    b.HasIndex("RenterID");

                    b.ToTable("Apartments");
                });

            modelBuilder.Entity("Building", b =>
                {
                    b.Property<int>("BuildingID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("TotalFloors")
                        .HasColumnType("int");

                    b.HasKey("BuildingID");

                    b.ToTable("Buildings");
                });

            modelBuilder.Entity("Floor", b =>
                {
                    b.Property<int>("FloorID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BuildingID")
                        .HasColumnType("int");

                    b.Property<int>("FloorNumber")
                        .HasColumnType("int");

                    b.HasKey("FloorID");

                    b.HasIndex("BuildingID");

                    b.ToTable("Floors");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Apartment", b =>
                {
                    b.HasOne("Floor", null)
                        .WithMany("Apartments")
                        .HasForeignKey("FloorID");

                    b.HasOne("User", "Owner")
                        .WithMany("OwnedApartments")
                        .HasForeignKey("OwnerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("User", "Renter")
                        .WithMany("RentedApartments")
                        .HasForeignKey("RenterID");

                    b.Navigation("Owner");

                    b.Navigation("Renter");
                });

            modelBuilder.Entity("Floor", b =>
                {
                    b.HasOne("Building", "Building")
                        .WithMany("Floors")
                        .HasForeignKey("BuildingID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Building");
                });

            modelBuilder.Entity("Building", b =>
                {
                    b.Navigation("Floors");
                });

            modelBuilder.Entity("Floor", b =>
                {
                    b.Navigation("Apartments");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Navigation("OwnedApartments");

                    b.Navigation("RentedApartments");
                });
#pragma warning restore 612, 618
        }
    }
}
