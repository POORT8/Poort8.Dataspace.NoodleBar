﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Poort8.Dataspace.OrganizationRegistry;

#nullable disable

namespace Poort8.Dataspace.OrganizationRegistry.Migrations
{
    [DbContext(typeof(OrganizationContext))]
    [Migration("20230928142648_AddedDomain")]
    partial class AddedDomain
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Organization", b =>
                {
                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<string>("Domain")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Identifier");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.OrganizationProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PropertyId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("OrganizationProperty");
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.OrganizationProperty", b =>
                {
                    b.HasOne("Poort8.Dataspace.OrganizationRegistry.Organization", null)
                        .WithMany("Properties")
                        .HasForeignKey("OrganizationIdentifier");
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Organization", b =>
                {
                    b.Navigation("Properties");
                });
#pragma warning restore 612, 618
        }
    }
}
