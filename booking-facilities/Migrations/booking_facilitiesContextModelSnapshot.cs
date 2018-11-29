﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using booking_facilities.Models;

namespace booking_facilities.Migrations
{
    [DbContext(typeof(booking_facilitiesContext))]
    partial class booking_facilitiesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("booking_facilities.Models.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("BookingDateTime");

                    b.Property<int>("FacilityId");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("BookingId");

                    b.HasIndex("FacilityId");

                    b.ToTable("Booking");
                });

            modelBuilder.Entity("booking_facilities.Models.Facility", b =>
                {
                    b.Property<int>("FacilityId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FacilityName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("SportId");

                    b.Property<int>("VenueId");

                    b.HasKey("FacilityId");

                    b.HasIndex("SportId");

                    b.HasIndex("VenueId");

                    b.ToTable("Facility");
                });

            modelBuilder.Entity("booking_facilities.Models.Sport", b =>
                {
                    b.Property<int>("SportId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SportName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("SportId");

                    b.ToTable("Sport");
                });

            modelBuilder.Entity("booking_facilities.Models.Venue", b =>
                {
                    b.Property<int>("VenueId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("VenueName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("VenueId");

                    b.ToTable("Venue");
                });

            modelBuilder.Entity("booking_facilities.Models.Booking", b =>
                {
                    b.HasOne("booking_facilities.Models.Facility", "Facility")
                        .WithMany()
                        .HasForeignKey("FacilityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("booking_facilities.Models.Facility", b =>
                {
                    b.HasOne("booking_facilities.Models.Sport", "Sport")
                        .WithMany()
                        .HasForeignKey("SportId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("booking_facilities.Models.Venue", "Venue")
                        .WithMany()
                        .HasForeignKey("VenueId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
