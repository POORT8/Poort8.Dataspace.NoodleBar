﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Poort8.Dataspace.AuthorizationRegistry;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    [DbContext(typeof(AuthorizationContext))]
    partial class AuthorizationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Audit.EnforceAuditRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Allow")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Attribute")
                        .HasColumnType("TEXT");

                    b.Property<string>("Explain")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IssuerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RequestContext")
                        .HasColumnType("TEXT");

                    b.Property<string>("ResourceId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ServiceProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp")
                        .IsDescending();

                    b.ToTable("EnforceAuditRecords");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Audit.EntityAuditRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Entity")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp")
                        .IsDescending();

                    b.ToTable("EntityAuditRecords");
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

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("EmployeeId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("ArEmployee", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee+EmployeeProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PropertyId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("EmployeeProperty");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization", b =>
                {
                    b.Property<string>("Identifier")
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

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Identifier");

                    b.ToTable("ArOrganization", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization+OrganizationProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PropertyId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("OrganizationProperty");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Policy", b =>
                {
                    b.Property<string>("PolicyId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Attribute")
                        .HasColumnType("TEXT");

                    b.Property<long>("Expiration")
                        .HasColumnType("INTEGER");

                    b.Property<long>("IssuedAt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("IssuerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("License")
                        .HasColumnType("TEXT");

                    b.Property<long>("NotBefore")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ResourceId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Rules")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServiceProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PolicyId");

                    b.ToTable("ArPolicy", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Policy+PolicyProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PolicyId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PropertyId");

                    b.HasIndex("PolicyId");

                    b.ToTable("PolicyProperty");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Resource", b =>
                {
                    b.Property<string>("ResourceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ResourceId");

                    b.ToTable("ArResource", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Resource+ResourceProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ResourceId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PropertyId");

                    b.HasIndex("ResourceId");

                    b.ToTable("ResourceProperty");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.ResourceGroup", b =>
                {
                    b.Property<string>("ResourceGroupId")
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

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ResourceGroupId");

                    b.ToTable("ArResourceGroup", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.ResourceGroup+ResourceGroupProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ResourceGroupId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PropertyId");

                    b.HasIndex("ResourceGroupId");

                    b.ToTable("ResourceGroupProperty");
                });

            modelBuilder.Entity("ResourceResourceGroup", b =>
                {
                    b.Property<string>("ResourceGroupId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ResourcesResourceId")
                        .HasColumnType("TEXT");

                    b.HasKey("ResourceGroupId", "ResourcesResourceId");

                    b.HasIndex("ResourcesResourceId");

                    b.ToTable("ResourceResourceGroup");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization", null)
                        .WithMany("Employees")
                        .HasForeignKey("OrganizationIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee+EmployeeProperty", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee", null)
                        .WithMany("Properties")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization+OrganizationProperty", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization", null)
                        .WithMany("Properties")
                        .HasForeignKey("OrganizationIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Policy+PolicyProperty", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Policy", null)
                        .WithMany("Properties")
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Resource+ResourceProperty", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Resource", null)
                        .WithMany("Properties")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.ResourceGroup+ResourceGroupProperty", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.ResourceGroup", null)
                        .WithMany("Properties")
                        .HasForeignKey("ResourceGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ResourceResourceGroup", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.ResourceGroup", null)
                        .WithMany()
                        .HasForeignKey("ResourceGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Resource", null)
                        .WithMany()
                        .HasForeignKey("ResourcesResourceId")
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

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Resource", b =>
                {
                    b.Navigation("Properties");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.ResourceGroup", b =>
                {
                    b.Navigation("Properties");
                });
#pragma warning restore 612, 618
        }
    }
}
