﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Poort8.Dataspace.AuthorizationRegistry;

#nullable disable

namespace Poort8.Dataspace.AuthorizationRegistry.Migrations
{
    [DbContext(typeof(AuthorizationContext))]
    partial class AuthorizationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Action", b =>
                {
                    b.Property<string>("ActionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AdditionalType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProductId")
                        .HasColumnType("TEXT");

                    b.HasKey("ActionId");

                    b.HasIndex("ProductId");

                    b.ToTable("Actions");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee", b =>
                {
                    b.Property<string>("EmployeeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FamilyName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GivenName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("EmployeeId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization", b =>
                {
                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("InvoicingContact")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Representative")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Identifier");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Policy", b =>
                {
                    b.Property<string>("PolicyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("IssuedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Issuer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("NotBefore")
                        .HasColumnType("TEXT");

                    b.Property<string>("Resource")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PolicyId");

                    b.ToTable("Policies");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Product", b =>
                {
                    b.Property<string>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Provider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Property", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmployeeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .HasColumnType("TEXT");

                    b.Property<string>("PolicyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServiceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PropertyId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("OrganizationIdentifier");

                    b.HasIndex("PolicyId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ServiceId");

                    b.ToTable("Property");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Service", b =>
                {
                    b.Property<string>("ServiceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ServiceId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("ProductService", b =>
                {
                    b.Property<string>("ProductsProductId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServicesServiceId")
                        .HasColumnType("TEXT");

                    b.HasKey("ProductsProductId", "ServicesServiceId");

                    b.HasIndex("ServicesServiceId");

                    b.ToTable("ProductService");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Action", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Product", null)
                        .WithMany("PotentialActions")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization", "Organization")
                        .WithMany("Employees")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Property", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee", null)
                        .WithMany("Properties")
                        .HasForeignKey("EmployeeId");

                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization", null)
                        .WithMany("Properties")
                        .HasForeignKey("OrganizationIdentifier");

                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Policy", null)
                        .WithMany("Properties")
                        .HasForeignKey("PolicyId");

                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Product", null)
                        .WithMany("Properties")
                        .HasForeignKey("ProductId");

                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Service", null)
                        .WithMany("Properties")
                        .HasForeignKey("ServiceId");
                });

            modelBuilder.Entity("ProductService", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Service", null)
                        .WithMany()
                        .HasForeignKey("ServicesServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee", b =>
                {
                    b.Navigation("Properties");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization", b =>
                {
                    b.Navigation("Employees");

                    b.Navigation("Properties");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Policy", b =>
                {
                    b.Navigation("Properties");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Product", b =>
                {
                    b.Navigation("PotentialActions");

                    b.Navigation("Properties");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Service", b =>
                {
                    b.Navigation("Properties");
                });
#pragma warning restore 612, 618
        }
    }
}
