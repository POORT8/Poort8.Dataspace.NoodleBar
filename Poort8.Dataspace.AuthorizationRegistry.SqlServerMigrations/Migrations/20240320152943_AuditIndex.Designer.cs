﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Poort8.Dataspace.AuthorizationRegistry;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    [DbContext(typeof(AuthorizationContext))]
    [Migration("20240320152943_AuditIndex")]
    partial class AuditIndex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FeatureProduct", b =>
                {
                    b.Property<string>("FeaturesFeatureId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FeaturesFeatureId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("FeatureProduct");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Audit.EnforceAuditRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Allow")
                        .HasColumnType("bit");

                    b.Property<string>("Explain")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ResourceId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubjectId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp")
                        .IsDescending();

                    b.ToTable("EnforceAuditRecords");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Audit.EntityAuditRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Entity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp")
                        .IsDescending();

                    b.ToTable("EntityAuditRecords");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee", b =>
                {
                    b.Property<string>("EmployeeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FamilyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GivenName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EmployeeId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("ArEmployee", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee+EmployeeProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("bit");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PropertyId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("EmployeeProperty");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Feature", b =>
                {
                    b.Property<string>("FeatureId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FeatureId");

                    b.ToTable("ArFeature", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Feature+FeatureProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FeatureId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("bit");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PropertyId");

                    b.HasIndex("FeatureId");

                    b.ToTable("FeatureProperty");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization", b =>
                {
                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("InvoicingContact")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Representative")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Identifier");

                    b.ToTable("ArOrganization", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Organization+OrganizationProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("bit");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PropertyId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("OrganizationProperty");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Policy", b =>
                {
                    b.Property<string>("PolicyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Attribute")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Expiration")
                        .HasColumnType("bigint");

                    b.Property<long>("IssuedAt")
                        .HasColumnType("bigint");

                    b.Property<string>("IssuerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("License")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("NotBefore")
                        .HasColumnType("bigint");

                    b.Property<string>("ResourceId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceProvider")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubjectId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PolicyId");

                    b.ToTable("ArPolicy", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Policy+PolicyProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("bit");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PolicyId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PropertyId");

                    b.HasIndex("PolicyId");

                    b.ToTable("PolicyProperty");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Product", b =>
                {
                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Provider")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UseCase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId");

                    b.ToTable("ArProduct", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Product+ProductProperty", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("bit");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PropertyId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductProperty");
                });

            modelBuilder.Entity("FeatureProduct", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Feature", null)
                        .WithMany()
                        .HasForeignKey("FeaturesFeatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Feature+FeatureProperty", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Feature", null)
                        .WithMany("Properties")
                        .HasForeignKey("FeatureId")
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

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Product+ProductProperty", b =>
                {
                    b.HasOne("Poort8.Dataspace.AuthorizationRegistry.Entities.Product", null)
                        .WithMany("Properties")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Employee", b =>
                {
                    b.Navigation("Properties");
                });

            modelBuilder.Entity("Poort8.Dataspace.AuthorizationRegistry.Entities.Feature", b =>
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
                    b.Navigation("Properties");
                });
#pragma warning restore 612, 618
        }
    }
}
